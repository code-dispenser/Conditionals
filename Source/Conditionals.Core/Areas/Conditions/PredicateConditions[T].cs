using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Seeds;
using System.Linq.Expressions;

namespace Conditionals.Core.Areas.Conditions;

///<summary>
/// Used to create a condition that uses a compiled lambda expression.
///</summary>
///<inheritdoc cref="Condition{TContext}" />
public sealed class PredicateCondition<TContext>(string conditionName, Expression<Func<TContext, bool>> conditionExpression, string failureMessage, EventDetails? eventDetails = null)
                                                : Condition<TContext>(conditionName, conditionExpression.ToString(), ConditionType.LambdaPredicate, failureMessage, GlobalStrings.Predicate_Condition_Evaluator, [], eventDetails)
{}


///<summary>
/// Used to create a condition that uses a compiled lambda expression, but needs additional information and a custom evaluator in order to be evaluated properly.
///</summary>
///<inheritdoc cref="Condition{TContext}" />
public sealed class CustomPredicateCondition<TContext>(string conditionName, Expression<Func<TContext, bool>> conditionExpression, string failureMessage, string evaluatorTypeName, Dictionary<string, string>? additionalInfo = null, EventDetails? eventDetails = null)
                                                : Condition<TContext>(conditionName, conditionExpression.ToString(), ConditionType.LambdaPredicate, failureMessage, evaluatorTypeName, additionalInfo ?? [], eventDetails)
{ }