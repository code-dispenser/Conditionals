namespace Conditionals.Core.Areas.Events;

/// <summary>
/// Represents a handler for a specific type of event.
/// Events will be implementations of either the <see cref="ConditionEventBase{T}" /> or <see cref="RuleEventBase{T}" /> abstract classes.
/// </summary>
/// <typeparam name="TEvent">The type of event handled by this handler.</typeparam>
public interface IEventHandler<TEvent> where TEvent : IEvent
{
    /// <summary>
    /// Handles the specified event asynchronously.
    /// </summary>
    /// <param name="theEvent">The event to be handled.
    /// Events will be implementations of either the <see cref="ConditionEventBase{T}" /> or <see cref="RuleEventBase{T}" /> abstract classes.
    /// </param>
    /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Handle(TEvent theEvent, CancellationToken cancellationToken = default);
}
