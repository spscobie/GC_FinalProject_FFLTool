using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GC_FinalProject_FFLTool.Startup))]
namespace GC_FinalProject_FFLTool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
