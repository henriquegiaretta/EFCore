using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataDriven.Data;
using DataDriven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataDriven.Controllers
{
    [Route("products")]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get()
        {
            var products = await _context.Products
                .Include(x => x.Category)
                .AsNoTracking()
                .ToListAsync();

            return Ok(products);
        }
        
        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Products
                .Include(x => x.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            
            return Ok(product);
        }
        
        [HttpGet]
        [Route("categories/{id:int}")]
        public async Task<ActionResult<Product>> GetByCategory(int id)
        {
            var products = await _context.Products
                .Include(x => x.Category)
                .AsNoTracking()
                .Where(x => x.Category.Id == id)
                .ToListAsync();

            return Ok(products);
        }
        
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<List<Product>>> Post([FromBody]Product model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                _context.Products.Add(model);
                await _context.SaveChangesAsync();

                return Ok(model);
            }
            catch(Exception e)
            {
                return BadRequest(new {message = $"Não foi possível criar o produto. Erro {e}"});
            }
        }
    }
}