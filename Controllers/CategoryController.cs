using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataDriven.Data;
using DataDriven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataDriven.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoryController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Category>>> Get()
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }
        
        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(category);
        }
        
        [HttpPost]
        [Route("")]
        [Authorize(Roles="employee")]
        public async Task<ActionResult<List<Category>>> Post([FromBody]Category model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                _context.Categories.Add(model);
                await _context.SaveChangesAsync();

                return Ok(model);
            }
            catch(Exception e)
            {
                return BadRequest(new {message = $"Não foi possível criar a categoria. Erro {e}"});
            }
        }
        
        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles="employee")]
        public async Task<ActionResult<List<Category>>> Put(int id, [FromBody]Category model)
        {
            if (id != model.Id)
                return NotFound(new {message = "Categoria não encontrada"});

            try
            {
                _context.Entry<Category>(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException e)
            {
                return BadRequest(new {message = $"Este registro já foi alterado. Erro {e}"});
            }
            catch (Exception e)
            {
                return BadRequest(new {message = $"Não foi possível atualizar a categoria. Erro {e}"});
            }
        }
        
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles="employee")]
        public async Task<ActionResult<List<Category>>> Delete(int id)
        {
            var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return BadRequest(new {message = "Categoria não encontrada"});

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return Ok(new {message = "Categoria removida com sucesso"});
            }
            catch(Exception e)
            {
                return BadRequest(new {message = $"Não foi possível deletar a categoria. Erro {e}"});
            }
        }
    }
}