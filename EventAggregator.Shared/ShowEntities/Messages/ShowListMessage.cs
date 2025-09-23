using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.ShowEntities.Models;

namespace EventAggregator.Shared.ShowEntities.Messages;

public record ShowListMessage(Guid RequestId, DateTime RequestDateTime, List<Show> Shows): MessageBase(RequestId, RequestDateTime);
