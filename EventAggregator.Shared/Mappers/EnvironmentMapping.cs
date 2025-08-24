using Microsoft.Extensions.Hosting;

namespace EventAggregator.Shared.Mappers;

public static class EnvironmentMapping
{
    private readonly static string[] _developments = ["Development"];
    private readonly static string[] _stagings = ["Staging"];
    private readonly static string[] _productions = ["Production"];

    public static string Map(string environmentName) => environmentName switch
    {
        var env when _developments.Contains(env) => Environments.Development,
        var env when _stagings.Contains(env) => Environments.Staging,
        var env when _productions.Contains(env) => Environments.Production,
        _ => throw new InvalidOperationException($"Unsupported environment: {environmentName}")
    };
}
