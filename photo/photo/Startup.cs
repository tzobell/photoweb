using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(photo.Startup))]
namespace photo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
