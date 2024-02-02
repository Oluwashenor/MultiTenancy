using Microsoft.EntityFrameworkCore;

namespace MultiTenancy
{
	public class DiaryDbContext : DbContext
	{
		private readonly ITenant _tenant;
		public DiaryDbContext(DbContextOptions<DiaryDbContext> options, ITenant tenant) : base(options)
		{
			_tenant = tenant;
		}

		public DbSet<Diary> Diaries { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) =>
			modelBuilder.Entity<Diary>().HasQueryFilter(t => t.TenantId == _tenant.Id);

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
		{
			foreach (var entry in ChangeTracker.Entries<Diary>())
			{
				if (entry.State == EntityState.Added)
				{
					entry.Entity.TenantId = _tenant.Id;
				}
			}
			return base.SaveChangesAsync(cancellationToken);
		}
	}
}
