using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;

namespace AutoSwap.Tests.WebSite.App_Start {
    public class AutofacConfig {
        public static void RegisterAll() {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}