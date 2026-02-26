namespace AuctionSys.Application.Interfaces.UseCases.Auction;

public interface ICloseAuctionUseCase
{
    Task ExecuteAsync(Guid auctionId);
}
