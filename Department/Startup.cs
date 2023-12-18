using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Department.Startup))]
namespace Department
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
