namespace BlazorHosted.Infrastructure
{
  using AutoMapper;
  using BlazorHosted.Features.Connections;
  using Hyperledger.Aries.Features.DidExchange;

  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
      //CreateMap<ConnectionInvitationMessage, InvitationDto>();
      CreateMap<ConnectionRecord, CreateInvitationResponse>();
    }
  }
}

