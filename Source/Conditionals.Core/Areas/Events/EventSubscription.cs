namespace Conditionals.Core.Areas.Events;


/// <summary>
/// An EventSubscription keeps a reference to an event that you have registered to receive.
/// To stop receiving published events you can call the Dispose method. The subscription uses weak references and as such
/// will be automatically removed once the subscription goes out of scope if dispose is not called.
/// </summary>
public class EventSubscription : IDisposable
{
    private readonly Action     _unsubscribe;
    private readonly Delegate   _handler;
    private bool                _disposed = false;
    /*
        * Need to store the actual delegate handler as otherwise due to the weak references there are no references to it and it just gets
        * gc collected and automatically removed from the subscriptions 
    */

    /// <summary>
    /// Initialises a new instance of the <see cref="EndOfStreamException"/> class.
    /// </summary>
    /// <param name="removeAction">A call back that removes the handler subscription from an instance of the <see cref="IEventAggregator"/> class.</param>
    /// <param name="handler">A delegate representing the event handler that was registered with an instance of the <see cref="IEventAggregator"/> class.</param>
    internal EventSubscription(Action removeAction, Delegate handler) => (_unsubscribe, _handler) = (removeAction, handler);

    /// <summary>
    /// Used to remove the association of a registered handler to the subscribed event.
    /// </summary>
    public void Dispose()
    {
        if (false == _disposed)
        {
            _disposed = true;
            _unsubscribe?.Invoke();
        }
    }
}
