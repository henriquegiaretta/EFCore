using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataDriven.Data;
using DataDriven.Models;
using DataDriven.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataDriven.Controllers
{
    [Route("users")]
    public class UserController : Controller
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get()
        {
            var users = await _context
                .Users
                .AsNoTracking()
                .ToListAsync();
            return users;
        }
        
        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post([FromBody] User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                model.Role = "employee";
                
                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                model.Password = "";

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(new {message = $"Não foi possível criar o usuário. Erro {e}"});
            }
        }
        
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User model)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(x => x.Username == model.Username && x.Password == model.Password)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new {message = "Usuário não encontrado"});

            var token = TokenService.GenerateToken(user);
            user.Password = "";
            
            return new
            {
                user = user,
                token = token
            };
        }
        
        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(int id, [FromBody]User model)
        {
            // Verifica se os dados são válidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifica se o ID informado é o mesmo do modelo
            if (id != model.Id)
                return NotFound(new { message = "Usuário não encontrada" });

            try
            {
                _context.Entry(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });

            }
        }
    }
}