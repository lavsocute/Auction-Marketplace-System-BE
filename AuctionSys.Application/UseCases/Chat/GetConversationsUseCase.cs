using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Chat;
using AuctionSys.Application.Interfaces.UseCases.Chat;
using AuctionSys.Domain.Interfaces;
using AutoMapper;

namespace AuctionSys.Application.UseCases.Chat;

public class GetConversationsUseCase : IGetConversationsUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetConversationsUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<ConversationDto>>> ExecuteAsync(Guid userId)
    {
        var allMessages = await _unitOfWork.ChatMessages.GetAsync(m => m.SenderId == userId || m.ReceiverId == userId);
        
        var conversations = allMessages
            .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Select(g => new
            {
                PartnerId = g.Key,
                LastMessage = g.OrderByDescending(m => m.SentAt).FirstOrDefault(),
                UnreadCount = g.Count(m => m.ReceiverId == userId && !m.IsRead)
            })
            .OrderByDescending(c => c.LastMessage?.SentAt)
            .ToList();

        var result = new List<ConversationDto>();

        foreach (var conv in conversations)
        {
            var partner = await _unitOfWork.Users.GetByIdAsync(conv.PartnerId);
            if (partner != null)
            {
                result.Add(new ConversationDto
                {
                    PartnerId = partner.Id,
                    PartnerName = partner.FullName,
                    PartnerAvatarUrl = partner.AvatarUrl ?? string.Empty,
                    LastMessage = _mapper.Map<ChatMessageDto>(conv.LastMessage),
                    UnreadCount = conv.UnreadCount
                });
            }
        }

        return ApiResponse<List<ConversationDto>>.Success(result, "Lấy danh sách hội thoại thành công.");
    }
}
