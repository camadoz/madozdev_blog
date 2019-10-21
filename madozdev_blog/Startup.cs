using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(madozdev_blog.Startup))]
namespace madozdev_blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
