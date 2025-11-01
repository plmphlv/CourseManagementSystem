using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiControllerBase : Controller
    {
        private ISender mediator = null!;

        protected ISender Mediator => mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    }
}
