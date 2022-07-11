using AutoDelivery.Core.Core;
using AutoDelivery.Core.Extensions;
using AutoDelivery.Core.Repository;
using AutoDelivery.Domain.Mail;
using AutoDelivery.Domain.Secrets;
using AutoDelivery.Domain.Url;
using AutoDelivery.Service.AutoMapper;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(c => c.AddPolicy("any", p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    opt =>
    {
        opt.Cookie.HttpOnly = true;
        opt.SlidingExpiration = true;
        opt.ExpireTimeSpan = TimeSpan.FromDays(1);
        opt.LoginPath = "/Auth/Login";
        opt.LogoutPath = "/Auth/Logout";
        opt.AccessDeniedPath = "/Auth/Login";
        opt.Cookie.SameSite = SameSiteMode.None;
        opt.Validate();
    }
);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// 启用swagger注释
builder.Services.AddSwaggerGen(
    s => s.EnableAnnotations()
);

var config = builder.Services.BuildServiceProvider().GetService<IConfiguration>() as IConfiguration;
var connStr = config.GetConnectionString("Default");

builder.Services.AddDbContext<AutoDeliveryContext>(
    opt => opt.UseSqlServer(connStr)
    );

builder.Services.AddAutoMapper(typeof(MapperProfile));
var mapperConfig = new MapperConfiguration(cfg => { });
mapperConfig.CompileMappings();


builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(
    containerBuilder =>
    {
        builder.Services.AddModule(containerBuilder);
        containerBuilder.RegisterType<Secrets>().As<ISecrets>();
        containerBuilder.RegisterType<ApplicationUrls>().As<IApplicationUrls>();
        containerBuilder.RegisterType<MailConfig>().As<IMailConfig>();
        containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
        containerBuilder.RegisterType<Mapper>().As<IMapper>();
    }
);




// builder.Services.RepositoryRegister();
// builder.Services.ServiceRegister();

// builder.Services.AddSingleton<ISecrets, Secrets>();
// builder.Services.AddSingleton<IApplicationUrls, ApplicationUrls>();
// builder.Services.AddSingleton<IMailConfig, MailConfig>();


builder.Services.AddAntiforgery(c =>
{
    // All embedded apps are loaded in an iframe. The server must not send the X-Frame-Options: Deny header
    c.SuppressXFrameOptionsHeader = true;
    c.Cookie.SameSite = SameSiteMode.None;
});
builder.Services.Configure<CookiePolicyOptions>(
    opt =>
    {
        opt.MinimumSameSitePolicy = SameSiteMode.Unspecified;
        opt.Secure = CookieSecurePolicy.Always;
    }
);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("any");
app.UseHsts();
app.UseHttpsRedirection();
app.UseStatusCodePages();
app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy();


app.MapControllers();

app.Run();
