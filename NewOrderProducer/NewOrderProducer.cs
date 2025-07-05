using BuildingBlocks.Dispatcher;
using BuildingBlocks.Helpers;
using BuildingBlocks.Models;
using Confluent.Kafka;

namespace NewOrder;

public class NewOrderProducer
{
    public async Task ProduceNewOrder()
    {
        using (var dispatcher = new KafkaDispatcher<string, string>(Serializers.Utf8, Serializers.Utf8))
        {
            var new_order_topic = "ECOMMERCE_NEW_ORDER";
            var send_email_topic = "ECOMMERCE_SEND_EMAIL";

            var orderId = Guid.NewGuid().ToString();
            var orderValue = $"{orderId},67523,1234";
            var emailValue = $"Thank you for your order {orderId}! We are processing your order!";
            await dispatcher.SendAsync(new_order_topic, orderId, orderValue);

            await dispatcher.SendAsync(send_email_topic, orderId, emailValue);

            Console.WriteLine("Todas as mensagens foram enviadas ou estão em processo de envio.");
        }
    }

    public async Task ProduceMultipleNewOrders(int numberOfOrders)
    {
        using (var orderDispatcher = new KafkaDispatcher<string, Order>(Serializers.Utf8, new JsonSerializer<Order>()))
        using (var emailDispatcher = new KafkaDispatcher<string, string>(Serializers.Utf8, Serializers.Utf8))
        {
            var new_order_topic = "ECOMMERCE_NEW_ORDER";
            var send_email_topic = "ECOMMERCE_SEND_EMAIL";

            for (int i = 0; i < numberOfOrders; i++)
            {
                var orderId = Guid.NewGuid().ToString();
                var userEmail = $"user_{i + 1}_{Guid.NewGuid().ToString().Substring(0, 4)}@example.com";
                var amount = new decimal(new Random().NextDouble() * 5000 + 1);

                var order = new Order(userEmail, orderId, amount);

                Console.WriteLine($"Produzindo order {i + 1}/{numberOfOrders} para a ordem {orderId}...");
                await orderDispatcher.SendAsync(new_order_topic, userEmail, order);

                var emailValue = $"Thank you for your order {orderId}! We are processing your order!";

                Console.WriteLine($"Produzindo email {i + 1}/{numberOfOrders} para a ordem {orderId}...");
                await emailDispatcher.SendAsync(send_email_topic, userEmail, emailValue);

                await Task.Delay(50);
            }

            Console.WriteLine($"Concluído o envio de {numberOfOrders} novas ordens.");
        }
    }
}
