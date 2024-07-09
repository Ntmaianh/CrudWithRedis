using DemoRedis.API.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Redis.API.Service;
using Redis.DAL;
using Redis.Model.Models;

namespace Dedis.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IRepository _repo;
        private readonly ICacheService _cacheService;

        public ProductsController(IRepository repo, ICacheService cacheService)
        {
            _repo = repo;
            _cacheService = cacheService;
        }
      
        [Cache(100)] // thuộc tính tự tạo để lưu cache vào redis 
        [HttpGet("getall")]
        public  ActionResult Get()
        {
            var lstProduct = _repo.GetProducts();
            return Ok(lstProduct);
        }
          [Cache(100)] // thuộc tính tự tạo để lưu cache vào redis 
        [HttpGet("getById")]
        public  ActionResult GetById(Guid Id)
        {
            var Product = _repo.GetProductById(Id);
            return Ok(Product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var cacheGetAll = "/Products/getall";
            await _cacheService.RemoveCacheAsync(cacheGetAll.ToLower());

            _repo.Create(product);  
            return Ok();

        }

         [HttpPut]
        public async Task<IActionResult> Update(Product product)
        {
            var cache = "/Products/getall";
            await _cacheService.RemoveCacheAsync(cache.ToLower());
            var cacheDetail = $"/Products/getById?Id={product.Id}";
            await _cacheService.RemoveCacheAsync(cacheDetail.ToLower());

            _repo.Update(product);  
            return Ok();

        }

        [HttpDelete]
        public async Task<IActionResult> Update(Guid id)
        {
            var cache = "/Products/getall";
            await _cacheService.RemoveCacheAsync(cache.ToLower());
            var cacheDetail = $"/Products/getById?Id={id}";
            await _cacheService.RemoveCacheAsync(cacheDetail.ToLower());
            _repo.Delete(id);
            return Ok();

        }

    }
}
