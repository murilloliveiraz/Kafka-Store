using Confluent.Kafka;

namespace BuildingBlocks.Interfaces
{
    public interface IConsumerFunction<TKey, TValue>
    {
        void Consume(ConsumeResult<TKey, TValue> record);
    }
}
