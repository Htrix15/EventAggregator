using EventAggregator.Kassir.Domain.Services;
using EventAggregator.Shared.Helpers;
using EventAggregator.Shared.ShowEntities.Messages;
using Microsoft.Extensions.Hosting;
using System.Threading.Channels;

namespace EventAggregator.Kassir.Infrastructure.BackgroundServices;

public class MessageManagerBackgroundService(Channel<StartShowAggregationMessage> actionsChannel,
    Channel<BreakShowAggregationMessage> breakersChannel,
    IMessageManagerService messageManagerService) : BackgroundService
{
    private readonly TaskManager _taskManager = new();
    private readonly ChannelReader<StartShowAggregationMessage> _actionsChannelReader = actionsChannel.Reader;
    private readonly ChannelReader<BreakShowAggregationMessage> _breakersChannelReader = breakersChannel.Reader;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Yield();

        await _taskManager.Run(_actionsChannelReader, _breakersChannelReader,
            action: messageManagerService.GetActionHandler(), 
            breakAction: null, 
            cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _taskManager.Dispose();
    }
}
