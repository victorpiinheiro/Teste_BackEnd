
using System.Security.Cryptography.X509Certificates;
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
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ClientesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDTO>> CreateCliente([FromBody] ClienteCreateDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Nome))
            {
                return BadRequest(new
                {
                    message = "O campo nome é obrigatório"
                });
            }
            var cliente = _mapper.Map<Cliente>(dto);

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            var clienteDto = _mapper.Map<ClienteDTO>(cliente);

            return CreatedAtAction(nameof(GetClienteById), new { id = cliente.Id }, new
            {
                message = "Cliente adicionado com sucesso",
                dados = clienteDto
            });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> GetClientes()
        {
            var clientes = await _context.Clientes.ToListAsync();
            if (clientes.Count == 0)
            {
                return NotFound(new
                {
                    message = "Nenhum registro encontrado"
                });
            }
            var clientesDto = _mapper.Map<List<ClienteDTO>>(clientes);
            return Ok(new
            {
                clientes = clientesDto
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDetailsDTO>> GetClienteById(int id)
        {
            var cliente = await _context.Clientes
            .Include(c => c.Endereco)
            .Include(c => c.Contatos)
            .FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null)
            {
                return NotFound(new
                {
                    message = "Cliente não encontrado"
                });
            }

            var clienteDto = _mapper.Map<ClienteDetailsDTO>(cliente);
            return Ok(clienteDto);

        }

        [HttpGet("Details")]
        public async Task<ActionResult<IEnumerable<ClienteDetailsDTO>>> GetClientesComDetalhes()
        {
            var clientes = await _context.Clientes
            .Include(c => c.Endereco)
            .Include(c => c.Contatos)
            .ToListAsync();

            if (clientes == null || clientes.Count == 0)
            {
                return NotFound(new
                {
                    message = "Nenhum registro encontrado"
                });
            }

            var clienteDetalhadosDtos = _mapper.Map<List<ClienteDetailsDTO>>(clientes);

            return Ok(clienteDetalhadosDtos);
        }

        [HttpGet("pesquisar")]
        public async Task<ActionResult<IEnumerable<ClienteDetailsDTO>>> SearchClientes(
            [FromQuery] string termo
        )
        {
            var clientesEncontrados = await _context.Clientes
            .Include(c => c.Endereco)
            .Include(c => c.Contatos)
            .Where(c =>
            c.Nome.Contains(termo) ||
            c.Endereco.Cep.Contains(termo) ||
            c.Endereco.Logradouro.Contains(termo) ||
            c.Endereco.Cidade.Contains(termo) ||
            c.Contatos.Any(contato =>
            contato.Texto.Contains(termo))).ToListAsync();

            if (clientesEncontrados.Count == 0)
            {
                return NotFound(new
                {
                    message = "Nenhum cliente encontrado"
                });
            }


            var clientesDetalhadosDto = _mapper.Map<List<ClienteDetailsDTO>>(clientesEncontrados);

            return Ok(clientesDetalhadosDto);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound(new
                {
                    message = "ID inválido ou usuario não existe"
                }
                   );
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Usuario deletado com sucesso"
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] ClienteUpdateDTO dto)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound(new
                {
                    message = "ID inválido ou cliente não existe"
                }
                    );
            }

            _mapper.Map(dto, cliente);
            await _context.SaveChangesAsync();

            var clienteDto = _mapper.Map<ClienteDTO>(cliente);

            return Ok(clienteDto);

        }
    }
}