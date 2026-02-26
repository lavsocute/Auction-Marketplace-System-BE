using AuctionSys.Application.Common.Models;

namespace AuctionSys.Application.Interfaces.UseCases.Report;

public interface IResolveReportUseCase
{
    Task<ApiResponse<string>> ExecuteAsync(Guid reportId, string resolutionNotes); // Optional: add notes to describe how it was resolved if needed, or just change status.
}
