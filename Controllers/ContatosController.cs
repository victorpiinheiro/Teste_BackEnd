using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ClienteApi.Data;
using ClienteApi.DTOs;
using ClienteApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClienteApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContatosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;


        public ContatosController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ContatoDTO>> CreateContato([FromBody] ContatoCreateDTO dto)
        {
            var isCliente = await _context.Clientes.FirstOrDefaultAsync(
                c => c.Id == dto.ClienteId);

            if (isCliente == null)
            {
                BadRequest(new
                {
                    message = "ClienteID inválido"
                });
            }

            var contato = _mapper.Map<Contato>(dto);
            _context.Contatos.Add(contato);
            await _context.SaveChangesAsync();

            var contatoDto = _mapper.Map<ContatoCreateDTO>(contato);

            

            return CreatedAtAction(nameof(GetContatoById), new { id = contato.Id }, new
            {
                message = "Contato Cadastrado com sucesso",
                dados = contatoDto
            });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContatoDTO>>> GetContatos()
        {
            var contatos = await _context.Contatos.ToListAsync();
            if (contatos == null || contatos.Count == 0)
            {
                return NotFound( new
                {
                    message = "Nenhum registro encontrado"
                });
            }

            var contatosDto = _mapper.Map<List<ContatoDTO>>(contatos);
            return Ok(contatosDto);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ContatoDTO>>> GetContatoById(int id)
        {
            var contato = await _context.Contatos.FirstOrDefaultAsync(c => c.Id == id);
            if (contato == null)
            {
                return NotFound(new
                {
                    message = "ID inválido"
                });
            }

            var contatoDto = _mapper.Map<ContatoDTO>(contato);
            return Ok(contatoDto);
        }

        [HttpGet("pesquisar")]
        public async Task<ActionResult<ContatoDTO>> SearchContato([FromQuery] string termo)
        {
            var filtraContatos = await _context.Contatos.Where(
                c =>
                c.Texto.Contains(termo)
            ).ToListAsync();

            if (filtraContatos == null || filtraContatos.Count == 0)
            {
                return NotFound(new
                {
                    message = "Nenhum registro encontrado"
                });
            }

            var contatosDto = _mapper.Map<List<ContatoDTO>>(filtraContatos);
            return Ok(contatosDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteContato(int id)
        {
            var contato = await _context.Contatos.FindAsync(id);
            if (contato == null)
            {
                return NotFound("ID inválido ou usuário não existe");
            }

            _context.Contatos.Remove(contato);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "Contato deletado com sucesso"
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ContatoDTO>> UpdateContato(int id, [FromBody] ContatoUpateDTO dto)
        {
            var contato = await _context.Contatos.FindAsync(id);
            if (contato == null)
            {
                return NotFound(new
                {
                    message = "ID inválido ou usuário não existe"
                });
            }

            _mapper.Map(dto, contato);
            await _context.SaveChangesAsync();

            var contatoAtualizadoDto = _mapper.Map<ContatoDTO>(contato);
            return Ok(new
            {
                message = "Contato atualizado com sucesso",
                data = contatoAtualizadoDto
            });


        }
    }
}