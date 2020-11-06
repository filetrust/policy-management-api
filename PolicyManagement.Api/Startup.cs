using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azure.Storage.Files.Shares;
using Glasswall.PolicyManagement.Business.Configuration;
using Glasswall.PolicyManagement.Business.Serialisation;
using Glasswall.PolicyManagement.Business.Services;
using Glasswall.PolicyManagement.Business.Store;
using Glasswall.PolicyManagement.Common.Configuration;
using Glasswall.PolicyManagement.Common.Configuration.Validation;
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

            services.TryAddTransient<IConfigurationParser, EnvironmentVariableParser>();
            services.TryAddTransient<IDictionary<string, IConfigurationItemValidator>>(_ => new Dictionary<string, IConfigurationItemValidator>
            {
                {nameof(IPolicyManagementApiConfiguration.AccountName), new StringValidator(1)},
                {nameof(IPolicyManagementApiConfiguration.AccountKey), new StringValidator(1)},
                {nameof(IPolicyManagementApiConfiguration.ShareName), new StringValidator(1)},
                {nameof(IPolicyManagementApiConfiguration.PolicyUpdateServiceEndpointCsv), new StringValidator(1)}
            });
            services.TryAddSingleton<IPolicyManagementApiConfiguration>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfigurationParser>();
                return configuration.Parse<PolicyManagementApiConfiguration>();
            });

            services.TryAddTransient<IPolicyDistributer, PolicyDistributer>();
            services.TryAddTransient<IPolicyService, PolicyService>();
            services.TryAddTransient<IJsonSerialiser, JsonSerialiser>();
            services.TryAddTransient<IFileShare, AzureFileShare>();

            services.TryAddTransient(s =>
            {
                var configuration = s.GetRequiredService<IPolicyManagementApiConfiguration>();
                return new ShareServiceClient($"DefaultEndpointsProtocol=https;AccountName={configuration.AccountName};AccountKey={configuration.AccountKey};EndpointSuffix=core.windows.net").GetShareClient(configuration.ShareName);
            });
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
    }
}
