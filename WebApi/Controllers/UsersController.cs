using Application.Users.Commands.ChangePassword;
using Application.Users.Commands.Common;
using Application.Users.Commands.Login;
using Application.Users.Commands.RefreshToken;
using Application.Users.Commands.Register;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class UsersController : ApiControllerBase
    {
        [HttpPost("Login")]
        public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] LoginCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterCommand command)
        {
            await Mediator.Send(command);

            return NoContent();
        }

        [HttpPut("ChangePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            await Mediator.Send(command);

            return NoContent();
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<AuthenticationResponse>> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            return await Mediator.Send(command);
        }
    }
}
