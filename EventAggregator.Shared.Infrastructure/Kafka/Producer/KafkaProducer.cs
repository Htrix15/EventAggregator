using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Constants;
using EventAggregator.Shared.MessageBrokers.Enums;
using KafkaFlow.Producers;

namespace EventAggregator.Shared.Infrastructure.Kafka.Producer
{
    public class KafkaProducer<TMessage>(IProducerAccessor producerAccessor) : IProducer<TMessage> where TMessage : IMessage
    {
        public async Task SendMessage(string messageKey, TMessage message, TopicType topic, CancellationToken cancellationToken)
        {
            var producer = producerAccessor.GetProducer<KafkaProducer<TMessage>>();

            await producer.ProduceAsync(Topics.GetTopic(topic), messageKey, message);
        }
    }
}
