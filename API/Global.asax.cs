using System.Web;
using System.Web.Http;

namespace HotelsApp.API
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(App_Start.WebApiConfig.Register);
        }
    }
}

