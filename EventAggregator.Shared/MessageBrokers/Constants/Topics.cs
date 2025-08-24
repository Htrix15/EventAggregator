using EventAggregator.Shared.MessageBrokers.Enums;

namespace EventAggregator.Shared.MessageBrokers.Constants;

public class Topics
{
    private readonly static Dictionary<TopicType, string> _topics = new()
    {
        [TopicType.StartShowAggregation] = "start-show-aggregation"
    };

    public static string GetTopic(TopicType topic)
    {
        if (_topics.TryGetValue(topic, out var topicStr))
        {
            return topicStr;
        }
        throw new KeyNotFoundException($"Topic value for '{topic}' not found in dictionary.");
    }
}
