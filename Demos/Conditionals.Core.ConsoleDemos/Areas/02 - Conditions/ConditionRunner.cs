using Conditionals.Core.Areas.Engine;
using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Conditions;

public static class ConditionRunner
{
    public static async Task Start(ConditionEngine conditionEngine)
    {
        WriteLineSeparator();
        WriteLine("2a-Predicate-Conditions");
        WriteLine();

        await new PredicateConditions(conditionEngine).RunPredicatesConditions();

        WriteLineSeparator();
        WriteLine("2b-Regex-Conditions");
        WriteLine();

        await new RegexConditions(conditionEngine).RunRegexConditions();

        WriteLineSeparator();
        WriteLine("2c-Custom-Predicate-Conditions");
        WriteLine();

        await new CustomPredicateConditions(conditionEngine).RunCustomPredicateConditions();

        WriteLineSeparator();
        WriteLine("2d-Custom-Conditions");
        WriteLine();

        await new CustomConditions(conditionEngine).RunCustomConditions();

        WriteLineSeparator();
        WriteLine("2e-Boolean-Condition-Precedence");
        WriteLine();

        new BooleanConditionPrecedence().RunConditionPrecedence();
    }
}
