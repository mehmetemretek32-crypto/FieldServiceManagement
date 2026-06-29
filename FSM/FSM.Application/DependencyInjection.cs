using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FSM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // FluentValidation'ı sisteme tanıtıyoruz
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}