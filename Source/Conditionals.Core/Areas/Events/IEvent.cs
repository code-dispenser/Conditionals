namespace Conditionals.Core.Areas.Events;

/// <summary>
/// Base set of properties shared by both the <see cref="ConditionEventBase{T}" /> and <see cref="RuleEventBase{T}" /> abstract classes.
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Gets the TenantID.
    /// </summary>
    string TenantID     { get; }

    /// <summary>
    /// Gets a boolean value that indicates if the rule or condition that raised the event was successful or unsuccessful.
    /// </summary>
    bool IsSuccessEvent { get; }

    /// <summary>
    /// Get the name of the rule or condition that raised the event.
    /// </summary>
    string SenderName   { get; }
}
