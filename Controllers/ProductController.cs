using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.models;

namespace Shop.Controllers
{
    [Route("produtos")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext dataContext)
        {
            // Esse 'Include(p => p.Category)' serve para mostrar a categoria junto, sem ele, apenas o id da categoria seria mostrado
           return await dataContext.Products.Include(p => p.Category).AsNoTracking().ToListAsync();
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> Get(int id, [FromServices] DataContext dataContext)
        {
            var product = await dataContext
                                .Products
                                .Include(p => p.Category)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Id == id);

            return Ok(product);
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(int id, [FromServices] DataContext dataContext)
        {
            // Como a Categoria foi incluida com o include(), entao posso filtrar com where se a categoria dele é igual a passada por parametro
            var products = await dataContext
                                .Products
                                .Include(p => p.Category)
                                .AsNoTracking()
                                .Where(x => x.CategoryId == id)
                                .ToListAsync();

            return Ok(products);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles="manager")]
        public async Task<ActionResult<Product>> Post([FromServices] DataContext dataContext, [FromBody] Product product)
        {
            if (ModelState.IsValid)
            {
                dataContext.Products.Add(product);
                await dataContext.SaveChangesAsync();
                return Ok(product);
            }

            return BadRequest(ModelState);
        }

    }
}