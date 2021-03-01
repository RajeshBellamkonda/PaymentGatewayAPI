using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PaymentGateway.API.Mappers;
using PaymentGateway.Services;
using PaymentGateway.Repositories;
using PaymentGateway.ExternalAccess;
using PaymentGateway.API.Metrics;

namespace PaymentGateway.API
{
    public class Startup : StartupBase
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            ConfigureAuthentication(services);
            services.AddSingleton<IServiceMetrics, ServiceMetrics>();
            services.AddSingleton<IBankClient, BankClient>();
            base.ConfigureServices(services);
        }

        public virtual void ConfigureAuthentication(IServiceCollection services)
        {
            var authurl = Configuration.GetValue<string>("AuthenticationServerBaseUrl");
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = authurl;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseAuthentication();

            base.Configure(app, env);
        }
    }
}
