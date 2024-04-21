using Conditionals.Core.Areas.Rules;

namespace Conditionals.Core.Common.Models;

/// <summary>
/// Represents the overall outcome of one or more condition evaluations.
/// Initialises a new instance of the <see cref="ConditionSetResult{T}" /> class.
/// </summary>
/// <typeparam name="T">The data type of the set value that can be assigned to a <see cref="RuleResult{T}" /> instance.</typeparam>
/// <param name="setName">The name of the condition set.</param>
/// <param name="setValue">The sets value.</param>
/// <param name="isSuccess">The boolean value indicating success or failure of the condition set.</param>
/// <param name="totalEvaluations">The total number of condition evaluation performed.</param>
/// <param name="totalMicroseconds">The total time in microseconds that was taken to evaluate all conditions.</param>
/// <param name="evaluationPrecedence">A string representation of the order of precedence.</param>
/// <param name="resultChain">A chain of <see cref="ConditionResult" /> instances.</param>
/// <param name="failureMessages">A list of failure messages from all failed condition evaluations.</param>
/// <param name="exceptions">A list of any exceptions that occurred during the processing of the condition set and its condition tree.</param>
/// <param name="previousSetResult">The previous <see cref="ConditionSetResult{T}"/> value that forms a chain of results. The default is null.</param>
public class ConditionSetResult<T>(string setName, T setValue, bool isSuccess, int totalEvaluations, long totalMicroseconds, string evaluationPrecedence, ConditionResult? resultChain, 
                                   List<string> failureMessages, List<Exception> exceptions, ConditionSetResult<T>? previousSetResult = null)
{

    /// <summary>
    /// Gets the previous <see cref="ConditionSetResult{T}"/> in the result chain.
    /// </summary>
    public ConditionSetResult<T>? PreviousSetResult { get; internal set; } = previousSetResult;

    /// <summary>
    /// Gets the chain of <see cref="ConditionResult"/> instances.
    /// </summary>
    public ConditionResult? ResultChain             { get; } = resultChain;

    /// <summary>
    /// Gets the condition sets value.
    /// </summary>
    public T SetValue                               { get; } = setValue;

    /// <summary>
    /// Gets the name of the condition set.
    /// </summary>
    public string SetName                           { get; } = setName;

    /// <summary>
    /// Gets a boolean value indicating success or failure of the condition set.
    /// </summary>
    public bool IsSuccess                           { get; } = isSuccess;

    /// <summary>
    /// Gets the total number of condition evaluation performed.
    /// </summary>
    public int TotalEvaluations                     { get; } = totalEvaluations;

    /// <summary>
    /// Gets the total time in microseconds that was taken to evaluate all conditions.
    /// </summary>
    public long TotalMicroseconds                   { get; } = totalMicroseconds;

    /// <summary>
    /// Gets a string representation of the order of precedence.
    /// </summary>
    public string EvaluationPrecedence              { get; } = String.IsNullOrWhiteSpace(evaluationPrecedence) ? string.Empty : evaluationPrecedence;

    /// <summary>
    /// Gets the total time in milliseconds that was taken to evaluate all conditions.
    /// </summary>
    public long TotalMilliseconds                   => TotalMicroseconds / 1000;

    /// <summary>
    /// Gets a list of failure messages from all failed condition evaluations.
    /// </summary>
    public List<string>     FailureMessages         { get; } = failureMessages ?? [];

    /// <summary>
    /// Gets a list of any exceptions that occurred during the processing of the condition set and its condition tree.
    /// </summary>
    public List<Exception>  Exceptions     { get; } = exceptions ?? [];

}
