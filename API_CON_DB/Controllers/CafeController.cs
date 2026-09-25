using API_CON_DB.DB;
using API_CON_DB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_CON_DB.Controllers;

[ApiController]
[Route("api/cafe")]
public class CafeController : ControllerBase
{
    private readonly AppDbContext _context;

    public CafeController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cafe>>> GetAll() =>
        await _context.Cafes.AsNoTracking().ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cafe>> GetById(int id)
    {
        var cafe = await _context.Cafes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return cafe is null ? NotFound() : Ok(cafe);
    }

    [HttpPost]
    public async Task<ActionResult<Cafe>> Create(Cafe cafe)
    {
        _context.Cafes.Add(cafe);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = cafe.Id }, cafe);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Cafe input)
    {
        if (id != input.Id) return BadRequest("El id de la ruta debe coincidir con el id del registro.");

        var cafe = await _context.Cafes.FindAsync(id);
        if (cafe is null) return NotFound();

        cafe.Nombre = input.Nombre;
        cafe.Especialidad = input.Especialidad;
        cafe.Presentacion = input.Presentacion;
        cafe.Origen = input.Origen;
        cafe.Cantidad = input.Cantidad;
        cafe.Valor = input.Valor;
        cafe.Descripcion = input.Descripcion;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cafe = await _context.Cafes.FindAsync(id);
        if (cafe is null) return NotFound();

        _context.Cafes.Remove(cafe);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
