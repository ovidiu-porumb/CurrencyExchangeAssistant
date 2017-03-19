using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Features.Variance;
using Autofac.Integration.WebApi;
using MediatR;
using OP.CurrencyExchangeAssistant.WebService.Handlers;
using OP.CurrencyExchangeAssistant.WebService.Queries;

namespace OP.CurrencyExchangeAssistant.WebService
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            RegisterDependencies();
        }

        private static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            var config = GlobalConfiguration.Configuration;
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            RegisterServices(builder);

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly).AsSelf().AsImplementedInterfaces();
            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });

            RegisterCommands(builder);
            RegisterQueries(builder);
        }

        private static void RegisterCommands(ContainerBuilder builder)
        {
            //builder.RegisterAssemblyTypes(typeof(RegisterDate).GetTypeInfo().Assembly).AsImplementedInterfaces();
            //builder.RegisterAssemblyTypes(typeof(RegisterDateHandler).GetTypeInfo().Assembly).AsImplementedInterfaces();
        }

        private static void RegisterQueries(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(ExchangeFluctuationQuery).GetTypeInfo().Assembly).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(ExchangeFluctuationHandler).GetTypeInfo().Assembly).AsImplementedInterfaces();
        }
    }
}
