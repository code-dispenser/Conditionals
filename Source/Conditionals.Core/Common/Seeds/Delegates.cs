using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Areas.Events;

namespace Conditionals.Core.Common.Seeds;


/// <summary>
/// Delegate used for fetching an implementation of an <see cref="IConditionEvaluator" />
/// </summary>
/// <param name="evaluatorTypeName">The name used to identify the evaluator,</param>
/// <param name="contextType">The data type of the condition to be evaluated.</param>
/// <returns>An implementation of an <see cref="IConditionEvaluator" /></returns>
public delegate IConditionEvaluator ConditionEvaluatorResolver(string evaluatorTypeName, Type contextType);


/// <summary>
/// Delegate used for assigning the method used to publish events.
/// </summary>
/// <param name="eventToPublish">The type of event to publish.</param>
/// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
public delegate void EventPublisher(IEvent eventToPublish, CancellationToken cancellationToken);


/// <summary>
/// A delegate used for methods that handle events.
/// </summary>
/// <typeparam name="TEvent">The type of event. This will be an event that implements either <see cref="ConditionEventBase{T}" /> or <see cref="RuleEventBase{T}" /></typeparam>
/// <param name="theEvent">The type of event to be handled.</param>
/// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
/// <returns>Asynchronous operation.</returns>
public delegate Task HandleEvent<TEvent>(TEvent theEvent, CancellationToken cancellationToken) where TEvent : IEvent;


