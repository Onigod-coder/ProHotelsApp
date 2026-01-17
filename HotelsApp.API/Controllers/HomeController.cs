using System.Web.Http;

namespace HotelsApp.API.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        [Route("~/")]
        public IHttpActionResult Get()
        {
            return Ok(new
            {
                message = "HotelsApp Web API",
                version = "1.0",
                endpoints = new
                {
                    hotels = "/api/hotels",
                    rooms = "/api/rooms",
                    bookings = "/api/bookings"
                },
                documentation = "Use the endpoints above to access the API resources."
            });
        }
    }
}
