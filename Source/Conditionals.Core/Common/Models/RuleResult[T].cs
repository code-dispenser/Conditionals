using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Common.Validation;

namespace Conditionals.Core.Common.Models;

/// <summary>
/// Represents the overall outcome of the evaluation of all conditions within one or more condition sets.
/// Initialises a new instance of the <see cref="RuleResult{T}" /> class.
/// </summary>
/// <typeparam name="T">The data type of the set and failure values.</typeparam>
/// <param name="ruleName">The name of the rule.</param>
/// <param name="isSuccess">A boolean indicating the success or failure of the rule.</param>
/// <param name="failureValue">The failure value assigned by the rule.</param>
/// <param name="finalSetName">The name of the last condition set that was evaluated.</param>
/// <param name="setValue">The set value from the last evaluated condition set.</param>
/// <param name="tenantID">The tenantID value that was assigned to the rule.</param>
/// <param name="ruleTimeMicroseconds">The total time in microseconds that was taken to evaluate the rule.</param>
/// <param name="evaluationCount">The total number of conditions evaluated.</param>
/// <param name="isDisabled">A boolean indicating if the rule is disabled.</param>
/// <param name="failureMessages">A list of failure messages from all failed condition evaluations.</param>
/// <param name="exceptions">A list of any exceptions that occurred during the processing of the rule, condition sets and conditions.</param>
/// <param name="setResultChain">The chain of <see cref="ConditionSetResult{T} "/> instances.</param>
/// <param name="previousRuleResult">The previous <see cref="RuleResult{T}" /> instance. Used when chaining rules, the default is null.</param>
public class RuleResult<T> (string ruleName, bool isSuccess, T failureValue, string finalSetName, T setValue, string tenantID, long ruleTimeMicroseconds, int evaluationCount,
                           bool isDisabled, List<string> failureMessages, List<Exception> exceptions, ConditionSetResult<T>? setResultChain,  RuleResult<T>? previousRuleResult = null) 
{
    /// <summary>
    /// Gets or sets the previous <see cref="RuleResult{T}"/> in the result chain.
    /// </summary>
    public RuleResult<T>? PreviousRuleResult        { get; internal set; } = previousRuleResult;

    /// <summary>
    /// Gets the name of the rule.
    /// </summary>
    public string RuleName                          { get; } = Check.ThrowIfNullEmptyOrWhitespace(ruleName);

    /// <summary>
    /// Gets the tenantID value that was assigned to the rule.
    /// </summary>
    public string TenantID                          { get; } = String.IsNullOrWhiteSpace(tenantID) ? GlobalStrings.Default_TenantID : tenantID;

    /// <summary>
    /// Gets the set value from the last evaluated condition set.
    /// </summary>
    public T SetValue                               { get; } = setValue;

    /// <summary>
    /// Gets the name of the last condition set that was evaluated.
    /// </summary>
    public string FinalSetName                      { get; } = String.IsNullOrWhiteSpace(finalSetName) ? String.Empty : finalSetName;

    /// <summary>
    /// Gets the failure value assigned by the rule.
    /// </summary>
    public T FailureValue                           { get; } = Check.ThrowIfNullEmptyOrWhitespace(failureValue);

    /// <summary>
    /// Gets the chain of <see cref="ConditionSetResult{T}"/> instances.
    /// </summary>
    public ConditionSetResult<T>? SetResultChain { get;  internal set; } = setResultChain;

    /// <summary>
    /// Gets a boolean indicating the success or failure of the rule.
    /// </summary>
    public bool IsSuccess                           { get; } = isSuccess;

    /// <summary>
    /// Gets a boolean indicating if the rule is disabled.
    /// </summary>
    public bool IsDisabled                          { get; } = isDisabled;

    /// <summary>
    /// Gets a list of failure messages from all failed condition evaluations.
    /// </summary>
    public List<string> FailureMessages             { get; } = failureMessages  ?? [];

    /// <summary>
    /// Gets a list of any exceptions that occurred during the processing of the rule, condition sets and conditions.
    /// </summary>
    public List<Exception> Exceptions               { get; } = exceptions       ?? [];

    /// <summary>
    /// Gets the total time in microseconds that was taken to evaluate the rule.
    /// </summary>
    public long RuleTimeMicroseconds                { get; } = ruleTimeMicroseconds;

    /// <summary>
    /// Gets the total number of conditions evaluated.
    /// </summary>
    public int EvaluationCount                      { get; } = evaluationCount;

    /// <summary>
    /// Gets the total time in milliseconds that was taken to evaluate the rule.
    /// </summary>
    public long RuleTimeMilliseconds => RuleTimeMicroseconds / 1000;

    /// <summary>
    /// Gets the result value based on the success or failure of the rule.
    /// </summary>
    public T ResultValue             => IsSuccess ? SetValue : FailureValue;

}
