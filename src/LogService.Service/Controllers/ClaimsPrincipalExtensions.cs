using System;
using System.Security.Claims;

namespace LogService.Service.Controllers
{
	public static class ClaimsPrincipalExtensions
	{
		public static Guid GetUserId(this ClaimsPrincipal principal)
		{
			var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
			return userId;
		}
	}
}