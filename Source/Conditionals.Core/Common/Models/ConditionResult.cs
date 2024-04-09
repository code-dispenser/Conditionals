using Conditionals.Core.Areas.Evaluators;
namespace Conditionals.Core.Common.Models;

/// <summary>
/// Represents the result of a condition evaluation which derives its result from an <see cref="EvaluationResult" />.
/// Initialises a new instance of the <see cref="ConditionResult" /> class.
/// </summary>
/// <param name="conditionName">The name of the condition that was evaluated.</param>
/// <param name="contextType">The type of data used in the evaluation.</param>
/// <param name="expressionToEvaluate">The expression that was evaluated.</param>
/// <param name="evaluationData">The data used for the evaluation.</param>
/// <param name="evaluatedBy">The name of the condition evaluator used to evaluate the condition.</param>
/// <param name="isSuccess">The boolean value indicating success or failure of the evaluation.</param>
/// <param name="failureMessage">The failure message associated with a failed evaluation.</param>
/// <param name="evaluationMicroseconds">The time in microseconds that was taken for the condition evaluation.</param>
/// <param name="totalMicroseconds">The total time in microseconds that it took to process the condition, including the evaluation time.</param>
/// <param name="tenantID">The tenantID value.</param>
/// <param name="previousResult">The previous <see cref="ConditionResult" /> value that forms part of the result chain. The default is null.</param>
/// <param name="exceptions">Gets the list of exceptions that may have occurred.</param>
public class ConditionResult(string conditionName, string contextType, string expressionToEvaluate, object? evaluationData, string evaluatedBy, bool isSuccess,
                             string failureMessage, long evaluationMicroseconds, long totalMicroseconds, string tenantID, ConditionResult? previousResult, List<Exception> exceptions)
{

    /// <summary>
    /// Gets the previous <see cref="ConditionResult"/> in the result chain.
    /// </summary>
    public ConditionResult? ResultChain { get; } = previousResult;

    /// <summary>
    /// Gets the type of data used in the evaluation.
    /// </summary>
    public string ContextType           { get; } = contextType ?? string.Empty;

    /// <summary>
    /// Gets the Tenant ID value that was assigned to the <see cref="ConditionData" /> class.
    /// </summary>
    public string TenantID              { get; } = tenantID ?? string.Empty;

    /// <summary>
    /// Gets the name of the condition that was evaluated.
    /// </summary>
    public string ConditionName         { get; } = conditionName ?? string.Empty;

    /// <summary>
    /// Gets the list of exceptions that may have occurred during the processing of a condition evaluation.
    /// </summary>
    public List<Exception> Exceptions   { get; } = exceptions ?? [];

    /// <summary>
    /// Gets the failure message associated with a failed evaluation, otherwise an empty string.
    /// </summary>
    public string FailureMessage        { get; } = failureMessage ?? string.Empty;

    /// <summary>
    /// Gets a boolean value indicating the success or failure of the evaluation.
    /// </summary>
    public bool IsSuccess               { get; } = isSuccess;

    /// <summary>
    /// Gets the expression that was evaluated.
    /// </summary>
    public string ExpressionToEvaluate  { get; } = expressionToEvaluate ?? string.Empty;

    /// <summary>
    /// Gets the data used for the evaluation.
    /// </summary>
    public object? EvaluationData       { get; } = evaluationData;

    /// <summary>
    /// Gets the name of the condition evaluator used to evaluate the condition.
    /// </summary>
    public string EvaluatedBy           { get; } = evaluatedBy ?? string.Empty;

    /// <summary>
    /// Gets the time in microseconds that was taken for the condition evaluation.
    /// </summary>
    public long EvaluationMicroseconds  { get; } = evaluationMicroseconds;

    /// <summary>
    /// Gets the total time in microseconds that it took to process the condition, including the evaluation time.
    /// </summary>
    public long TotalMicroseconds       { get; } = totalMicroseconds;
}