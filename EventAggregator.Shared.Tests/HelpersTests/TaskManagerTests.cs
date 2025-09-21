using EventAggregator.Shared.Helpers;
using EventAggregator.Shared.ShowEntities.Messages;
using System.Threading.Channels;

namespace EventAggregator.Shared.Tests.HelpersTests;

[TestFixture]
internal class TaskManagerTests
{
    [Test]
    public async Task Run_OnlyActionMessages_MessagesProcessed()
    {
        // Arrange
        var channel1 = Channel.CreateBounded<StartShowAggregationMessage>(2);
        var channel2 = Channel.CreateBounded<BreakShowAggregationMessage>(4);

        var requestId1 = Guid.NewGuid();
        var requestId2 = Guid.NewGuid();

        List<StartShowAggregationMessage> startMessages = [
            new StartShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = requestId1,
                SearchDateRanges = [],
                ShowTypes = []
            },
            new StartShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = requestId2,
                SearchDateRanges = [],
                ShowTypes = []
            }];

        List<Guid> processedGuids = [];
        var tokenSource = new CancellationTokenSource();
        Func<StartShowAggregationMessage, CancellationToken, Task> action =
            async (StartShowAggregationMessage m, CancellationToken t) =>
            {
                await Task.Delay(1000, t);
                processedGuids.Add(m.RequestId);
            };

        // Act
        using var taskManager = new TaskManager();
        var managerTask = taskManager.Run(channel1.Reader, channel2.Reader, action, null, tokenSource.Token);

        var startMessagesTask = Task.Run(async () =>
        {
            foreach (var item in startMessages)
            {
                await channel1.Writer.WriteAsync(item);
                await Task.Delay(500);
            }
            await Task.Delay(500);
            channel1.Writer.Complete();
        });

        channel2.Writer.Complete();

        await managerTask;
        await startMessagesTask;

        // Assert
        var expected = new[] { requestId1, requestId2 };
        Assert.That(expected, Is.SubsetOf(processedGuids));
    }

    [Test]
    public async Task Run_ActionMessagesAndBreakMessage_ProcessedOnlyNotBreakMessage()
    {
        // Arrange
        var channel1 = Channel.CreateBounded<StartShowAggregationMessage>(2);
        var channel2 = Channel.CreateBounded<BreakShowAggregationMessage>(4);

        var requestId1 = Guid.NewGuid();
        var requestId2 = Guid.NewGuid();

        List<StartShowAggregationMessage> startMessages = [
            new StartShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = requestId1,
                SearchDateRanges = [],
                ShowTypes = []
            },
            new StartShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = requestId2,
                SearchDateRanges = [],
                ShowTypes = []
            }];

        List<BreakShowAggregationMessage> breakMessages = [
            new BreakShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = Guid.NewGuid(),
            },
            new BreakShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = requestId2,
            }
        ];

        List<Guid> processedGuids = [];
        var tokenSource = new CancellationTokenSource();
        Func<StartShowAggregationMessage, CancellationToken, Task> action =
            async (StartShowAggregationMessage m, CancellationToken t) =>
            {
                await Task.Delay(400, t);

                if (t.IsCancellationRequested) {
                    return;
                }
                processedGuids.Add(m.RequestId);
            };

        // Act
        using var taskManager = new TaskManager();
        var managerTask = taskManager.Run(channel1.Reader, channel2.Reader, action, null, tokenSource.Token);

        var startMessagesTask = Task.Run(async () =>
        {
            foreach (var item in startMessages)
            {
                await channel1.Writer.WriteAsync(item);
                await Task.Delay(500);
            }
            await Task.Delay(500);
            channel1.Writer.Complete();
        });

        var breakMessagesTask = Task.Run(async () =>
        {
            foreach (var item in breakMessages)
            {
                await channel2.Writer.WriteAsync(item);
                await Task.Delay(500);
            }
            await Task.Delay(500);
            channel2.Writer.Complete();
        });

        await managerTask;
        await startMessagesTask;
        await breakMessagesTask;

        // Assert
        var expected = new[] { requestId1 };
        Assert.That(expected, Is.SubsetOf(processedGuids));
    }


    [Test]
    public async Task Run_ActionMessagesAndBreakMessageAndBreakAction_ProcessedNotBreakMessageProcessedBreakAction()
    {
        // Arrange
        var channel1 = Channel.CreateBounded<StartShowAggregationMessage>(2);
        var channel2 = Channel.CreateBounded<BreakShowAggregationMessage>(4);

        var requestId1 = Guid.NewGuid();
        var requestId2 = Guid.NewGuid();

        List<StartShowAggregationMessage> startMessages = [
            new StartShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = requestId1,
                SearchDateRanges = [],
                ShowTypes = []
            },
            new StartShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = requestId2,
                SearchDateRanges = [],
                ShowTypes = []
            }];

        List<BreakShowAggregationMessage> breakMessages = [
            new BreakShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = Guid.NewGuid(),
            },
            new BreakShowAggregationMessage()
            {
                RequestDateTime = DateTime.Now,
                RequestId = requestId2,
            }
        ];

        List<Guid> processedGuids = [];
        var tokenSource = new CancellationTokenSource();
        Func<StartShowAggregationMessage, CancellationToken, Task> action =
            async (StartShowAggregationMessage m, CancellationToken t) =>
            {
                await Task.Delay(400, t);

                if (t.IsCancellationRequested)
                {
                    return;
                }
                processedGuids.Add(m.RequestId);
            };

        List<Guid> cancelActionGuids = [];
        Func<BreakShowAggregationMessage, CancellationToken, Task> breakAction =
          async (BreakShowAggregationMessage m, CancellationToken t) =>
          {
              cancelActionGuids.Add(m.RequestId);
          };

        // Act
        using var taskManager = new TaskManager();
        var managerTask = taskManager.Run(channel1.Reader, channel2.Reader, action, breakAction, tokenSource.Token);

        var startMessagesTask = Task.Run(async () =>
        {
            foreach (var item in startMessages)
            {
                await channel1.Writer.WriteAsync(item);
                await Task.Delay(500);
            }
            await Task.Delay(500);
            channel1.Writer.Complete();
        });

        var breakMessagesTask = Task.Run(async () =>
        {
            foreach (var item in breakMessages)
            {
                await channel2.Writer.WriteAsync(item);
                await Task.Delay(500);
            }
            await Task.Delay(500);
            channel2.Writer.Complete();
        });

        await managerTask;
        await startMessagesTask;
        await breakMessagesTask;

        // Assert
        var expectedNotCanceledMessage = new[] { requestId1 };
        Assert.That(expectedNotCanceledMessage, Is.SubsetOf(processedGuids));

        var expectedCanceledMessage = new[] { requestId2 };
        Assert.That(expectedCanceledMessage, Is.SubsetOf(cancelActionGuids));
    }
}
