using EventAggregator.WebApi.Application.DTOs.Requests;
using EventAggregator.WebApi.Application.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace EventAggregator.WebApi.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class EventController() : ControllerBase
{

    [HttpPost]
    public async Task<StartEventAggregationResponse> StartEventAggregation([FromBody] StartEventAggregationRequest request)
    {
        return new StartEventAggregationResponse() { RequestId = Guid.NewGuid() };
    }
}
