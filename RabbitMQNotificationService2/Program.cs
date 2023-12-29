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
            Console.WriteLine("RabbitMQ Notification Service 2");
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {

                channel.QueueDeclare(queue: "Notification2OrderCreation", durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueDeclare(queue: "Notification2OrderUpdation", durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.ExchangeDeclare(exchange: "OrderUpdation", type: ExchangeType.Topic);
                channel.ExchangeDeclare(exchange: "OrderCreation", type: ExchangeType.Fanout);
 
                channel.QueueBind("Notification2OrderCreation", "OrderCreation", "Order2");
                channel.QueueBind("Notification2OrderUpdation", "OrderUpdation", "OrderUpdation.5");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                    channel.BasicAck(ea.DeliveryTag, false);
                };
                channel.BasicConsume(queue: "Notification2OrderCreation",
                autoAck: false,
                consumer: consumer);
                channel.BasicConsume(queue: "Notification2OrderUpdation",
                autoAck: false,
                consumer: consumer);
                Console.ReadLine();
            }


        }
    }
}
