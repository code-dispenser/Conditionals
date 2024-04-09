using Conditionals.Core.Areas.Engine;
using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Results;

public static class ResultsRunner
{
    public static async Task Start(ConditionEngine conditionEngine)
    {
        WriteLineSeparator();
        WriteLine("6a-Evaluation-And-Condition-Results");
        WriteLine();

        await EvaluationAndConditionResults.EvaluatingConditions();

        WriteLineSeparator();
        WriteLine("6b-ConditionSet-Results");
        WriteLine();
        await new ConditionSetResults(conditionEngine).CollatingSetResult();


        WriteLineSeparator();
        WriteLine("6c-Rule-Results");
        WriteLine();
        await new RuleResults(conditionEngine).ReturningResults();
    }
}
