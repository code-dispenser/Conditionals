using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using System.Reflection;

namespace Conditionals.Core.Areas.Conditions;

/// <summary>
/// The base class for all conditions that provides the ability to build boolean expressions using the result of 
/// the evaluated conditions.
/// </summary>
public abstract class BooleanConditionBase
{
    /// <summary>
    /// Used to evaluate each boolean condition in the condition tree.
    /// </summary>
    /// <param name="evaluatorResolver">An implementation of the <see cref="ConditionEvaluatorResolver" /> delegate that returns a condition evaluator by name and 
    /// context type <see cref="ConditionEvaluatorResolver" />.</param>
    /// <param name="conditionData">The data for all conditions <see cref="ConditionData" />.</param>
    /// <param name="eventPublisher">Optional, an implementation of the <see cref="EventPublisher"/> EventPublisher delegate for publishing events <see cref="EventPublisher" />.</param>
    /// <param name="previousResult">Optional, the previous instance of a <see cref="ConditionResult" /> class, the default is null</param>
    /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
    /// <returns>An asynchronous operation that returns an instance of the <see cref="ConditionResult "/> class </returns>
    public abstract Task<ConditionResult> Evaluate(ConditionEvaluatorResolver evaluatorResolver, ConditionData conditionData, EventPublisher? eventPublisher = null, ConditionResult? previousResult = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Used to create a boolean expression with left and right condition operands using the AndAlso logic (&amp;&amp;) operator.
    /// </summary>
    /// <param name="condition">The boolean condition to form the right hand side of the boolean expression.</param>
    /// <returns>A new instance of the <see cref="AndAlsoConditions"/> class with the current condition as the left operand and the condition argument as the right operand.</returns>
    public BooleanConditionBase AndAlso(BooleanConditionBase condition)

        => new AndAlsoConditions(this, condition);
    /// <summary>
    /// Used to create a boolean expression with left and right condition operands using the OrElse logic (||) operator.
    /// </summary>
    /// <param name="condition">The boolean condition to form the right hand side of the boolean expression.</param>
    /// <returns>A new new instance of the <see cref="OrElseConditions" /> class with the current condition as the left operand and the condition argument as the right operand.</returns>
    public BooleanConditionBase OrElse(BooleanConditionBase condition)

        => new OrElseConditions(this, condition);

    /// <summary>
    /// Produces a deep clone of the conditions
    /// </summary>
    /// <param name="condition">The condition tree to be cloned.</param>
    /// <returns>A deep clone of the condition tree.</returns>
    /// <exception cref="InvalidBooleanConditionTypeException"></exception>
    public static BooleanConditionBase DeepCloneCondition(BooleanConditionBase condition)

       => condition switch
       {
           AndAlsoConditions andAlso => new AndAlsoConditions(
               DeepCloneCondition(andAlso.Left),
               DeepCloneCondition(andAlso.Right)),

           OrElseConditions orElse => new OrElseConditions(
               DeepCloneCondition(orElse.Left),
               DeepCloneCondition(orElse.Right)),

           ICondition derivedCondition => CreateCondition(derivedCondition),

           _ => throw new InvalidBooleanConditionTypeException(GlobalStrings.Invalid_Boolean_Condition_Type_Exception_Message)
       };

    private static BooleanConditionBase CreateCondition(ICondition derivedCondition)
    {
        BooleanConditionBase instance = default!;

        var derivedType  = typeof(Condition<>).MakeGenericType(derivedCondition.ContextType);
        EventDetails? eventDetails = derivedCondition.EventDetails?.DeepCloneEvent();
        try
        {
            instance = (BooleanConditionBase)Activator.CreateInstance(derivedType, BindingFlags.NonPublic | BindingFlags.Instance, null,
                                                                    [derivedCondition.ConditionName, derivedCondition.ExpressionToEvaluate, derivedCondition.ConditionType, derivedCondition.FailureMessage,
                                                                        derivedCondition.EvaluatorTypeName, derivedCondition.AdditionalInfo.ToDictionary(k => k.Key, v => v.Value), eventDetails
                                                                    ], null)!;
        }
        catch (Exception exception) { throw new InvalidBooleanConditionTypeException(GlobalStrings.Invalid_Boolean_Condition_Type_Exception_Message, exception);}

        return instance!;
    }
}


