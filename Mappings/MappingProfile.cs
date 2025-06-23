using AutoMapper;
using ClienteApi.DTOs;
using ClienteApi.ExternalModels;
using ClienteApi.Models;

namespace ClienteApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Cliente, ClienteDTO>().ReverseMap();
            CreateMap<ClienteCreateDTO, Cliente>().ReverseMap();
            CreateMap<Cliente, ClienteDetailsDTO>().ReverseMap();
            CreateMap<Cliente, ClienteUpdateDTO>().ReverseMap();


            CreateMap<Endereco, EnderecoUpdateCompletoDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Endereco, EnderecoUpdateParcialDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Endereco, EnderecoDTO>().ReverseMap();

            CreateMap<Endereco, EnderecoCreateDTO>().ReverseMap();

            CreateMap<ViaCepResponse, Endereco>()
    .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Localidade))
    .ForMember(dest => dest.Complemento, opt => opt.Ignore())
    .ReverseMap()
    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    
            CreateMap<ViaCepResponse, EnderecoDTO>()
    .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Localidade)).ReverseMap();


            CreateMap<Contato, ContatoDTO>().ReverseMap();
            CreateMap<ContatoCreateDTO, Contato>().ReverseMap();
            CreateMap<ContatoDTO, ContatoUpateDTO>().ReverseMap();
            CreateMap<Contato, ContatoUpateDTO>().ReverseMap();


        }

    }
}