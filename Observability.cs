using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Telemetry;

public static class Observability
{
    public static void ConfigureTelemetry(this WebApplicationBuilder builder)
    {
        var tracingOTlpEndpoint = builder.Configuration["OT_LP_TRACING_ENDPOINT_URL"];
        var logOTlpEndpoint = builder.Configuration["OT_LP_LOGGING_ENDPOINT_URL"];

        var oTel = builder.Services.AddOpenTelemetry();

        // Configure OpenTelemetry Resources with the application name
        oTel.ConfigureResource(resource => resource
            .AddService(serviceName: builder.Environment.ApplicationName));
        
        // If no logs go out to console then the container api w
        // will not appear in container discover by alloy config
        builder.Logging.ClearProviders();
        // Add Logs for ASP.NET Core and export to Loki
        oTel.WithLogging(logs =>
            {
                if (logOTlpEndpoint != null)
                {
                    logs.AddOtlpExporter(exporter =>
                        {
                            exporter.Endpoint = new Uri(logOTlpEndpoint);
                        }
                    );
                }
                else
                {
                    logs.AddConsoleExporter();
                }
            }
        );
        
        // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
        oTel.WithMetrics(metrics => metrics
            // Metrics provider from OpenTelemetry
            .AddAspNetCoreInstrumentation()
            .AddMeter(Greeter.GreeterMeter.Name)
            // Metrics provides by ASP.NET Core in .NET 8
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            // Metrics provided by System.Net libraries
            .AddMeter("System.Net.Http")
            .AddMeter("System.Net.NameResolution")
            .AddPrometheusExporter());

        // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
        oTel.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
            tracing.AddSource(Greeter.GreeterActivitySource.Name);
            if (tracingOTlpEndpoint != null)
            {
                tracing.AddOtlpExporter(oTlpOptions =>
                {
                    oTlpOptions.Endpoint = new Uri(tracingOTlpEndpoint);
                });
            }
            else
            {
                tracing.AddConsoleExporter();
            }
        });
    }
}