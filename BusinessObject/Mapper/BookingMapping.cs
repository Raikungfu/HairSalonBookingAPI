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
    public class BookingMapping : Profile
    {
        public BookingMapping()
        {
            CreateMap<Booking, ViewBookingDTO>()
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.Voucher, opt => opt.MapFrom(src => src.Voucher))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments))
                .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails));

            CreateMap<Booking, ViewManageBookingDTO>()
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.Voucher, opt => opt.MapFrom(src => src.Voucher))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments));

            CreateMap<Booking, ViewManageBookingStylistDTO>()
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.Voucher, opt => opt.MapFrom(src => src.Voucher))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails));


            CreateMap<Booking, BookingRequestDTO>()
                .ForMember(dest => dest.VoucherId, opt => opt.MapFrom(src => src.VoucherId))
                .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
                .ReverseMap();


            CreateMap<Booking, ChangebookingStatusDTO>().ReverseMap();

            CreateMap<Booking, BookingHistoryDTO>()
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
            .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Schedule.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.Schedule.EndDate))
            .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.BookingDetails.FirstOrDefault().ServiceId))  
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.BookingDetails.FirstOrDefault().Service.ServiceName))  
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.BookingDetails.FirstOrDefault().StylistId))  
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.BookingDetails.FirstOrDefault().Stylist.UserName))
            .ReverseMap();  
        }
    }
}
