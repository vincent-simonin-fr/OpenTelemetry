using Microsoft.AspNetCore.Mvc;
using Telemetry;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureTelemetry();

var app = builder.Build();

// Configure the Prometheus scraping endpoint
app.MapPrometheusScrapingEndpoint();

app.MapGet("/health", () => "Hello World! I'm shaka shaka!");

app.MapGet("/", SendGreeting);

app.MapGet("/NestedGreeting", SendNestedGreeting);

async Task<string> SendGreeting(ILogger<Program> logger)
{
    // Create a new Activity scoped to the method
    using var activity = Greeter.GreeterActivitySource.StartActivity("GreeterActivity");

    // Log a message
    logger.LogInformation("Sending greeting");

    // Increment the custom counter
    Greeter.CountGreetings.Add(1);

    // Add a tag to the Activity
    activity?.SetTag("greeting", "Hello World!");

    return "Hello World!";
}

async Task SendNestedGreeting(int nestlevel, HttpContext context, [FromServices] ILogger<Program> logger, [FromServices] IHttpClientFactory clientFactory)
{
    // Create a new Activity scoped to the method
    using var activity = Greeter.GreeterActivitySource.StartActivity("GreeterActivity");

    if (nestlevel <= 5)
    {
        // Log a message
        logger.LogInformation("Sending greeting, level {nestlevel}", nestlevel);

        // Increment the custom counter
        Greeter.CountGreetings.Add(1);

        // Add a tag to the Activity
        activity?.SetTag("nest-level", nestlevel);

        await context.Response.WriteAsync($"Nested Greeting, level: {nestlevel}\r\n");

        if (nestlevel > 0)
        {
            var request = context.Request;
            var url = new Uri($"{request.Scheme}://{request.Host}{request.Path}?nestlevel={nestlevel - 1}");

            // Makes an http call passing the activity information as http headers
            var nestedResult = await clientFactory.CreateClient().GetStringAsync(url);
            await context.Response.WriteAsync(nestedResult);
        }
    }
    else
    {
        // Log a message
        logger.LogError("Greeting nest level {nestlevel} too high", nestlevel);
        await context.Response.WriteAsync("Nest level too high, max is 5");
    }
}

app.Run();
