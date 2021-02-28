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

namespace PaymentGateway.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
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

            services.AddControllers();
            services.AddSwaggerGen();
            services.AddAutoMapper(typeof(PaymentGatewayAPIMapper));
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IPaymentRepository, PaymentRepository>();

            var bankConfig = new BankConfiguration();
            Configuration.GetSection(BankConfiguration.ConfigurationName).Bind(bankConfig);
            services.AddSingleton<IBankConfiguration>(bankConfig);
            services.AddSingleton<IBankClient, BankClient>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Gateway API");
            });
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
            });
        }
    }
}
