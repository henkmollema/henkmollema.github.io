using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("[controller]/[action]")]
    public class ValuesController : ApiController
    {
        [HttpGet]
        public MyViewModel Get()
        {
            return new MyViewModel { Message = "Hello, world!" };
        }
    }

    public class MyViewModel
    {
        public string Message { get; set; }
    }
}
