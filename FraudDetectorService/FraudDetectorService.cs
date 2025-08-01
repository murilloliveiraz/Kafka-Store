﻿using BuildingBlocks.Helpers;
using BuildingBlocks.Interfaces;
using BuildingBlocks.Models;
using BuildingBlocks.Services;
using Confluent.Kafka;

namespace Producer
{
    public class FraudDetectorService : IConsumerFunction<string, Order>
    {
        public void Consume(ConsumeResult<string, Order> record)
        {
            Console.WriteLine("---------------------");
            Console.WriteLine("Processando nova ordem, verificando fraude");
            Console.WriteLine($"Key (UserId): {record.Message.Key}");
            Console.WriteLine($"Value (Order): {record.Message.Value}");
            Console.WriteLine($"  Order ID: {record.Message.Value.OrderId}");
            Console.WriteLine($"  Amount: {record.Message.Value.Amount}");
            Console.WriteLine($"Partição: {record.Partition.Value}");
            Console.WriteLine($"Offset: {record.Offset.Value}");
            Console.WriteLine($"Timestamp: {record.Message.Timestamp.UtcDateTime}");
            
            try
            {
                Thread.Sleep(5000);
            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine($"FraudDetectorService: Interrupção durante o sleep: {e.Message}");
            }

            Console.WriteLine("FraudDetectorService: Order processed (fraud check complete)");
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            using (var kafkaService = new KafkaService<string, Order>(
                "FraudDetectorService",
                "ECOMMERCE_NEW_ORDER",
                this,
                Deserializers.Utf8,
                new JsonDeserializer<Order>()
                ))
            {
                await kafkaService.Run(cancellationToken);
            }
        }
    }
}
