using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Team2LibraryProject_01.Startup))]
namespace Team2LibraryProject_01
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
