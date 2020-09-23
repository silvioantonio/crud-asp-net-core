using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.models;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("usuarios")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles="manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext dataContext)
        {
           return await dataContext.Users.AsNoTracking().ToListAsync();
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<User>> Get(int id, [FromServices] DataContext dataContext)
        {
            var user = await dataContext
                                .Users
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Id == id);

            return Ok(user);
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles="manager")]
        public async Task<ActionResult<User>> Update(int id, [FromServices] DataContext dataContext, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return NotFound(new {message = "Usuario nao encontrado"});
            }
            try
            {
                dataContext.Entry(user).State = EntityState.Modified;
                await dataContext.SaveChangesAsync();
                return user;
            }
            catch (Exception)
            {
                return BadRequest(new {message = "Nao foi possivel atualizar o usuario"});
            }
           
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        //[Authorize(Roles="manager")]
        public async Task<ActionResult<User>> Post([FromServices] DataContext dataContext, [FromBody] User user)
        {
            //Força o usuario a ser sempre 'funcionario'
            user.Role = "empoyee";
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try{
                dataContext.Users.Add(user);
                await dataContext.SaveChangesAsync();

                //esconde a senha
                user.Password = "*********";
                return Ok(user);
            }catch(Exception){
                return BadRequest(new {message = "Nao foi possivel criar o usuario"});
            }

        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromServices] DataContext dataContext, [FromBody] User user)
        {

            var usuario = await dataContext.Users.AsNoTracking().Where(x => x.UserName == user.UserName && x.Password == user.Password).FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound(new {message = "Usurio ou senha invalidos"});
            
            var token = TokenService.GenerateToken(usuario);
            //esconde a senha
                usuario.Password = "*********";
            return new {user = usuario, token = token};

        }

    }
}