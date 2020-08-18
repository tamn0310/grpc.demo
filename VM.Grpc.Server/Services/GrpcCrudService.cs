using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VM.Grpc.Server.Protos;

namespace VM.Grpc.Server.Services
{
    public class GrpcCrudService : ProductService.ProductServiceBase
    {
        private readonly List<Product> _products = new List<Product>();
        private int idCount = 0;
        private readonly ILogger<GrpcCrudService> _logger;

        public GrpcCrudService(ILogger<GrpcCrudService> logger)
        {
            _logger = logger;
            _products.Add(new Product()
            {
                Id = idCount++,
                Name = "Farm Flour",
                Description = "this is product",
                Content = "Bill's Corn product",
            });
        }

        public override Task<ProductList> GetAll(Empty empty, ServerCallContext context)
        {
            ProductList productList = new ProductList();
            productList.Products.AddRange(_products);
            return Task.FromResult(productList);
        }

        public override Task<Product> Get(ProductId productId, ServerCallContext context)
        {
            return Task.FromResult( //
                (from p in _products where p.Id == productId.Id select p).FirstOrDefault());
        }

        public override Task<Product> Insert(Product product, ServerCallContext context)
        {
            product.Id = idCount++;
            _products.Add(product);
            return Task.FromResult(product);
        }

        public override Task<Product> Update(Product product, ServerCallContext context)
        {
            var productToUpdate = (from p in _products where p.Id == product.Id select p).FirstOrDefault();
            if (productToUpdate != null)
            {
                productToUpdate.Name = product.Name;
                productToUpdate.Description = product.Description;
                productToUpdate.Content = product.Content;
                return Task.FromResult(product);
            }
            return Task.FromException<Product>(new EntryPointNotFoundException());
        }

        public override Task<Empty> Delete(ProductId productId, ServerCallContext context)
        {
            var productToDelete = (from p in _products where p.Id == productId.Id select p).FirstOrDefault();
            if (productToDelete == null)
            {
                return Task.FromException<Empty>(new EntryPointNotFoundException());
            }
            _products.Remove(productToDelete);
            return Task.FromResult(new Empty());
        }
    }
}