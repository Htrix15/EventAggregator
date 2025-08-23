namespace EventAggregator.WebApi.Application.DTOs.Responses;

public record StartShowAggregationResponse
{
    public Guid RequestId { get; set; }
}
