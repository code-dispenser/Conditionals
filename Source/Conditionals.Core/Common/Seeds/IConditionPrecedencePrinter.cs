using Conditionals.Core.Areas.Conditions;

namespace Conditionals.Core.Common.Seeds;


/// <summary>Provides the ability to create a string representation of the order of precedence in the evaluation of the condition tree.</summary>
public interface IConditionPrecedencePrinter
{
    /// <summary>
    /// Creates a string representation of the conditions evaluation order of precedence 
    /// </summary>
    /// <param name="booleanCondition">The condition that implements <see cref="BooleanConditionBase"/> that holds the condition tree.</param>
    /// <returns>The string representation of the order of precedence.</returns>
    string PrintPrecedenceOrder(BooleanConditionBase booleanCondition);
}