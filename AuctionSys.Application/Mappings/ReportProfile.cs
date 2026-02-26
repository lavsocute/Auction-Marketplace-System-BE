using AuctionSys.Application.DTOs.Report;
using AutoMapper;

namespace AuctionSys.Application.Mappings;

public class ReportProfile : Profile
{
    public ReportProfile()
    {
        CreateMap<Domain.Entities.Report, ReportDto>();
    }
}
