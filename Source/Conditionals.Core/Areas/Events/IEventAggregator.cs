using Conditionals.Core.Common.Seeds;

namespace Conditionals.Core.Areas.Events;

/// <summary>
/// Defines the operations required by the Event Aggregator.
/// </summary>
internal interface IEventAggregator
{
    /// <summary>
    /// Publishes an event.
    /// </summary>
    /// <typeparam name="TEvent">The type of event. 
    /// Events are implementations of either the <see cref="RuleEventBase{T}" /> or <see cref="ConditionEventBase{T}" /> abstract classes.</typeparam>
    /// <param name="theEvent">The event to publish.</param>
    /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
    void Publish<TEvent>(TEvent theEvent, CancellationToken cancellationToken) where TEvent : IEvent;

    /// <summary>
    /// Associates a local event handler with an event.
    /// </summary>
    /// <typeparam name="TEvent">The type of event. 
    /// Events are implementations of either the <see cref="RuleEventBase{T}" /> or <see cref="ConditionEventBase{T}" /> abstract classes.</typeparam>
    /// <param name="handler">An implementation of the <see cref="HandleEvent{TEvent}"/> delegate.</param>
    /// <returns>An instance of the <see cref="EventSubscription"/> class that contains an action 
    /// to remove the event association when its <see cref="EventSubscription.Dispose()"/> method is invoked.</returns>
    EventSubscription Subscribe<TEvent>(HandleEvent<TEvent> handler) where TEvent : IEvent;
}
