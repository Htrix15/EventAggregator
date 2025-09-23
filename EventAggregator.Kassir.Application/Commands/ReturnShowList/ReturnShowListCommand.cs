using EventAggregator.Shared.Commands.Abstractions;
using EventAggregator.Shared.ShowEntities.Models;

namespace EventAggregator.Kassir.Application.Commands.ReturnShowList;

public record ReturnShowListCommand(Guid RequestId, List<Show> Shows) : ICommand;