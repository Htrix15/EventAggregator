using EventAggregator.Shared.Commands.Abstractions;
using EventAggregator.WebApi.Application.Commands.BreakShowAggregation;
using EventAggregator.WebApi.Application.Commands.StartShowAggregation;
using EventAggregator.WebApi.Application.DTOs.Requests;
using EventAggregator.WebApi.Application.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace EventAggregator.WebApi.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class ShowController(ICommandHandler<StartShowAggregationCommand> startShowAggregationCommandHandler,
    ICommandHandler<BreakShowAggregationCommand> breakShowAggregationCommandHandler) : ControllerBase
{

    [HttpPost]
    public async Task<StartShowAggregationResponse> StartShowAggregation([FromBody] StartShowAggregationRequest request, 
        CancellationToken cancellationToken)
    {
        var result = await startShowAggregationCommandHandler.Handle(new StartShowAggregationCommand() 
            { 
                SearchDateRanges = request.SearchDateRanges, 
                ShowTypes = request.ShowTypes
            }, 
            cancellationToken);

        return new StartShowAggregationResponse() { 
            RequestId = result.RequestId
        };
    }

    [HttpGet]
    public async Task<bool> BreakShowAggregation(Guid requestId,
        CancellationToken cancellationToken)
    {
        var result = await breakShowAggregationCommandHandler.Handle(new BreakShowAggregationCommand()
            {
                RequestId = requestId
            },
            cancellationToken);

        return result.IsSuccess;
    }
}
