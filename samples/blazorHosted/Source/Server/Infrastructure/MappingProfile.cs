namespace Hyperledger.Aries.AspNetCore.Infrastructure
{
  using AutoMapper;
  using Hyperledger.Aries.AspNetCore.Features.Connections;
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

