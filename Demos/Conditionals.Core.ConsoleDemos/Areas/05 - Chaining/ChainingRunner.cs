using Conditionals.Core.Areas.Engine;
using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Chaining;

public static class ChainingRunner
{
    public static async Task Start(ConditionEngine conditionEngine)
    {
        WriteLineSeparator();
        WriteLine("5a-Chaining-Rule-Results");
        WriteLine();

        await new ChainingRuleResults(conditionEngine).RunChainingResults();
    }
}
