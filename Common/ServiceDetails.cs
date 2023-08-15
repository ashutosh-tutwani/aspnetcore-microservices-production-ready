namespace Services.Common
{
    public static class ServiceDetails
    {
        public static string Name { get; } = Environment.GetEnvironmentVariable("SERVICE_NAME");

        public static string Namespace { get; } = Environment.GetEnvironmentVariable("Namespace");

        public static string? Version { get; } = Environment.GetEnvironmentVariable("VERSION");

        public static string InstanceId { get; } = Environment.MachineName;
    }
}
