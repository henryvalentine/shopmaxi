using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ShopKeeper.Startup))]
namespace ShopKeeper
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
