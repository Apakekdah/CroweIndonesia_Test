using AutoMapper;

namespace CI.MapperRegistration
{
    public interface IMapperConfigurator
    {
        void SetMapper(Profile profile);
    }
}