using Autofac;
using AutoMapper;
using Tidal.GoogleMaps.Models.dto;
using Tidal.GoogleMaps.Models.Models;

namespace Tidal.GoogleMaps.Models.Mapper
{
    public class AutoFacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {


            builder.Register(context => new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Map, CreateUpdateMarkerDto>();
                cfg.CreateMap<CreateUpdateMarkerDto, Map>();

            })).AsSelf().SingleInstance();

            builder.Register(c =>
                {
                    //This resolves a new context that can be used later.
                    var context = c.Resolve<IComponentContext>();
                    var config = context.Resolve<MapperConfiguration>();
                    return config.CreateMapper(context.Resolve);
                })
                .As<IMapper>()
                .InstancePerLifetimeScope();
        }
    }
}