﻿using AutoMapper;
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
    public class UserProfileMaping : Profile
    {
        public UserProfileMaping() {
            CreateMap<UserProfile, UserProfileDTO>().ReverseMap();

            CreateMap<UserProfile, UpdateUserProfileDTO>().ReverseMap();
        }
    }
}
