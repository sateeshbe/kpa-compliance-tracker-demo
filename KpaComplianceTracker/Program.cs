using Microsoft.EntityFrameworkCore;
using KpaComplianceTracker.Data;
using KpaComplianceTracker.Entities;

using Amazon.S3;
using Amazon.Extensions.NETCore.Setup;


using KpaComplianceTracker.Services;  

var builder = WebApplication.CreateBuilder(args);

//DB
builder.Services.AddDbContext<KpaDbContext>(options =>
    options.UseInMemoryDatabase("KpaDemo"));
//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("local", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// AWS and S3 archiver 
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

// Bind bucket name
builder.Services.Configure<S3Options>(builder.Configuration.GetSection("S3"));

// Register S3 archive service
builder.Services.AddScoped<IS3ArchiveService, S3ArchiveService>();

// MVC/Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
//Pipeline
app.UseCors("local");
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
