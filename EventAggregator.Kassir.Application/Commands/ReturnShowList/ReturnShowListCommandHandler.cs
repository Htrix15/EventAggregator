using EventAggregator.Shared.Commands.Abstractions;
using EventAggregator.Shared.Commands.Implementations;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Enums;
using EventAggregator.Shared.ShowEntities.Messages;

namespace EventAggregator.Kassir.Application.Commands.ReturnShowList;

public class ReturnShowListCommandHandler(IProducer<ShowListMessage> producer) : ICommandHandler<ReturnShowListCommand>
{
    public async Task<ICommandResult> Handle(ReturnShowListCommand command, CancellationToken cancellationToken = default)
    {
        var requestId = command.RequestId;
        var message = new ShowListMessage(requestId, DateTime.UtcNow, command.Shows);

        try
        {
            await producer.SendMessage(messageKey: requestId.ToString(), message, TopicType.SearchShowStarted, cancellationToken);
        }
        catch
        {
            return new CommandResult()
            {
                RequestId = requestId,
                IsSuccess = false
            };
        }

        return new CommandResult()
        {
            RequestId = requestId,
            IsSuccess = true
        };
    }
}
