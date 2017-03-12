namespace MVCMongo
{
    using AutoMapper;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using MVCMongo.Core.Abstraction;
    using MVCMongo.Services.MapperProfiles;
    using MVCMongo.Core.Model;
    using MVCMongo.Data.Contexts;
    using MVCMongo.Data.Repository;
    using MVCMongo.Services;
    using JWTIssuer;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using System;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;

    public class Startup
    {
        private readonly string _secretKey;
        private readonly SymmetricSecurityKey _signingKey;
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            
            _secretKey = Configuration.GetSection("TokenOptions:SecretKey").Value;
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey));
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            var jwtAppSettingOptions = Configuration.GetSection(nameof(TokenOptions));
            // Configure Jwt Options
            services.Configure<TokenOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(TokenOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(TokenOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            ConfigureMongoSettings(services);
            ConfigureTokenIssuer(services);
            ConfigureDependecies(services);
            ConfigurAuthorizationPolicy(services);

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            ConfigureAuthorizationMiddleWare(app);
            app.UseMvc();
        }

        private void ConfigureDependecies(IServiceCollection services)
        {
            // AutoMapperConfig
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfileConfiguration());
            });

            var mapper = config.CreateMapper();

            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IProductRepostory, ProductRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddScoped<IMongoContext, MongoContext>();

            services.AddAutoMapper();
        }

        private void ConfigureTokenIssuer(IServiceCollection services)
        {
            // JWT: Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(TokenOptions));

            var issue12 = jwtAppSettingOptions["Issuer"];
            var s = nameof(TokenOptions.Issuer);
            // JWT: Configure JwtIssuerOptions
            services.Configure<TokenOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(TokenOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(TokenOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });
        }

        private void ConfigurAuthorizationPolicy(IServiceCollection services)
        {
            //Claims-Based Authorization.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("SuperUser",
                      policy => policy.RequireClaim("SuperAdmin", "IAmSuperAdmin"));
            });
        }

        private void ConfigureAuthorizationMiddleWare(IApplicationBuilder app)
        {
            var jwtAppSettingOptions = Configuration.GetSection(nameof(TokenOptions));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(TokenOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(TokenOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.FromMinutes(0)
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters,
                Events = new JwtBearerEvents
                {
                    OnChallenge = async context => {
                        // Override the response status code.
                        context.Response.StatusCode = 401;

                        if (!string.IsNullOrEmpty(context.Error))
                        {
                            await context.Response.WriteAsync(context.Error);
                        }

                        if (!string.IsNullOrEmpty(context.ErrorDescription))
                        {
                            await context.Response.WriteAsync(context.ErrorDescription);
                        }

                        context.HandleResponse();
                    }
                }
            });
        }

        private void ConfigureMongoSettings(IServiceCollection services)
        {
            // MongoDB connection setting
            services.Configure<MongoSettings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.Database = Configuration.GetSection("MongoConnection:Database").Value;
            });
        }
    }
}