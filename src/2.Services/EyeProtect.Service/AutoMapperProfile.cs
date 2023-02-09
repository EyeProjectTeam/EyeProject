using AutoMapper;
using EyeProtect.Domain.Members;
using EyeProtect.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //AutoMapper
            CreateMap<Member, loginOuput>();
        }
    }
}
