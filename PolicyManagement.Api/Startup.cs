using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Glasswall.PolicyManagement.Api.BackgroundServices;
using Glasswall.PolicyManagement.Business.BackgroundServices;
using Glasswall.PolicyManagement.Business.Configuration;
using Glasswall.PolicyManagement.Business.Serialisation;
using Glasswall.PolicyManagement.Business.Services;
using Glasswall.PolicyManagement.Business.Store;
using Glasswall.PolicyManagement.Common.BackgroundServices;
using Glasswall.PolicyManagement.Common.Configuration;
using Glasswall.PolicyManagement.Common.Serialisation;
using Glasswall.PolicyManagement.Common.Services;
using Glasswall.PolicyManagement.Common.Store;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Glasswall.PolicyManagement.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logging =>
            {
                logging.AddDebug();
            })
                .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);

            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("*",
                    builder =>
                    {
                        builder
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin();
                    });
            });

            services.TryAddScoped(_ => ValidateAndBind(Configuration));

            services.TryAddTransient<IPolicyDistributer, PolicyDistributer>();
            services.TryAddTransient<IPolicyService, PolicyService>();
            services.TryAddTransient<IJsonSerialiser, JsonSerialiser>();
            services.TryAddTransient<IFileStore>(s => new MountedFileStore(s.GetRequiredService<ILogger<MountedFileStore>>(), "/mnt/policies"));
            services.AddHostedService<PolicySynchronizationBackgroundService>();
            services.AddTransient<IPolicySynchronizer, NcfsPolicyDistributer>();
            services.AddTransient<IPolicySynchronizer, AdaptationPolicyDistributer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthorization();
            
            app.Use((context, next) =>
            {
                context.Response.Headers["Access-Control-Allow-Methods"] = "*";
                context.Response.Headers["Access-Control-Expose-Headers"] = "*";
                context.Response.Headers["Access-Control-Allow-Headers"] = "*";
                context.Response.Headers["Access-Control-Allow-Origin"] = "*";

                if (context.Request.Method != "OPTIONS") return next.Invoke();

                context.Response.StatusCode = 200;
                return context.Response.WriteAsync("OK");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCors("*");
        }


        private static IPolicyManagementApiConfiguration ValidateAndBind(IConfiguration configuration)
        {
            ThrowIfNullOrWhitespace(configuration, "PolicyUpdateServiceUsername");
            ThrowIfNullOrWhitespace(configuration, "PolicyUpdateServicePassword");
            ThrowIfNullOrWhitespace(configuration, "NcfsPolicyUpdateServiceUsername");
            ThrowIfNullOrWhitespace(configuration, "NcfsPolicyUpdateServicePassword");
            ThrowIfNullOrWhitespace(configuration, "PolicyUpdateServiceEndpointCsv");
            ThrowIfNullOrWhitespace(configuration, "NcfsPolicyUpdateServiceEndpointCsv");
            
            var businessConfig = new PolicyManagementApiConfiguration();

            configuration.Bind(businessConfig);

            return businessConfig;
        }

        private static void ThrowIfNullOrWhitespace(IConfiguration configuration, string key)
        {
            if (string.IsNullOrWhiteSpace(configuration[key]))
                throw new ConfigurationErrorsException($"{key} was not provided");
        }
    }
}
