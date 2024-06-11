using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BlogMVC.Models;
using BlogMVC.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BlogMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Set up the dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            DependencyResolver.SetResolver(new CustomDependencyResolver(serviceProvider));
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<BlogEntities>();
        }
    }
}
