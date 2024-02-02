using System.ComponentModel.DataAnnotations;

namespace MultiTenancy
{

	public interface ITenancyManager
	{
		Tenant? GetTenant(string name);
	}

	public class TenancyManager : ITenancyManager
	{
		public Tenant? GetTenant(string name) => name switch
		{
			"clientA" => new Tenant() { Id = 1, Name = "Client A" },
			"clientB" => new Tenant() { Id = 1, Name = "Client B" },
			"clientC" => new Tenant() { Id = 1, Name = "Client C" },
			_ => null
		};
	}

	public class Tenant : ITenant, ITenantSetter
	{
		[Key]
		public int Id { get; set; }	
		public string Name { get; set; }	
	}

	public interface ITenant
	{
		int Id { get; }
		string Name { get; }
	}

	public interface ITenantSetter
	{
		int Id { set; } 
		string Name { set; } 
	}
}
