using Autofac;
using Autofac.Extensions.DependencyInjection;
using BlazorDatasource.Server.Areas;
using BlazorDatasource.Server.Areas.Identity.Pages.Account;
using BlazorDatasource.Server.Infrastructure;
using BlazorDatasource.Server.Models.Account;
using BlazorDatasource.Server.Models.Common;
using BlazorDatasource.Server.Models.Localization;
using BlazorDatasource.Server.Validators.Account;
using BlazorDatasource.Server.Validators.Localization;
using BlazorDatasource.Shared;
using BlazorDatasource.Shared.Identity;
using BlazorDatasource.Shared.Infrastructure;
using BlazorDatasource.Shared.Services.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Serilog;
using System;
using System.Net.Http.Headers;

namespace BlazorDatasource.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Autofac
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            // Serilog
            builder.Host.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration.Enrich.FromLogContext()
                                          .WriteTo.File("Logs/Errors.log", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, shared: true)
                                          .WriteTo.Console();
            });

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            // Identity Context
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContextFactory<ApplicationDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(connectionString, sqlServerDbContextOptionsBuilder =>
                {
                    sqlServerDbContextOptionsBuilder.MigrationsAssembly("BlazorDatasource.Server");
                });
            });

            // Identity Options
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // User options
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._@+ ";
                options.User.RequireUniqueEmail = true;
                // Password options
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                // SignIn options
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Entity Framework Context
            builder.Services.AddDbContextFactory<EntityObjectContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(connectionString, sqlServerDbContextOptionsBuilder =>
                {
                    sqlServerDbContextOptionsBuilder.MigrationsAssembly("BlazorDatasource.Server");
                });
            });

            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                // Work context
                builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();

                // Repositories
                builder.RegisterGeneric(typeof(EntityRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

                // Services
                builder.RegisterType<LanguageService>().As<ILanguageService>().InstancePerLifetimeScope();
                builder.RegisterType<LocalizationService>().As<ILocalizationService>().InstancePerLifetimeScope();
                builder.RegisterType<LocalizedEntityService>().As<ILocalizedEntityService>().InstancePerLifetimeScope();

                // Validators
                // Account
                builder.RegisterType<LoginInputModelValidator>().As<IValidator<LoginModel.InputModel>>().InstancePerLifetimeScope();
                builder.RegisterType<AccountModelValidator>().As<IValidator<AccountModel>>().InstancePerLifetimeScope();

                // Languages
                builder.RegisterType<LanguageValidator>().As<IValidator<LanguageModel>>().InstancePerLifetimeScope();
                builder.RegisterType<LanguageResourceValidator>().As<IValidator<LocaleResourceModel>>().InstancePerLifetimeScope();

                // IStringLocalizer
                builder.RegisterType<DatabaseStringLocalizer>().As<IStringLocalizer>().InstancePerLifetimeScope();

                // Service to communicate success on edit between pages
                builder.RegisterType<EditSuccess>().AsSelf().InstancePerLifetimeScope();

                // State container for selected dataset from search results
                builder.RegisterType<StateContainer>().AsSelf().InstancePerLifetimeScope();

                // Service to communicate access and refresh tokens to the blazor server application
                builder.RegisterType<TokenProvider>().AsSelf().InstancePerLifetimeScope();

                // Revalidating identity authentication state provider
                builder.RegisterType<RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>().As<AuthenticationStateProvider>().InstancePerLifetimeScope();
            });

            // Blazor localization
            builder.Services.AddLocalization();

            // HttpClient to make calls to BlazorDatasource.Api
            var datasourceApiUrl = builder.Configuration["DatasourceApiUrl"] ??
                throw new InvalidOperationException("Datasource API URL is not configured");

            // Configure the HttpClient for the backend API
            builder.Services.AddHttpClient<DatasourceApiHttpClient>(configureClient =>
            {
                configureClient.BaseAddress = new(datasourceApiUrl);
                configureClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json));
            })
            .ConfigurePrimaryHttpMessageHandler(handler =>
            {
                var clientHandler = new System.Net.Http.HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) => true
                };

                return clientHandler;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseRequestLocalization();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}