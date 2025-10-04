using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogService.Contracts;
using LogService.Services;
using LogService.Contracts.Commands;
using LogService.Contracts.ListItems;
using LogService.Contracts.Filters;

namespace LogService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LogMessageController : ControllerBase
{
	private readonly LogMessageService _logMessageService;

	public LogMessageController(LogMessageService logMessageService)
	{
		_logMessageService = logMessageService;
	}

	[HttpGet]
	public ActionResult<LogMessageListItem[]> List()
	{
		var userId = User.GetUserId();
		var items = _logMessageService.GetItemsForUserAsync(userId);
		return Ok(items);
	}

	[HttpPost("[action]")]
	public ActionResult<LogMessageListItem[]> ListFiltered(LogMessageFilter filter)
	{
		var userId = User.GetUserId();
		var items = _logMessageService.GetItemsForUserAsync(filter, userId);
		return Ok(items);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<LogMessage>> GetItem(Guid id)
	{
		var userId = User.GetUserId();
		var item = await _logMessageService.GetItemAsync(id, userId).ConfigureAwait(false);
		if (item == null)
		{
			return NotFound();
		}

		return Ok(item);
	}

	[HttpPut]
	public async Task<ActionResult> Create([FromBody] CreateLogMessage[] commands)
	{
		var userId = User.GetUserId();
		var source = User.Claims.FirstOrDefault(c => c.Type == "Source")?.Value ?? "";
		await _logMessageService.CreateLogMessages(commands, source, userId).ConfigureAwait(false);
		return Ok();
	}
}