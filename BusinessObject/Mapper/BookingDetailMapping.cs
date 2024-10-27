using AutoMapper;
using BusinessObject.Model;
using BusinessObject.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;

namespace BusinessObject.Mapper
{
    public class BookingDetailMapping : Profile
    {
        public BookingDetailMapping()
        {
            CreateMap<BookingDetail, ViewBookingDetailDTO>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.ServiceName))
                .ForMember(dest => dest.StylistName, opt => opt.MapFrom(src => src.Stylist.UserName));

        }
    }
}
