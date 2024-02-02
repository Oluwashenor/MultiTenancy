using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;

namespace MultiTenancy
{
	public class TenancyMiddleware
	{
		private readonly RequestDelegate _next;

		public TenancyMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, ITenancyManager tenancyManager, ITenantSetter tenant)
		{
			var path = context.Request.Path;
			if (!path.HasValue)
			{
				context.Response.StatusCode = 500;
				return;
			}
			
			var segments = path.Value.Split('/').Where(x=>x != string.Empty).ToArray();
			if(segments.Length < 1) {
				context.Response.StatusCode = 500;
				await context.Response.WriteAsJsonAsync(new ProblemDetails
				{
					 Detail =  "Tenant is missing",
					 Status = (int)HttpStatusCode.Conflict
				});
				return;
			}

			var tenantName = segments[0];
			var exemptedPaths = new List<string> { "swagger" };
			if (exemptedPaths.Contains(tenantName))
			{
				await _next(context);
				return;
			}
			var currenttenant = tenancyManager.GetTenant(tenantName);
			if(currenttenant is null)
			{
				await context.Response.WriteAsJsonAsync(new ProblemDetails
				{
					Detail = "Tenant is invalid",
					Status = 500
				});
				return;
			}
				tenant.Id = currenttenant.Id;
				tenant.Name = currenttenant.Name;
			    Console.WriteLine(context.Request.Path.ToString());
				context.Request.Path = Path.Combine("/", string.Join("/", segments.Skip(1)));
			Console.WriteLine(context.Request.Path.ToString());
			await _next(context);
		}
	}

	public static class TenancyMiddlewareExtension
	{
		public static IApplicationBuilder UseTenancyMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<TenancyMiddleware>();
		}
	}
}
