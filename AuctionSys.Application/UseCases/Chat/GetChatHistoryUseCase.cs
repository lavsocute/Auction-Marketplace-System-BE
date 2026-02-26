using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Chat;
using AuctionSys.Application.Interfaces.UseCases.Chat;
using AuctionSys.Domain.Interfaces;
using AutoMapper;

namespace AuctionSys.Application.UseCases.Chat;

public class GetChatHistoryUseCase : IGetChatHistoryUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetChatHistoryUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResponse<ChatMessageDto>>> ExecuteAsync(Guid userId, Guid partnerId, int pageNumber = 1, int pageSize = 20)
    {
        var history = await _unitOfWork.ChatMessages.GetAsync(m => 
            (m.SenderId == userId && m.ReceiverId == partnerId) || 
            (m.SenderId == partnerId && m.ReceiverId == userId));

        var totalCount = history.Count;

        var pagedHistory = history
            .OrderByDescending(m => m.SentAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Mark as read received messages
        var unreadReceived = pagedHistory.Where(m => m.ReceiverId == userId && !m.IsRead).ToList();
        if (unreadReceived.Any())
        {
            foreach (var msg in unreadReceived)
            {
                msg.IsRead = true;
                await _unitOfWork.ChatMessages.UpdateAsync(msg);
            }
            await _unitOfWork.CompleteAsync();
        }

        var dtos = _mapper.Map<List<ChatMessageDto>>(pagedHistory);

        return ApiResponse<PagedResponse<ChatMessageDto>>.Success(new PagedResponse<ChatMessageDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        }, "Lấy lịch sử tin nhắn thành công.");
    }
}
