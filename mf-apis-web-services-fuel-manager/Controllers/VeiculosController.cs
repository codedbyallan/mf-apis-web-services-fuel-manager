using mf_apis_web_services_fuel_manager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mf_apis_web_services_fuel_manager.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VeiculosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public VeiculosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var model = await _context.Veiculos.ToListAsync();

            foreach (var item in model)
            {
                GerarLinks(item);
            }

            return Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Veiculo model)
        {
            if (model.AnoFabricacao <= 0 || model.AnoModelo <= 0)
            {
                return BadRequest(new
                {
                    message = "Ano de fabricação e Ano do modelo são obrigatórios e devem ser maiores que zero"
                });
            }

            _context.Veiculos.Add(model);
            await _context.SaveChangesAsync();

            GerarLinks(model);

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var model = await _context.Veiculos
                .Include(t => t.Usuarios).ThenInclude(t=> t.Usuario)
                .Include(t => t.Consumos)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (model == null)
                return NotFound();
            GerarLinks(model);
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Veiculo model)
        {
            if (id != model.Id)
                return BadRequest();
            var modeloDb = await _context.Veiculos
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (modeloDb == null)
                return NotFound();
            _context.Veiculos.Update(model);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _context.Veiculos.FindAsync(id);

            if (model == null) return NotFound();
            _context.Veiculos.Remove(model);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private void GerarLinks(Veiculo model)
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

        [HttpPost ("{Id}/usuarios")]
        public async Task<ActionResult> AddUsuario(int Id, VeiculoUsuarios model)
        {
            if (Id != model.VeiculoId) return BadRequest();
            
            _context.VeiculoUsuarios.Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetById", new { id = model.VeiculoId }, model);
        }

        [HttpDelete ("{Id}/usuarios/{usuarioId}")]
        public async Task<ActionResult> DeleteUsuario(int Id, int usuarioId)
        {
            var model = await _context.VeiculoUsuarios
                .Where(c => c.VeiculoId == Id && c.UsuarioId == usuarioId)
                .FirstOrDefaultAsync();

            if (model == null) return NotFound();
            _context.VeiculoUsuarios.Remove(model);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}
