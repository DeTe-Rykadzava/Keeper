using KeeperAPI.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataBaseKeeperContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnect")));
builder.Services.AddControllers(opt => opt.AllowEmptyInputInBodyModelBinding = true);

builder.Services.AddCors(opt => opt.AddDefaultPolicy(pol => { pol.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
