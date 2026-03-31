using mf_apis_web_services_fuel_manager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mf_apis_web_services_fuel_manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var model = await _context.Usuarios.ToListAsync();

            foreach (var item in model)
            {
                GerarLinks(item);
            }

            return Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(UsuarioDto model)
        {
            var usuario = new Usuario
            {
                Nome = model.Nome,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Perfil = model.Perfil
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            GerarLinks(usuario);

            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var model = await _context.Usuarios
                .FirstOrDefaultAsync(c => c.Id == id);
            if (model == null)
                return NotFound();
            GerarLinks(model);
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UsuarioDto model)
        {
            if (id != model.Id) return BadRequest();

            var modeloDb = await _context.Usuarios
                .FirstOrDefaultAsync(c => c.Id == id);

            if (modeloDb == null) return NotFound();

            modeloDb.Nome = model.Nome;
            modeloDb.Perfil = model.Perfil;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                modeloDb.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _context.Usuarios.FindAsync(id);

            if (model == null) return NotFound();
            _context.Usuarios.Remove(model);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private void GerarLinks(Usuario model)
        {
            model.Links.Clear();

            var idObj = new { id = model.Id };

            model.Links.Add(new LinkDto(model.Id,
                Url.ActionLink(nameof(GetById), values: idObj),
                "self", "GET"));

            model.Links.Add(new LinkDto(model.Id,
                Url.ActionLink(nameof(GetById), values: idObj),
                "update", "PUT"));

            model.Links.Add(new LinkDto(model.Id,
                Url.ActionLink(nameof(GetById), values: idObj),
                "delete", "DELETE"));

            model.Links.Add(new LinkDto(model.Id,
                Url.ActionLink(nameof(Create)),
                "create", "POST"));
        }
    }
}
