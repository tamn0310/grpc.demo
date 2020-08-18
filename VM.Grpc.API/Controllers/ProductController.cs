using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using VM.Grpc.API.Constants;
using VM.Grpc.Server.Protos;

namespace VM.Grpc.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly GrpcChannel channel;

        public ProductController()
        {
            channel = GrpcChannel.ForAddress(UrlConstants.BaseUrl);
        }

        [HttpGet("products")]
        public List<Product> GetAll()
        {
            var client = new ProductService.ProductServiceClient(channel);
            return client.GetAll(new Empty()).Products.ToList();
        }

        [HttpGet("products/{id}", Name = "GetProduct")]
        public IActionResult GetById(int id)
        {
            var client = new ProductService.ProductServiceClient(channel);
            var product = client.Get(new ProductId { Id = id });
            if (product == null)
            {
                return NotFound();
            }
            //return new ObjectResult(product);
            return Ok(product);
        }

        [HttpPost("products")]
        public IActionResult Post([FromBody] Product product)
        {
            var client = new ProductService.ProductServiceClient(channel);
            var createdProduct = client.Insert(product);

            //return CreatedAtRoute("GetProduct", new { id = createdProduct.Id }, createdProduct);
            return Created("products", createdProduct);
        }

        [HttpPut("products/{id:int}")]
        public IActionResult Put([FromBody] Product product)
        {
            var client = new ProductService.ProductServiceClient(channel);
            var udpatedProduct = client.Update(product);
            if (udpatedProduct == null)
            {
                return NotFound();
            }
            return Ok(udpatedProduct);
        }

        [HttpDelete("products/{id}")]
        public IActionResult Delete(int id)
        {
            var client = new ProductService.ProductServiceClient(channel);
            client.Delete(new ProductId { Id = id });
            //return new ObjectResult(id);
            return Ok(id);
        }
    }
}