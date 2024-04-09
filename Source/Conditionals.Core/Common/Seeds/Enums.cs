namespace Conditionals.Core.Common.Seeds;

/// <summary>
/// Indicates the type of the conditions expression to evaluate property.
/// </summary>
public enum ConditionType : int
{
    /// <summary>
    /// Indicates that the expression is a lambda expression.
    /// </summary>
    LambdaPredicate = 0,
    /// <summary>
    /// Indicates that the expression is a custom expression and uses a custom condition evaluator.
    /// </summary>
    CustomExpression
}

/// <summary>
/// Indicates whether the output string will contain the condition names or the actual condition expression.
/// </summary>
public enum PrecedencePrintType : int
{
    /// <summary>
    /// The condition names will be used in the output.
    /// </summary>
    ConditionNameOnly         = 0,
    /// <summary>
    /// The expression to evaluate will be used in the output.
    /// </summary>
    ExpressionToEvaluateOnly 
}

/// <summary>
/// The type of operator used.
/// </summary>
public enum OperatorType : int
{
    /// <summary>
    /// Uses the short-circuiting AndAlso logic (&amp;&amp;).
    /// </summary>
    AndAlso = 0,
    /// <summary>
    /// Uses the short-circuiting OrElse logic (||).
    /// </summary>
    OrElse
}

/// <summary>
/// Indicates when the event should be raised/published.
/// </summary>
public enum EventWhenType : int
{
    /// <summary>
    /// The event should never be raised.
    /// </summary>
    Never = 0,
    /// <summary>
    /// The event should be raised on either a successful or unsuccessful evaluation.
    /// </summary>
    OnSuccessOrFailure = 1,
    /// <summary>
    /// The event should only be raised on a successful evaluation.
    /// </summary>
    OnSuccess = 2,
    /// <summary>
    /// The event should only be raised on an unsuccessful evaluation.
    /// </summary>
    OnFailure = 3,

}

