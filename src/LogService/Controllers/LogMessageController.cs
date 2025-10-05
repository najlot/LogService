using LogService.Contracts.Commands;
using LogService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LogMessageController(LogMessageService logMessageService) : ControllerBase
{
	private static Guid GetUserId(ClaimsPrincipal principal)
	{
		var name = principal.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrEmpty(name))
		{
			throw new InvalidOperationException("User id not found");
		}

		var userId = Guid.Parse(name);
		return userId;
	}

	[HttpPut]
	public async Task<ActionResult> Create([FromBody] CreateLogMessage[] commands)
	{
		var userId = GetUserId(User);
		var source = User.Claims.FirstOrDefault(c => c.Type == "Source")?.Value ?? "";
		await logMessageService.CreateLogMessages(commands, source, userId).ConfigureAwait(false);
		return Ok();
	}
}