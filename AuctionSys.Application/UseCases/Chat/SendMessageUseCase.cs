using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Chat;
using AuctionSys.Application.Interfaces.UseCases.Chat;
using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;
using AutoMapper;

namespace AuctionSys.Application.UseCases.Chat;

public class SendMessageUseCase : ISendMessageUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SendMessageUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ChatMessageDto>> ExecuteAsync(Guid senderId, SendMessageDto request)
    {
        var receiver = await _unitOfWork.Users.GetByIdAsync(request.ReceiverId);
        if (receiver == null)
            return ApiResponse<ChatMessageDto>.Fail("Người nhận không tồn tại.", 404);

        if (senderId == request.ReceiverId)
            return ApiResponse<ChatMessageDto>.Fail("Bạn không thể tự gửi tin nhắn cho bản thân.", 400);

        var message = new ChatMessage
        {
            SenderId = senderId,
            ReceiverId = request.ReceiverId,
            Content = request.Content,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };

        await _unitOfWork.ChatMessages.AddAsync(message);

        // Tạo Notification
        var sender = await _unitOfWork.Users.GetByIdAsync(senderId);
        await _unitOfWork.Notifications.AddAsync(new AuctionSys.Domain.Entities.Notification
        {
            UserId = request.ReceiverId,
            Type = NotificationType.NewMessage,
            Title = "Bạn có tin nhắn mới!",
            Message = $"{sender?.FullName ?? "Ai đó"} vừa gửi tin nhắn cho bạn.",
            ReferenceId = message.Id.ToString()
        });

        await _unitOfWork.CompleteAsync();

        var dto = _mapper.Map<ChatMessageDto>(message);
        return ApiResponse<ChatMessageDto>.Success(dto, "Gửi tin nhắn thành công.");
    }
}
