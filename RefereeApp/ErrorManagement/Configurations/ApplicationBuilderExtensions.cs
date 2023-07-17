using RefereeApp.ErrorManagement.Configurations;

namespace RefereeApp.Configurations;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder AddGlobalErrorHandler(this IApplicationBuilder applicationBuilder) =>
        applicationBuilder.UseMiddleware<GlobalErrorHandlingMiddleware>();
}