namespace EventAggregator.Shared.Infrastructure.Kafka.Mapping;

public static class AutoOffsetResetMapping
{
    public static Confluent.Kafka.AutoOffsetReset Map(MessageBrokers.Enums.AutoOffsetReset autoOffsetReset)
         => autoOffsetReset switch
         {
             MessageBrokers.Enums.AutoOffsetReset.Latest => Confluent.Kafka.AutoOffsetReset.Latest,
             MessageBrokers.Enums.AutoOffsetReset.Earliest => Confluent.Kafka.AutoOffsetReset.Earliest,
             MessageBrokers.Enums.AutoOffsetReset.Error => Confluent.Kafka.AutoOffsetReset.Error,
             _ => throw new NotImplementedException(),
         };
}
