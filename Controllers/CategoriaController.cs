
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.models;

[Route("categorias")]
public class CategoriaController : ControllerBase{

    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Category>>> Get( [FromServices] DataContext dataContext)
    {
        // O AsNoTracking faz com que as chamadas nao armazenem informaçoes adicionais na requisiçao(Ja que sao GET)
        var categories = await dataContext.Categories.AsNoTracking().ToListAsync();
        return Ok(categories);
    }

    [HttpGet]
    [Route("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Category>> Get(int id, [FromServices] DataContext dataContext)
    {
        var category = await dataContext.Categories.AsNoTracking().FirstOrDefaultAsync();
        return Ok(category);
    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles="employee")]
    public async Task<ActionResult<Category>> Post([FromBody] Category category, [FromServices] DataContext dataContext){
        if (ModelState.IsValid)
        {
            try
            {
                dataContext.Categories.Add(category);

                //Salvar mudanças de forma assincrona
                await dataContext.SaveChangesAsync();

                return Ok(category);
            }
            catch
            {
                return BadRequest(new {message = "Nao foi possivel criar a categoria"});
            }
            
        }
        return BadRequest(ModelState);
    }

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles="employee")]
    public async Task<ActionResult<Category>> Update(int id, [FromBody] Category category, [FromServices] DataContext dataContext){
        
        if (category.Id != id)
            return NotFound(new {message = "Categoria nao encontrada"});

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            dataContext.Entry<Category>(category).State = EntityState.Modified;
            await dataContext.SaveChangesAsync();
            return Ok(category);
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new {message = "Este registro ja foi atualizado"});
        }
        catch(Exception)
        {
            return BadRequest(new {message = "Nao foi possivel atualizar o registro"});
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles="employee")]
    public async Task<ActionResult<List<Category>>> Delete(int id, [FromServices] DataContext dataContext){
        
        var category = await dataContext.Categories.FirstOrDefaultAsync(x => x.Id == id);

        if (category == null)
        {
            return NotFound(new {message = "Categoria nao encontrada"});
        }

        try
        {
            dataContext.Categories.Remove(category);
            await dataContext.SaveChangesAsync();
            return Ok(new {message = "Categoria removida com sucesso"});
        }
        catch(Exception)
        {
            return BadRequest(new {message = "Nao foi possivel remover o registro"});
        }
    }

}