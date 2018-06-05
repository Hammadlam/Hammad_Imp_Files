using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MvcSchoolWebApp.Startup))]
namespace MvcSchoolWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
