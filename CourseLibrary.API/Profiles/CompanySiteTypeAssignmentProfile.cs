using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Profiles
{
    public class CompanySiteTypeAssignmentProfile : Profile
    {

        public CompanySiteTypeAssignmentProfile()
        {
            //Source and the Destination mapping...
            CreateMap<Entities.CompanySiteTypes, Models.CompanySiteTypeDto>();

            CreateMap<Models.CompanySiteTypeDto, Entities.CompanySiteTypes>();
            
        }
        
       

    }
}
