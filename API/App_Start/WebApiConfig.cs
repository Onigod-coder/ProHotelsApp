using System.Web.Http;
using System.Web.Http.Cors;

namespace HotelsApp.API.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Настройка маршрутов API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Настройка CORS (разрешить все источники для разработки)
            // В продакшене нужно указать конкретные домены
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Настройка JSON форматирования
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = 
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss";
            
            // Удаление XML форматтера (оставляем только JSON)
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Регистрация фильтров
            config.Filters.Add(new Filters.ExceptionFilter());
        }
    }
}

