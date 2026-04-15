using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using Admission.MailManager.Inbox.Handlers;
using Admission.MailManager.Inbox.Models;

namespace Admission.MailManager.Inbox.Processing;

public sealed class InboxMessageDispatcher(IServiceProvider serviceProvider) : IInboxMessageDispatcher
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private static readonly ConcurrentDictionary<string, HandlerRegistration> Registrations = BuildRegistrations();

    public async Task DispatchAsync(InboxMessage inboxMessage, CancellationToken cancellationToken = default)
    {
        if (!Registrations.TryGetValue(inboxMessage.MessageType, out var registration))
        {
            throw new InvalidOperationException($"Unsupported inbox message type '{inboxMessage.MessageType}'.");
        }

        var message = JsonSerializer.Deserialize(inboxMessage.PayloadJson, registration.MessageType, SerializerOptions)
            ?? throw new InvalidOperationException(
                $"Failed to deserialize inbox payload '{inboxMessage.Id}' into '{registration.MessageType.FullName}'.");

        using var scope = serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService(registration.HandlerInterfaceType);

        var task = (Task?)registration.HandleMethod.Invoke(handler, [message, cancellationToken]);
        if (task is null)
        {
            throw new InvalidOperationException(
                $"Handler '{registration.HandlerInterfaceType.FullName}' returned null for '{registration.MessageType.FullName}'.");
        }

        await task;
    }

    private static ConcurrentDictionary<string, HandlerRegistration> BuildRegistrations()
    {
        var registrations = new ConcurrentDictionary<string, HandlerRegistration>(StringComparer.Ordinal);
        var handlerInterfaceType = typeof(IInboxMessageHandler<>);
        var handlerTypes = typeof(IInboxMessageHandler<>).Assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .SelectMany(
                type => type.GetInterfaces()
                    .Where(@interface =>
                        @interface.IsGenericType &&
                        @interface.GetGenericTypeDefinition() == handlerInterfaceType)
                    .Select(@interface => new { HandlerType = type, HandlerInterfaceType = @interface }))
            .ToArray();

        foreach (var handlerType in handlerTypes)
        {
            var messageType = handlerType.HandlerInterfaceType.GetGenericArguments()[0];
            var handleMethod = handlerType.HandlerInterfaceType.GetMethod(nameof(IInboxMessageHandler<object>.HandleAsync))
                ?? throw new InvalidOperationException(
                    $"Method '{nameof(IInboxMessageHandler<object>.HandleAsync)}' was not found on '{handlerType.HandlerInterfaceType.FullName}'.");

            var key = messageType.FullName
                ?? throw new InvalidOperationException($"Message type '{messageType}' must have a full name.");

            if (!registrations.TryAdd(key, new HandlerRegistration(messageType, handlerType.HandlerInterfaceType, handleMethod)))
            {
                throw new InvalidOperationException($"Duplicate inbox handler registration for '{key}'.");
            }
        }

        return registrations;
    }

    private sealed record HandlerRegistration(Type MessageType, Type HandlerInterfaceType, MethodInfo HandleMethod);
}
