using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MultiTenancy;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DiaryDbContext>(options =>
	options.UseSqlServer(connectionString));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITenancyManager, TenancyManager>();
builder.Services.AddScoped<Tenant>();
builder.Services.AddScoped<ITenant>(s=> s.GetRequiredService<Tenant>());
builder.Services.AddScoped<ITenantSetter>(s=> s.GetRequiredService<Tenant>());

var app = builder.Build();

app.UseTenancyMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();


app.MapPost("create", async (Diary diary, DiaryDbContext context) =>
{
	await context.Diaries.AddAsync(diary);
	await context.SaveChangesAsync();
	return Results.Ok(true);
}).WithName("add")
.WithOpenApi();

app.MapGet("all", async (DiaryDbContext context) =>
{
	return Results.Ok(await context.Diaries.ToListAsync());
}).WithName("get")
.WithOpenApi();

app.MapGet("single", async (int id, DiaryDbContext context) =>
{
	return Results.Ok(await context.Diaries.FirstOrDefaultAsync(x=>x.Id == id));
}).WithName("single")
.WithOpenApi();

app.MapGet("/tenantInfo", (ITenant tenant) => {
	return Results.Ok(tenant);
});

app.Run();