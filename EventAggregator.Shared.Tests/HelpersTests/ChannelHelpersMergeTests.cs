using EventAggregator.Shared.Helpers;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.ShowEntities.Messages;
using System.Threading.Channels;

namespace EventAggregator.Shared.Tests.HelpersTests;

[TestFixture]
internal class ChannelHelpersMergeTests
{
    [Test]
    public async Task MergeAsync_EmptyChannels_CompletesImmediately()
    {
        // Arrange
        var channel1 = Channel.CreateBounded<StartShowAggregationMessage>(2);
        var channel2 = Channel.CreateBounded<BreakShowAggregationMessage>(4);

        channel1.Writer.Complete();
        channel2.Writer.Complete();

        // Act
        var result = new List<IMessage>();
        await foreach (var item in ChannelHelpers.MergeAsync(channel1.Reader, channel2.Reader))
        {
            result.Add(item);
        }

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task MergeAsync_OneChannelFails_ThrowsException()
    {
        // Arrange
        var channel1 = Channel.CreateBounded<StartShowAggregationMessage>(2);
        var channel2 = Channel.CreateBounded<BreakShowAggregationMessage>(4);

        var channel1Writer = new ValueTask();
        var channel2Writer = new ValueTask();

        for (int i = 0; i < 2; i++)
        {
            channel1Writer = channel1.Writer.WriteAsync(new StartShowAggregationMessage() {
                RequestDateTime = DateTime.Now,
                RequestId = Guid.NewGuid(),
                SearchDateRanges = [],
                ShowTypes = []
            });
        }

        for (int i = 0; i < 4; i++)
        {
            channel2Writer = channel2.Writer.WriteAsync(new BreakShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = Guid.NewGuid(),
            });
        }

        await channel1Writer;
        await channel2Writer;

        channel1.Writer.Complete();
        channel2.Writer.Complete(new InvalidOperationException("Test error"));

        var mergedReaders = ChannelHelpers.MergeAsync(channel1.Reader, channel2.Reader);

        // Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            // Act
            await foreach (var item in mergedReaders)
            {
            }
        });

        Assert.That(exception.Message, Is.EqualTo("Test error"));
    }

    [Test]
    public async Task MergeAsync_MultipleChannels_MergesAllItems()
    {
        // Arrange
        var channel1 = Channel.CreateBounded<StartShowAggregationMessage>(2);
        var channel2 = Channel.CreateBounded<BreakShowAggregationMessage>(4);

        var channel1Writer = new ValueTask();
        var channel2Writer = new ValueTask();

        List<StartShowAggregationMessage> startMessages = [
            new StartShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = Guid.NewGuid(),
                SearchDateRanges = [],
                ShowTypes = []
            },
            new StartShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = Guid.NewGuid(),
                SearchDateRanges = [],
                ShowTypes = []
            }];

        foreach (var item in startMessages) {
            channel1Writer = channel1.Writer.WriteAsync(item);
        }

        List<BreakShowAggregationMessage> breakMessages = [
            new BreakShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = Guid.NewGuid(),
            },
            new BreakShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = Guid.NewGuid(),
            },
            new BreakShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = Guid.NewGuid(),
            },
            new BreakShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = Guid.NewGuid(),
            }];

        foreach (var item in breakMessages)
        {
            channel2Writer = channel2.Writer.WriteAsync(item);
        }

        await channel1Writer;
        await channel2Writer;

        channel1.Writer.Complete();
        channel2.Writer.Complete();

        var mergedReaders = ChannelHelpers.MergeAsync(channel1.Reader, channel2.Reader);

        // Assert
        var result = new List<IMessage>();
        await foreach (var item in mergedReaders)
        {
            result.Add(item);
        }

        Assert.Multiple(() =>
        {
            Assert.That(result.Where(r => r is StartShowAggregationMessage).ToList(), Has.Count.EqualTo(startMessages.Count));
            Assert.That(result.Where(r => r is BreakShowAggregationMessage).ToList(), Has.Count.EqualTo(breakMessages.Count));
        });
    }

    [Test]
    public async Task MergeAsync_CancelBeforeStart_ThrowsTaskCanceledException()
    {
        // Arrange
        var channel1 = Channel.CreateBounded<StartShowAggregationMessage>(2);
        var channel2 = Channel.CreateBounded<BreakShowAggregationMessage>(4);

        using var cancellationTokenSource = new CancellationTokenSource();


        // Act
        cancellationTokenSource.Cancel();
        var mergedReaders = ChannelHelpers.MergeAsync(channel1.Reader, channel2.Reader, mainCancellationToken: cancellationTokenSource.Token);

        // Assert
        var exception = Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await foreach (var item in mergedReaders)
            {
            }
        });

        Assert.That(exception.CancellationToken, Is.EqualTo(cancellationTokenSource.Token));
    }
}
