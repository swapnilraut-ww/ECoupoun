using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ECoupoun.Web.Startup))]
namespace ECoupoun.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
