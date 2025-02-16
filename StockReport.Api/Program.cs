using StockReport.Core;
using StockReport.Extensions;

var builder = WebApplication.CreateBuilder(args);

Extension.SqlDbconnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();
builder.Services.AddApplicationServices();

// builder.WebHost.UseKestrel()
//               .UseUrls("http://localhost:5277");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();
app.UseCors("CorsPolicy");
app.Run($"http://*:{builder.Configuration.GetValue<int>("portnumber")}");

