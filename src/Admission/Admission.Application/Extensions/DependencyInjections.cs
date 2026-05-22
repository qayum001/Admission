using Admission.Application.Common;
using Admission.Application.Services;
using LiteBus.Commands;
using LiteBus.Commands.Abstractions;
using LiteBus.Events;
using LiteBus.Events.Abstractions;
using LiteBus.Extensions.Microsoft.DependencyInjection;
using LiteBus.Queries;
using LiteBus.Queries.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Admission.Application.Extensions;

public static class DependencyInjections
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventDispatcher, LiteBusDomainEventDispatcher>();
        services.AddScoped<DictionaryEntityResolver>();

        services.AddLiteBus(liteBus =>
        {
            var appAssembly = typeof(DependencyInjections).Assembly;

            liteBus.AddCommandModule(module => module.RegisterFromAssembly(appAssembly));
            liteBus.AddQueryModule(module => module.RegisterFromAssembly(appAssembly));
            liteBus.AddEventModule(module => module.RegisterFromAssembly(appAssembly));
        });

        RegisterHandlers(services);
    }

    private static void RegisterHandlers(IServiceCollection services)
    {
        var appAssembly = typeof(DependencyInjections).Assembly;

        var handlerInterfaces = new[]
        {
            typeof(ICommandHandler<>),
            typeof(ICommandHandler<,>),
            typeof(IQueryHandler<,>),
            typeof(IEventHandler<>)
        };

        var handlers = appAssembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Select(t => new
            {
                Implementation = t,
                Contracts = t.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                                handlerInterfaces.Contains(i.GetGenericTypeDefinition()))
                    .ToArray()
            })
            .Where(x => x.Contracts.Length > 0);

        foreach (var handler in handlers)
            foreach (var contract in handler.Contracts)
                services.AddScoped(contract, handler.Implementation);
    }
}
