using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;

namespace Conditionals.Core.Areas.Conditions;

/// <summary>
/// Sealed class that provides the left and right operands for an AndAlso operation. 
/// This is an implementation of the <see cref="BooleanConditionBase"/> abstract class.
/// </summary>
public sealed class AndAlsoConditions : BooleanConditionBase
{
    /// <summary>
    /// Gets the left operand (a condition that will evaluate to true or false).
    /// </summary>
    public BooleanConditionBase Left { get; }
    /// <summary>
    /// Gets the right operand (a condition that will evaluate to true or false).
    /// </summary>
    public BooleanConditionBase Right { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="AndAlsoConditions" /> class.
    /// </summary>
    /// <param name="left">The left <see cref="BooleanConditionBase" /> operand</param>
    /// <param name="right">The right <see cref="BooleanConditionBase" /> operand</param>
    public AndAlsoConditions(BooleanConditionBase left, BooleanConditionBase right)

        => (Left, Right) = (left, right);

    /// <summary>
    /// Evaluates the left operand and if successful (returns true) then evaluates the right.
    /// </summary>
    /// <param name="evaluatorResolver">An implementation of the <see cref="ConditionEvaluatorResolver" /> delegate that returns a condition evaluator by name 
    /// and context type <see cref="ConditionEvaluatorResolver" />.</param>
    /// <param name="conditionData">The data for all conditions <see cref="ConditionData" />.</param>
    /// <param name="eventPublisher">Optional, an implementation of the <see cref="EventPublisher" /> delegate for publishing events.</param>
    /// <param name="previousResult">Optional, the previous <see cref="ConditionResult" />, the default is null.</param>
    /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
    /// <returns>An asynchronous operation that return and instance of the <see cref="ConditionResult" /> class.</returns>
    public override async Task<ConditionResult> Evaluate(ConditionEvaluatorResolver evaluatorResolver, ConditionData conditionData, EventPublisher? eventPublisher = null, ConditionResult? previousResult = null, CancellationToken cancellationToken = default)
    {
        var leftResult = await Left.Evaluate(evaluatorResolver, conditionData, eventPublisher, previousResult, cancellationToken);

        if (leftResult.IsSuccess == true)
        {
            var rightResult = await Right.Evaluate(evaluatorResolver, conditionData, eventPublisher, leftResult, cancellationToken);

            return rightResult;
        }

        return leftResult;
    }
}

/// <summary>
/// Sealed class that provides the left and right operands for an OrElse operation. 
/// This is an implementation of the <see cref="BooleanConditionBase"/> abstract class.
/// </summary>
public sealed class OrElseConditions : BooleanConditionBase
{
    /// <summary>
    /// Gets the left operand (a condition that will evaluate to true or false).
    /// </summary>
    public BooleanConditionBase Left { get; }
    /// <summary>
    /// Gets the right operand (a condition that will evaluate to true or false).
    /// </summary>
    public BooleanConditionBase Right { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="OrElseConditions"/> class.
    /// </summary>
    /// <param name="left">The left <see cref="BooleanConditionBase" />operand</param>
    /// <param name="right">The right <see cref="BooleanConditionBase" />operand</param>
    public OrElseConditions(BooleanConditionBase left, BooleanConditionBase right)

        => (Left, Right) = (left, right);


    /// <summary>
    /// Evaluates the left operand and if unsuccessful (returns false) then evaluates the right.
    /// </summary>
    /// <param name="evaluatorResolver">An implementation of the <see cref="ConditionEvaluatorResolver" /> delegate that returns a condition evaluator by name 
    /// and context type <see cref="ConditionEvaluatorResolver" />.</param>
    /// <param name="conditionData">The data for all conditions <see cref="ConditionData" />.</param>
    /// <param name="eventPublisher">Optional, an implementation of the <see cref="EventPublisher" /> delegate for publishing events.</param>
    /// <param name="previousResult">Optional, the previous <see cref="ConditionResult" />, the default is null.</param>
    /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
    /// <returns>An asynchronous operation that return and instance of the <see cref="ConditionResult" /> class.</returns>
    public override async Task<ConditionResult> Evaluate(ConditionEvaluatorResolver evaluatorResolver, ConditionData conditionData, EventPublisher? eventPublisher = null, ConditionResult? previousResult = null, CancellationToken cancellationToken = default)
    {
        var leftResult = await Left.Evaluate(evaluatorResolver, conditionData, eventPublisher, previousResult, cancellationToken);

        if (leftResult.IsSuccess == false)
        {
            var rightResult = await Right.Evaluate(evaluatorResolver, conditionData, eventPublisher, leftResult, cancellationToken);
            return rightResult;
        }

        return leftResult;
    }

}
