using EventAggregator.Shared.MessageBrokers.Abstractions;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace EventAggregator.Shared.Helpers;

public class ChannelHelpers
{
    /// <summary>
    /// Merges 2 IMessage channel readers into a single asynchronous IMessage stream.
    /// The merged stream completes successfully when all source channels are completed normally,
    /// but terminates with an exception if any source channel completes with an error.
    /// </summary>
    public static async IAsyncEnumerable<IMessage> MergeAsync<TStartActionMessage, TBreakActionMessage>(
        ChannelReader<TStartActionMessage> startActionMessageChannel,
        ChannelReader<TBreakActionMessage> breakActionMessageChannel,
        int bufferSize = 100,
        BoundedChannelFullMode fullMode = BoundedChannelFullMode.Wait,
        [EnumeratorCancellation] CancellationToken mainCancellationToken = default)
        where TStartActionMessage : IMessage
        where TBreakActionMessage : IMessage
    {
        if (startActionMessageChannel == null 
            || breakActionMessageChannel == null)
        {
            yield break;
        }

        var outputChannel = Channel.CreateBounded<IMessage>(new BoundedChannelOptions(bufferSize)
        {
            FullMode = fullMode,
            SingleReader = true,
            SingleWriter = false
        });

        using var channelCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(mainCancellationToken);
        var completionTasks = new Task[2];

        try
        {
            completionTasks[0] = ReadFromChannelAsync(startActionMessageChannel, outputChannel.Writer, channelCancellationTokenSource);
            completionTasks[1] = ReadFromChannelAsync(breakActionMessageChannel, outputChannel.Writer, channelCancellationTokenSource);
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
                finally
                {
                    completionTasks = null;
                }
            }
        }
    }

    private static async Task ReadFromChannelAsync<T>(
        ChannelReader<T> reader,
        ChannelWriter<IMessage> writer,
        CancellationTokenSource cancellationTokenSource) where T : IMessage
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
