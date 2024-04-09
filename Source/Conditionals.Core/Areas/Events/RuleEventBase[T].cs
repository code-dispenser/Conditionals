namespace Conditionals.Core.Areas.Events;

/// <summary>
/// Base class used for rule events.
/// Initialises the implementation of the <see cref="RuleEventBase{T} "/> abstract class.
/// </summary>
/// <typeparam name="T">The type of event that will either be an implementation of <see cref="RuleEventBase{T}"/> or <see cref="ConditionEventBase{T}"/></typeparam>
/// <param name="senderName">The name of the rule that raised the event.</param>
/// <param name="isSuccessEvent">A boolean indicating whether the evaluation of the rule was successful or unsuccessful.</param>
/// <param name="successValue">The success value.</param>
/// <param name="failureValue">The failure value.</param>
/// <param name="tenantID">The tenantID of the data used in the evaluation of the condition.</param>
/// <param name="executionExceptions">A list that contains any exceptions that may have occurred during the processing of the rule.</param>
/// /// <inheritdoc cref="IEvent" />
public abstract class RuleEventBase<T>(string senderName, bool isSuccessEvent, T successValue, T failureValue, string tenantID, List<Exception> executionExceptions) : IEvent
{
    ///<inheritdoc />
    public string TenantID      { get; } = tenantID;

    ///<inheritdoc />
    public string SenderName    { get; } = senderName;

    ///<inheritdoc />
    public bool IsSuccessEvent  { get; } = isSuccessEvent;
    
    /// <summary>
    /// Gets the value associated with a rule that evaluates to true.
    /// </summary>
    public T SuccessValue       { get; } = successValue;

    /// <summary>
    /// Get the value associated with a rule that evaluates to false.
    /// </summary>
    public T FailureValue       { get; } = failureValue;

    /// <summary>
    /// Gets the list of exceptions that may have occurred at any point during the evaluation of rule this includes propagating any exceptions that may have occurred during 
    /// condition evaluations.
    /// </summary>
    public List<Exception> ExecutionExceptions { get; } = executionExceptions ?? [];
}
