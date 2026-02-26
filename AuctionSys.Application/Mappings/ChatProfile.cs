using AuctionSys.Application.DTOs.Chat;
using AutoMapper;

namespace AuctionSys.Application.Mappings;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<Domain.Entities.ChatMessage, ChatMessageDto>();
    }
}
