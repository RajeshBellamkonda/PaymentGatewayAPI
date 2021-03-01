using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentGateway.API.Mappers;
using PaymentGateway.Services;
using PaymentGateway.Repositories;
using PaymentGateway.ExternalAccess;

namespace PaymentGateway.API
{
    /// <summary>
    /// Base Startup for use by API and test server hosing
    /// </summary>
    public class StartupBase
    {
        public StartupBase(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddControllers();
            services.AddAutoMapper(typeof(PaymentGatewayAPIMapper));
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IPaymentRepository, PaymentRepository>();

            var bankConfig = new BankConfiguration();
            Configuration.GetSection(BankConfiguration.ConfigurationName).Bind(bankConfig);
            services.AddSingleton<IBankConfiguration>(bankConfig);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc");
            });
        }
    }
}
