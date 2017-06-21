using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(lecturate.Startup))]
namespace lecturate
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
