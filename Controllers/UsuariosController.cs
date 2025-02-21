using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using L01_2018AC605.Data;
using L01_2018AC605.Models;

namespace L01_2018AC605.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly BlogContext _context;

        public UsuariosController(BlogContext context)
        {
            _context = context;
        }

 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.UsuarioId)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.UsuarioId }, usuario);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("PorNombreApellido")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosPorNombreApellido(string nombre, string apellido)
        {
            var usuarios = await _context.Usuarios
                .Where(u => u.Nombre.Contains(nombre) && u.Apellido.Contains(apellido))
                .ToListAsync();

            if (!usuarios.Any())
            {
                return NotFound("No se encontraron usuarios con esos datos.");
            }

            return Ok(usuarios);
        }

        [HttpGet("PorRol/{rolId}")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosPorRol(int rolId)
        {
            var usuarios = await _context.Usuarios
                .Where(u => u.RolId == rolId)
                .ToListAsync();

            if (!usuarios.Any())
            {
                return NotFound("No se encontraron usuarios con ese rol.");
            }

            return Ok(usuarios);
        }

        [HttpGet("TopNComentarios/{cantidad}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTopNUsuariosConMasComentarios(int cantidad)
        {
            var topUsuarios = await _context.Usuarios
                .Select(u => new
                {
                    UsuarioId = u.UsuarioId,
                    NombreCompleto = u.Nombre + " " + u.Apellido,
                    TotalComentarios = _context.Comentarios.Count(c => c.UsuarioId == u.UsuarioId)
                })
                .OrderByDescending(u => u.TotalComentarios)
                .Take(cantidad)
                .ToListAsync();

            if (!topUsuarios.Any())
            {
                return NotFound("No hay suficientes comentarios registrados.");
            }

            return Ok(topUsuarios);
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}

