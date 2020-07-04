namespace Hyperledger.Aries.OpenApi.Infrastructure
{
  using AutoMapper;
  using Hyperledger.Aries.OpenApi.Features.Connections;
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

