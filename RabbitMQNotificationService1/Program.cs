using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQNotificationService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("RabbitMQ Notification Service 1");
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {

                channel.QueueDeclare(queue: "Notification1OrderCreation", durable: false, exclusive: false, autoDelete: false, arguments: null);

                channel.ExchangeDeclare(exchange: "OrderCreation", type: ExchangeType.Fanout);
                channel.QueueBind("Notification1OrderCreation", "OrderCreation", "Order");
 
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                    channel.BasicAck(ea.DeliveryTag, false);
                };
                channel.BasicConsume(queue: "Notification1OrderCreation",
                autoAck: false,
                consumer: consumer);
                Console.ReadLine();
            }
        }
    }
}
