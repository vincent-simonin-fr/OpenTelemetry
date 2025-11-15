using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Telemetry;

public static class Greeter
{
    public static Meter GreeterMeter => new ("OtPrGrYa.Example", "1.0.0");
    public static Counter<int> CountGreetings => GreeterMeter.CreateCounter<int>("greetings.count", description: "Counts the number of greetings");

    // Custom ActivitySource for the application
    public static ActivitySource GreeterActivitySource => new ("OtPrGrJa.Example");
}
