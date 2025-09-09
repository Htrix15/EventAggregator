using EventAggregator.Shared.Extensions;
using System.Threading.Channels;

namespace EventAggregator.Shared.Tests.ExtensionsTests;

[TestFixture]
internal class MergeAsyncExtensionTests
{
    [Test]
    public async Task MergeAsync_EmptyChannels_ReturnsEmptyStream()
    {
        // Arrange
        var channels = new List<ChannelReader<int>>();

        // Act
        var result = new List<int>();
        await foreach (var item in channels.MergeAsyncExtension())
        {
            result.Add(item);
        }

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task MergeAsync_NullChannels_ReturnsEmptyStream()
    {
        // Arrange
        List<ChannelReader<int>> channels = null;

        // Act
        var result = new List<int>();
        await foreach (var item in channels.MergeAsyncExtension())
        {
            result.Add(item);
        }

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task MergeAsync_EmptyChannels_CompletesImmediately()
    {
        // Arrange
        var channel1 = Channel.CreateBounded<int>(10);
        var channel2 = Channel.CreateBounded<int>(10);
        var channels = new List<ChannelReader<int>> { channel1.Reader, channel2.Reader };

        channel1.Writer.Complete();
        channel2.Writer.Complete();

        // Act
        var result = new List<int>();
        await foreach (var item in channels.MergeAsyncExtension())
        {
            result.Add(item);
        }

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task MergeAsync_SingleChannel_ReturnsAllItems()
    {
        // Arrange
        var channel = Channel.CreateBounded<int>(10);
        var channels = new List<ChannelReader<int>> { channel.Reader };

        for (int i = 0; i < 3; i++)
        {
            await channel.Writer.WriteAsync(i);
        }
        channel.Writer.Complete();

        // Act
        var result = new List<int>();
        await foreach (var item in channels.MergeAsyncExtension())
        {
            result.Add(item);
        }

        // Assert
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result, Is.EquivalentTo(new[] { 0, 1, 2 }));
    }

    [Test]
    public async Task MergeAsync_MultipleChannels_MergesAllItems()
    {
        // Arrange
        var channel1 = Channel.CreateBounded<int>(2);
        var channel2 = Channel.CreateBounded<int>(4);

        var channels = new List<ChannelReader<int>> { channel1.Reader, channel2.Reader };

        var channel1Writer = new ValueTask();
        var channel2Writer = new ValueTask();

        var channel1Task = Task.Run(async () =>
        {
            for (int i = 1; i < 3; i++)
            {
                channel1Writer = channel1.Writer.WriteAsync(i);
                await Task.Delay(100);
            }
            channel1.Writer.Complete();
        });

        var channel2Task = Task.Run(async () =>
        {
            for (int i = 10; i < 13; i++)
            {
                channel2Writer = channel2.Writer.WriteAsync(i);
                await Task.Delay(10);
            }
            channel2.Writer.Complete();
        });

        // Act
        var result = new List<int>();
        await foreach (var item in channels.MergeAsyncExtension())
        {
            result.Add(item);
        }

        // Assert
        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result, Is.EquivalentTo(new[] { 1, 2, 10, 11, 12 }));
    }

    [Test]
    public async Task MergeAsync_BigAndSmallChannels_MergesAllItems()
    {
        // Arrange
        var channel1 = Channel.CreateBounded<int>(200);
        var channel2 = Channel.CreateBounded<int>(10000);

        var channels = new List<ChannelReader<int>> { channel1.Reader, channel2.Reader };

        var channel1Writer = new ValueTask();
        var channel2Writer = new ValueTask();

        var channel1Task = Task.Run(async () =>
        {
            for (int i = 0; i < 200; i++)
            {
                channel1Writer = channel1.Writer.WriteAsync(i);
                await Task.Delay(1);
            }
            channel1.Writer.Complete();
        });

        var channel2Task = Task.Run(async () =>
        {
            for (int i = 0; i < 10000; i++)
            {
                channel2Writer = channel2.Writer.WriteAsync(i);
            }
            channel2.Writer.Complete();
        });

        // Act
        var result = new List<int>();
        await foreach (var item in channels.MergeAsyncExtension())
        {
            result.Add(item);
        }

        // Assert
        Assert.That(result.Count, Is.EqualTo(10200));
    }

    [Test]
    public async Task MergeAsync_OneChannelFails_ThrowsException()
    {
        // Arrange
        var channel1 = Channel.CreateBounded<int>(2);
        var channel2 = Channel.CreateBounded<int>(4);

        var channels = new List<ChannelReader<int>> { channel1.Reader, channel2.Reader };

        var channel1Writer = new ValueTask();
        var channel2Writer = new ValueTask();

        for (int i = 1; i < 3; i++)
        {
            channel1Writer = channel1.Writer.WriteAsync(i);
        }

        for (int i = 10; i < 13; i++)
        {
            channel2Writer = channel2.Writer.WriteAsync(i);
        }

        await channel1Writer;
        await channel2Writer;

        channel1.Writer.Complete();
        channel2.Writer.Complete(new InvalidOperationException("Test error"));

        // Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            // Act
            await foreach (var item in channels.MergeAsyncExtension())
            {
            }
        });

        Assert.That(exception.Message, Is.EqualTo("Test error"));
    }

    [Test]
    public async Task MergeAsync_CancelBeforeStart_ThrowsTaskCanceledException()
    {
        // Arrange
        var channel = Channel.CreateBounded<int>(10);
        var channels = new List<ChannelReader<int>> { channel.Reader };
        using var cancellationTokenSource = new CancellationTokenSource();


        // Act
        cancellationTokenSource.Cancel();

        // Assert
        var exception = Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await foreach (var item in channels.MergeAsyncExtension(mainCancellationToken: cancellationTokenSource.Token))
            {
            }
        });

        Assert.That(exception.CancellationToken, Is.EqualTo(cancellationTokenSource.Token));
    }
}