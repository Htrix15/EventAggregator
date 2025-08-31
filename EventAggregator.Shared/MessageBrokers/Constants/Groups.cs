using EventAggregator.Shared.MessageBrokers.Enums;

namespace EventAggregator.Shared.MessageBrokers.Constants;
public class Groups
{
    private readonly static Dictionary<GroupType, string> _topics = new()
    {
        [GroupType.Orchestrator] = "orchestrator"
    };

    public static string GetGroup(GroupType groupType)
    {
        if (_topics.TryGetValue(groupType, out var groupTypeStr))
        {
            return groupTypeStr;
        }
        throw new KeyNotFoundException($"Groups value for '{groupType}' not found in dictionary.");
    }
}
