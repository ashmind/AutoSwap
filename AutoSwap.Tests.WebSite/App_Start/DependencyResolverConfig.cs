using System.Web.Mvc;
using AutoSwap.Autofac;
using AutoSwap.Tests.WebSite.Models;
using Autofac;
using Autofac.Integration.Mvc;

namespace AutoSwap.Tests.WebSite.App_Start {
    public class DependencyResolverConfig {
        public static void RegisterAll() {
            var builder = new ContainerBuilder();
            builder.RegisterModule<AutoSwapModule>();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterType<VersionService>()
                   .As<IVersionService>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}