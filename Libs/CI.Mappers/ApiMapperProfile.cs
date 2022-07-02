using AutoMapper;
using CI.Commands.API;

namespace CI.Mappers
{
    class ApiMapperProfile : Profile
    {
        public ApiMapperProfile()
        {
            // Controller to Command
            CreateMap<Model.Models.User, UserCommand>()
                .ForMember(c => c.UserID, opt => opt.MapFrom(src => src.UserID))
                .ForMember(c => c.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(c => c.Password, opt => opt.MapFrom(src => src.Password))
            ;

            CreateMap<Model.Models.AuthenticateUser, AuthenticateCommand>();

            CreateMap<Model.Domain.MeetingEventDomain, MeetingEventCommandCU>();

            // Model to Entity
            CreateMap<Model.Models.User, Data.Entity.User>();
            CreateMap<Model.Models.MeetingEvent, Data.Entity.MeetingEvent>();

            // Reverse Entity to Model
            CreateMap<Data.Entity.User, Model.Models.User>();
            CreateMap<Data.Entity.MeetingEvent, Model.Models.MeetingEvent>();
        }
    }
}