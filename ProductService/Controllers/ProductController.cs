using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using gRPCOrderService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {

        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

     
        /// <summary>
        /// Place order
        /// </summary>
        /// <returns></returns>
        [Route("PlaceOrder")]
        [HttpPost]
        public async Task<string> Post(int cartId)
        {

            using var channel2 = GrpcChannel.ForAddress("https://localhost:5002");
            var client2 = new Order_Service.Order_ServiceClient(channel2);
            var reply2 = await client2.PlaceOrderAsync(new OrderCreateRequest { CartId = cartId.ToString() , ProductId = 12 , Quantity = 1 });
            return $"{reply2}";
        }

        /// <summary>
        /// Update order
        /// </summary>
        /// <returns></returns>
        [Route("UpdateOrder")]
        [HttpPost]
        public async Task<string> Post(int orderId, int productCount)
        {

            using var channel2 = GrpcChannel.ForAddress("https://localhost:5002");
            var client2 = new Order_Service.Order_ServiceClient(channel2);
            var reply2 = await client2.UpdateOrderAsync(new OrderUpdateRequest { OrderId = orderId.ToString() , ProductId = 12 , Quantity = 3  });
            return $"{reply2}";
        }
    }
}
