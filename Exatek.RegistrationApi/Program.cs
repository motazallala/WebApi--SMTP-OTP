using Exatek.RegistrationApi.Services;
using Exatek.RegistrationApi.Services.Config;
using Exatek.RegistrationApi.Services.Interfase;
using Exatek.RegistrationEF.AppData;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ExatekRegistrationDB"));
});
//Mail Configuration
builder.Services.Configure<MailConfig>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
//Phone Configuration
builder.Services.AddSingleton<IPhoneSenderService, PhoneSenderService>();
//OTP Configuration
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IOtpService,OtpService>();
//
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
