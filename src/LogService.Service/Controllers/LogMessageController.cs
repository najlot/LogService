﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogService.Contracts;
using LogService.Service.Services;
using LogService.Contracts.Commands;
using LogService.Contracts.ListItems;
using LogService.Contracts.Filters;

namespace LogService.Service.Controllers;

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
	public async Task<ActionResult<List<LogMessageListItem>>> List()
	{
		var userId = User.GetUserId();
		var query = _logMessageService.GetItemsForUserAsync(userId);
		var items = await query.ToListAsync().ConfigureAwait(false);
		return Ok(items);
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<List<LogMessageListItem>>> ListFiltered(LogMessageFilter filter)
	{
		var userId = User.GetUserId();
		var query = _logMessageService.GetItemsForUserAsync(filter, userId);
		var items = await query.ToListAsync().ConfigureAwait(false);
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

	[HttpPost]
	public async Task<ActionResult> Create([FromBody] CreateLogMessage command)
	{
		var userId = User.GetUserId();
		await _logMessageService.CreateLogMessage(command, userId).ConfigureAwait(false);
		return Ok();
	}

	[HttpPut]
	public async Task<ActionResult> Update([FromBody] UpdateLogMessage command)
	{
		var userId = User.GetUserId();
		await _logMessageService.UpdateLogMessage(command, userId).ConfigureAwait(false);
		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(Guid id)
	{
		var userId = User.GetUserId();
		await _logMessageService.DeleteLogMessage(id, userId).ConfigureAwait(false);
		return Ok();
	}
}