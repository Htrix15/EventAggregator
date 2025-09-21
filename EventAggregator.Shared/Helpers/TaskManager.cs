using EventAggregator.Shared.MessageBrokers.Abstractions;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace EventAggregator.Shared.Helpers;

public class TaskManager(int maxConcurrency = 10) : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(maxConcurrency, maxConcurrency);
    private bool _disposed = false;

    private record TaskWrap(Task Task, CancellationTokenSource CancellationTokenSource) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            try
            {
                CancellationTokenSource.Cancel();

                if (Task is not null && !Task.IsCompleted)
                {
                    try
                    {
                        await Task.ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            finally
            {
                CancellationTokenSource.Dispose();
            }
        }
    }


    private static async Task ActiveTasksFinallizer(ConcurrentDictionary<Guid, TaskWrap> activeTasks)
    {
        if (!activeTasks.IsEmpty)
        {
            foreach (var activeTask in activeTasks)
            {
                if (!activeTask.Value.CancellationTokenSource.IsCancellationRequested)
                {
                    activeTask.Value.CancellationTokenSource.Cancel();
                }
            }

            var tasksToWait = activeTasks.Values.Where(t => !t.Task.IsCompleted).Select(t => t.Task).ToArray();

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                await Task.WhenAll(tasksToWait).WaitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                //TODO Log
            }

            foreach (var activeTask in activeTasks)
            {
                await activeTask.Value.DisposeAsync();
            }

            activeTasks.Clear();
        }
    }

    public async Task Run<TStartActionMessage, TBreakActionMessage>(ChannelReader<TStartActionMessage> actionsChannelReader,
        ChannelReader<TBreakActionMessage> breakersChannelReader,
        Func<TStartActionMessage, CancellationToken, Task> action,
        Func<TBreakActionMessage, CancellationToken, Task>? breakAction,
        CancellationToken cancellationToken)
            where TStartActionMessage : IMessage
            where TBreakActionMessage : IMessage
    {
        ConcurrentDictionary<Guid, TaskWrap> activeTasks = [];

        try
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var mergedChannels = ChannelHelpers.MergeAsync(actionsChannelReader,
                breakersChannelReader,
                mainCancellationToken: cancellationToken);

            await foreach (var message in mergedChannels)
            {

                switch (message)
                {
                    case TStartActionMessage actionMessage:
                        {
                            if (cancellationToken.IsCancellationRequested)
                                continue;

                            if (activeTasks.ContainsKey(actionMessage.RequestId))
                            {
                                continue;
                            }

                            await _semaphore.WaitAsync(cancellationToken);

                            var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                            var task = Task.Run(async () =>
                            {
                                try
                                {
                                    await action(actionMessage, tokenSource.Token);
                                }
                                catch (OperationCanceledException) when (tokenSource.Token.IsCancellationRequested)
                                {
    
                                }
                                catch (Exception)
                                {
                                  //TODO Loging
                                }
                                finally 
                                {
                                    _semaphore.Release();
                                }
                            }, cancellationToken);

                            var taskWrap = new TaskWrap(task, tokenSource);

                            if (!activeTasks.TryAdd(actionMessage.RequestId, taskWrap))
                            {
                                _semaphore.Release();
                                await taskWrap.DisposeAsync();
                                continue;
                            }

                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await task;
                                }
                                finally
                                {
    
                                    if (activeTasks.TryRemove(actionMessage.RequestId, out var completedTask))
                                    {
                                        await completedTask.DisposeAsync();
                                    }
                                }
                            }, CancellationToken.None);
                            break;
                        }                     
                    case TBreakActionMessage breakMessage:
                        {
                            if (activeTasks.TryRemove(breakMessage.RequestId, out var activeTask))
                            {
                                if (breakAction != null)
                                {

                                    var _ = Task.Run(async () =>
                                    {
                                        try
                                        {
                                            await breakAction(breakMessage, cancellationToken);
                                        }
                                        catch  {
                                        //TODO Log
                                        }
                                    }, CancellationToken.None);
                                }
                                await activeTask.DisposeAsync();
                            }
                            break;
                        }
                }
            }
        }
        finally
        {
           await ActiveTasksFinallizer(activeTasks);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _semaphore?.Dispose();
        GC.SuppressFinalize(this);
    }
}
