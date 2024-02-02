using System.ComponentModel.DataAnnotations;

namespace MultiTenancy
{
	public class Diary
	{
		[Key]
		public int Id { get; set; }
		public string Name  { get; set; }
		public int TenantId  { get; set; }
	}
}
