using AutoMapper;
using StockMarket.Core.Models;

namespace StockMarket.Data.MongoDB
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Stock, StockDocument>()
                .ReverseMap();
        }
    }
}
