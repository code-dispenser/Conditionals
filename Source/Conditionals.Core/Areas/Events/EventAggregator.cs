using Conditionals.Core.Common.Seeds;
using System.Collections.Concurrent;

namespace Conditionals.Core.Areas.Events;


/// <inheritdoc cref="IEventAggregator" />
internal class EventAggregator : IEventAggregator
{
    private readonly ConcurrentDictionary<Type, List<WeakReference<Delegate>>> _eventSubscriptions = new();

    private readonly Func<Type, dynamic>? _resolver = null;

    private readonly bool _dependencyInjectionEnabled = false;

    public EventAggregator(Func<Type, object> resolver)

        => (_resolver, _dependencyInjectionEnabled) = (resolver, true);

    public EventAggregator()

        => (_resolver, _dependencyInjectionEnabled) = (null, false);

    private void Unsubscribe(Type eventType, WeakReference<Delegate> handler)
    {
        if (true == _eventSubscriptions.TryGetValue(eventType, out var handlers))
        {
            handlers.Remove(handler);
            if (handlers.Count == 0) _eventSubscriptions.TryRemove(eventType, out _);
        }
    }

    ///<inheritdoc />
    public void Publish<TEvent>(TEvent theEvent, CancellationToken cancellationToken) where TEvent : IEvent
    {
        List<HandleEvent<TEvent>> eventHandlers = [];

        eventHandlers.AddRange(GetSubscribedHandlers<TEvent>());

        if (true == _dependencyInjectionEnabled) eventHandlers.AddRange(GetRegisteredHandlers<TEvent>(cancellationToken));

        FireAndForgetStrategy(theEvent, eventHandlers, cancellationToken);

    }
    private static void FireAndForgetStrategy<TEvent>(TEvent theEvent, List<HandleEvent<TEvent>> eventHandlers, CancellationToken cancellationToken) where TEvent : IEvent
    {
        foreach (var handler in eventHandlers)
        {
            _ = Task.Run(async () => await handler(theEvent, cancellationToken),CancellationToken.None);//Fire and forget
        }
    }

    ///<inheritdoc />
    public EventSubscription Subscribe<TEvent>(HandleEvent<TEvent> handler) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);

        var handlerList     = _eventSubscriptions.GetOrAdd(eventType, _ => []);
        var weakRefHandler  = new WeakReference<Delegate>(handler);
        var comparer        = new WeakReferenceDelegateComparer();

        if (false == handlerList.Any(existing => comparer.Equals(existing, weakRefHandler))) handlerList.Add(weakRefHandler);

        return new EventSubscription(() => Unsubscribe(eventType, weakRefHandler), handler);
    }


    private List<HandleEvent<TEvent>> GetRegisteredHandlers<TEvent>(CancellationToken cancellationToken) where TEvent : IEvent
    {
        List<HandleEvent<TEvent>> registeredHandlers = [];

        var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(typeof(TEvent));

        try
        {

            var enumerableHandlersType = typeof(IEnumerable<>).MakeGenericType(eventHandlerType);
            var domainEventHandlers = _resolver!(enumerableHandlersType) as Array;

            foreach (var eventHandler in domainEventHandlers!)
            {
                HandleEvent<TEvent> handler = (TEvent, _) => ((dynamic)eventHandler).Handle(TEvent, cancellationToken);
                registeredHandlers.Add(handler);
            }

        }
        catch 
        {
            //TODO give some more thought about communicating errors here to the caller without, 
            return registeredHandlers;
        }
        return registeredHandlers;
    }

    private List<HandleEvent<TEvent>> GetSubscribedHandlers<TEvent>() where TEvent : IEvent
    {
        List<HandleEvent<TEvent>> eventHandlers = [];

        List<WeakReference<Delegate>> subscribedHandlers;

        subscribedHandlers = _eventSubscriptions.TryGetValue(typeof(TEvent), out subscribedHandlers!) == true ? [.. subscribedHandlers] : [];

        foreach (var subscribedHandler in subscribedHandlers)
        {
            if ((subscribedHandler.TryGetTarget(out Delegate? target) == true) && (target is HandleEvent<TEvent> handler))
            {
                eventHandlers.Add(handler);
            }
            else { Task.Run(() => Unsubscribe(typeof(TEvent), subscribedHandler)); }
        }

        return eventHandlers;
    }

    internal class WeakReferenceDelegateComparer : IEqualityComparer<WeakReference<Delegate>>
    {
        public bool Equals(WeakReference<Delegate>? x, WeakReference<Delegate>? y)
        {
            if (x == null || y == null) return false;

            if (false == x.TryGetTarget(out Delegate? xTarget) || false == y.TryGetTarget(out Delegate? yTarget)) return false;

            return xTarget == yTarget || xTarget.Equals(yTarget);
        }

        public int GetHashCode(WeakReference<Delegate> obj)
        {
            return obj.TryGetTarget(out Delegate? target) && target != null ? target.GetHashCode() : obj.GetHashCode();
        }
    }
}
