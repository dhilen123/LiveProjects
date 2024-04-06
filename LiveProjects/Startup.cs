using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LiveProjects.Startup))]
namespace LiveProjects
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
