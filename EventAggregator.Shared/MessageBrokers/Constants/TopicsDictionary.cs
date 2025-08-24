using EventAggregator.Shared.MessageBrokers.Enums;

namespace EventAggregator.Shared.MessageBrokers.Constants;

public class TopicsDictionary
{
    private readonly static Dictionary<TopicType, string> _topicsLibrary = new()
    {
        [TopicType.StartShowAggregation] = "start-show-aggregation"
    };

    public static string GetTopic(TopicType topic)
    {
        if (_topicsLibrary.TryGetValue(topic, out var topicStr))
        {
            return topicStr;
        }
        throw new KeyNotFoundException($"Topic value for '{topic}' not found in dictionary.");
    }
}
