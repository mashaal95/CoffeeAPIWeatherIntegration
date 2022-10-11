
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "RedisDemo_";
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


builder.Services.AddScoped<ICoffeeRepository, CoffeeRepository>();



var app = builder.Build();

app.MapGet("/brew-coffee", async Task<object> (HttpContext context, IDistributedCache cache, ICoffeeRepository coffeeRepository) =>
{

    var response = await coffeeRepository.GetCoffeeAsync(cache, context);

    return response;
})
    .WithName("brew-coffee");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();



app.Run();

