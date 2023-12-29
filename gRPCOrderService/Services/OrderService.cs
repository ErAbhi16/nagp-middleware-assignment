using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace gRPCOrderService
{
    public class OrderService : Order_Service.Order_ServiceBase
    {
        private readonly ILogger<OrderService> _logger;
        public OrderService(ILogger<OrderService> logger)
        {
            _logger = logger;
        }

        public override Task<OrderResponse> PlaceOrder(OrderCreateRequest request, ServerCallContext context)
        {
            var orderNo = (Convert.ToInt64(request.CartId) * 4).ToString();
            
            CreateOrderRabbitMQNotification(orderNo);
            return Task.FromResult(new OrderResponse
            {
                Message = $"Order No. {orderNo} is created successfully ",
                OrderId = orderNo
            });
        }

        public override Task<OrderResponse> UpdateOrder(OrderUpdateRequest request, ServerCallContext context)
        {
            
            UpdateOrderRabbitMQNotification(request.OrderId);
            return Task.FromResult(new OrderResponse
            {
                Message = $"Order No. {request.OrderId} is updated successfully "
            });
        }

        private static void CreateOrderRabbitMQNotification(string orderno)
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "OrderCreation", type: ExchangeType.Fanout);

                string orderPlacedMessage = $"Order No. {orderno} placed successfully ";
                byte[] body = Encoding.UTF8.GetBytes(orderPlacedMessage);

                channel.BasicPublish(exchange: "OrderCreation", routingKey: "OrderCreation.#", basicProperties: null, body: body);

            }
        }


        private static void UpdateOrderRabbitMQNotification(string orderno)
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {

                channel.ExchangeDeclare(exchange: "OrderUpdation", type: ExchangeType.Topic);

                string orderPlacedMessage = $"Order No. {orderno} updated successfully ";
                byte[] body = Encoding.UTF8.GetBytes(orderPlacedMessage);

                channel.BasicPublish(exchange: "OrderUpdation", routingKey: "OrderUpdation.5", basicProperties: null, body: body);

            }
        }
    }
}
