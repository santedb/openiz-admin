using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OpenIZAdmin.Startup))]
namespace OpenIZAdmin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
