using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace gRPCOrderService
{
    public class Producer
    {
        public void StartProducer(string OrderNo,string cartId)
        {
            string topicName = "Order";
            //Define the configuration
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9093,localhost:9094,localhost:9095",
                ClientId = Dns.GetHostName(),
                Acks = Acks.All,
                MessageSendMaxRetries = 0,
                BatchSize = 16384,
                LingerMs = 1,
                QueueBufferingMaxKbytes = 33554432,

            };

            // create the producer
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                    producer.Produce(topicName,
                        new Message<string, string>
                        {
                            Key = "CartId : " + cartId,
                            Value = "Order No. : " + OrderNo +" is placed for cart Id : "+cartId
                        }
                        );
                    Console.WriteLine("Message sent successfully ");
                // wait for up to 10 seconds for any inflight messages to be delivered.
                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}
