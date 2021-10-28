using System.Collections.Generic;
using CarvedRock.Api.ApiModels;
using CarvedRock.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;
namespace CarvedRock.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductLogic _productLogic;

        public ProductsController(IProductLogic productLogic)
        {
            _productLogic = productLogic;
        }

        [HttpGet]
        public IEnumerable<Product> GetProducts(string category = "all")
        {
            //Log.Information("Starting controller action GetProducts for {category}", category); //remove ILogger and use serilog
            Log.ForContext("Category", category)
                .Information("Starting controller action GetProducts"); //use the seq extension of serilog to better format your logging entries
            return _productLogic.GetProductsForCategory(category);
        }
    }
}