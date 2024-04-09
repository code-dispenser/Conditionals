using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Seeds;
using System.Linq.Expressions;

namespace Conditionals.Core.Areas.Conditions;

///<summary>
/// Used to create a condition that does not use a compiled lambda expression.
///</summary>
///<inheritdoc cref="Condition{TContext}" />
public class CustomCondition<TContext>(string conditionName, string expressionToEvaluate, string failureMessage, string evaluatorTypeName, Dictionary<string, string>? additionalInfo = null, EventDetails? eventDetails = null) 
    : Condition<TContext>(conditionName, expressionToEvaluate, ConditionType.CustomExpression, failureMessage, evaluatorTypeName, additionalInfo ?? [], eventDetails)
{}
