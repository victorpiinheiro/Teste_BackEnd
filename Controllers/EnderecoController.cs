using AutoMapper;
using ClienteApi.Data;
using ClienteApi.DTOs;
using ClienteApi.ExternalModels;
using ClienteApi.Models;
using ClienteApi.utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClienteApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnderecoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;

        public EnderecoController(AppDbContext context, IHttpClientFactory httpClientFactory, IMapper mapper)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<EnderecoDTO>> CreateEndereco([FromBody] EnderecoCreateDTO dto)
        {

            dto.Cep = dto.Cep.Replace("-", "").Trim();
            if (!Validacoes.CepValido(dto.Cep))
            {
                return BadRequest("CEP inválido: deve conter exatamente 8 números.");

            }

            var cliente = await _context.Clientes.
            Include(c => c.Endereco)
            .FirstOrDefaultAsync(c => c.Id == dto.ClienteId);
            if (cliente == null)
            {
                return BadRequest("ClienteID inválido");
            }

            if (cliente.Endereco != null)
            {
                return Conflict("Este cliente ja possui um endereço cadastrado");
            }

            ViaCepResponse viaCepResponse;
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                viaCepResponse = await httpClient.GetFromJsonAsync<ViaCepResponse>($"https://viacep.com.br/ws/{dto.Cep}/json/");
                if (viaCepResponse.Cep == null || string.IsNullOrEmpty(viaCepResponse.Cep))
                {
                    return BadRequest("CEP inválido ou não existe");
                }
                var endereco = _mapper.Map<Endereco>(dto);
                _mapper.Map(viaCepResponse, endereco);

                _context.Enderecos.Add(endereco);
                await _context.SaveChangesAsync();

                var enderecoDto = _mapper.Map<EnderecoDTO>(endereco);

                return CreatedAtAction(
                    nameof(GetEnderecoById),
                    new { id = endereco.ClienteId },
                    new
                    {
                        message = "Endereço criado com sucesso",
                        dados = enderecoDto }
                );
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, "Serviço ViaCEP fora do ar");
            }


        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnderecoDTO>>> GetEnderecos()
        {
            var enderecos = await _context.Enderecos.ToListAsync();

            if (enderecos.Count == 0)
            {
                return NotFound("Não há dados para mostrar");
            }

            var enderecoDto = _mapper.Map<List<EnderecoDTO>>(enderecos);

            return Ok(enderecoDto);


        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<EnderecoDTO>>> GetEnderecoById(int id)
        {
            var endereco = await _context.Enderecos.FirstOrDefaultAsync(e => e.ClienteId == id);
            if (endereco == null)
            {
                return NotFound("ID inválido ou endereço não existe");
            }

            var enderecoDto = _mapper.Map<EnderecoDTO>(endereco);

            return Ok(enderecoDto);

        }

        [HttpGet("pesquisar")]
        public async Task<ActionResult<IEnumerable<EnderecoDTO>>> SearchCliente([FromQuery] string termo)
        {


            var filtraEnderecos = await _context.Enderecos
            .Include(e => e.Cliente)
            .Where(e =>
                e.Cep.Replace("-", "").Contains(termo) ||
                e.Logradouro.Contains(termo) ||
                e.Cidade.Contains(termo)
            ).ToListAsync();

            if (filtraEnderecos.Count == 0)
            {
                return NotFound("Nenhum endereço encontrado");
            }

            var enderecosFiltradosDto = _mapper.Map<List<EnderecoDTO>>(filtraEnderecos);

            return Ok(enderecosFiltradosDto);

        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEndereco(int id)
        {
            var endereco = await _context.Enderecos.FindAsync(id);
            if (endereco == null)
            {
                return NotFound("ID inválido ou cliente não existe");
            }

            _context.Enderecos.Remove(endereco);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Endereço deletado com sucesso"
            });
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<EnderecoDTO>> UpdateParcialEndereco(
            int id,
            [FromBody]
            EnderecoUpdateParcialDTO dto)

        {
            var endereco = await _context.Enderecos.FindAsync(id);
            if (endereco == null)
            {
                return NotFound("Endereço não encontrado.");
            }

            _mapper.Map(dto, endereco);

            await _context.SaveChangesAsync();

            var enderecoEditadoDto = _mapper.Map<EnderecoDTO>(endereco);

            return Ok(enderecoEditadoDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EnderecoDTO>> UpdateCompletoEndereco(int id,
        [FromBody] EnderecoUpdateCompletoDTO dto)
        {
            var endereco = await _context.Enderecos.FindAsync(id);
            if (endereco == null)
            {
                return NotFound("ID inválido ou endereço não existe");
            }
            dto.Cep = dto.Cep.Replace("-", "").Trim();


            if (!Validacoes.CepValido(dto.Cep))
            {
                return BadRequest("CEP inválido: deve conter exatamente 8 números.");
            }


            ViaCepResponse viaCepResponse;
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                viaCepResponse = await httpClient.GetFromJsonAsync<ViaCepResponse>($"https://viacep.com.br/ws/{dto.Cep}/json/");

                if (viaCepResponse.Cep == null || string.IsNullOrEmpty(viaCepResponse.Cep))
                {
                    return BadRequest("CEP inválido ou não encontrado");
                }

                _mapper.Map(viaCepResponse, endereco);

                _mapper.Map(dto, endereco);
                await _context.SaveChangesAsync();

                var enderecoCompletoDto = _mapper.Map<EnderecoDTO>(endereco);

                return Ok(enderecoCompletoDto);
            }
            catch (HttpRequestException)
            {

                return StatusCode(503, "Serviço ViaCEP fora do ar");
            }
        }
    }



}