using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace EventAggregator.Shared.Extensions;

public static class ChannelReaderExtensions
{
    /// <summary>
    /// Merges multiple channel readers into a single asynchronous stream.
    /// The merged stream completes successfully when all source channels are completed normally,
    /// but terminates with an exception if any source channel completes with an error.
    /// </summary>
    public static async IAsyncEnumerable<T> MergeAsyncExtension<T>(this IReadOnlyList<ChannelReader<T>> channels,
        int bufferSize = 100,
        BoundedChannelFullMode fullMode = BoundedChannelFullMode.Wait,
        [EnumeratorCancellation] CancellationToken mainCancellationToken = default)
    {
        if (channels == null || !channels.Any())
        {
            yield break;
        }

        var outputChannel = Channel.CreateBounded<T>(new BoundedChannelOptions(bufferSize)
        {
            FullMode = fullMode,
            SingleReader = true,
            SingleWriter = false
        });

        using var channelCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(mainCancellationToken);
        var completionTasks = new Task[channels.Count];

        try
        {
            for (int i = 0; i < channels.Count; i++)
            {
                completionTasks[i] = ReadFromChannelAsync(channels[i], outputChannel.Writer, channelCancellationTokenSource);
            }
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.WhenAll(completionTasks);
                }
                finally
                {
                    outputChannel.Writer.TryComplete();
                }
            }, mainCancellationToken);

            await foreach (var item in outputChannel.Reader.ReadAllAsync(mainCancellationToken))
            {
                yield return item;
            }
        }
        finally
        {
            if (completionTasks.Length > 0)
            {
                try
                {
                    await Task.WhenAll(completionTasks).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    //TODO Log
                }
            }
        }
    }

    private static async Task ReadFromChannelAsync<T>(
        ChannelReader<T> reader,
        ChannelWriter<T> writer,
        CancellationTokenSource cancellationTokenSource)
    {
        try
        {
            await foreach (var item in reader.ReadAllAsync(cancellationTokenSource.Token))
            {
                await writer.WriteAsync(item, cancellationTokenSource.Token);
            }
        }
        catch (OperationCanceledException) when (cancellationTokenSource.Token.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            cancellationTokenSource.Cancel();
            writer.TryComplete(ex);
            throw;
        }
    }
}
