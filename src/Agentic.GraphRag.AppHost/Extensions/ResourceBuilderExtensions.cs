namespace Agentic.GraphRag.AppHost.Extensions;

internal static class ResourceBuilderExtensions
{
    public static string? GetValue(this IResourceBuilder<ParameterResource> parameter) =>
        parameter.Resource.GetValueAsync(default).AsTask().GetAwaiter().GetResult();
}
