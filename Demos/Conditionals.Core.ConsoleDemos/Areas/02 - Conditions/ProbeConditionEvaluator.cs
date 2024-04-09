using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.Models;

namespace Conditionals.Core.ConsoleDemos.Areas.Conditions;

public class ProbeConditionEvaluator(AppSettings appSettings) : ConditionEvaluatorBase<Probe>
{
    private readonly AppSettings _appSettings = appSettings;

    public override async Task<EvaluationResult> Evaluate(Condition<Probe> condition, Probe data, CancellationToken cancellationToken, string tenantID = "All_Tenants")
    {
        var conditionPassed = false;
        var failureMessage  = String.Empty;

        if (condition.ExpressionToEvaluate.Contains("CalibrationTest"))
        {
            var dictionary = condition.AdditionalInfo;
            //Use defaults if missing
            var minValue    = dictionary.TryGetValue("MinValue", out var min) ? int.Parse(min) : 25;
            var meanValue   = dictionary.TryGetValue("MeanValue", out var mean) ? int.Parse(mean) : 50;
            var maxValue    = dictionary.TryGetValue("MaxValue", out var max) ? int.Parse(max) : 90;

            await Console.Out.WriteLineAsync($"Ran calibration tests using values min: {minValue}, mean: {meanValue}, max: {maxValue}, all ok");
            await Console.Out.WriteLineAsync($"Added the calibration results to database using the connection string {_appSettings.DBWriteConnectionString}");

            conditionPassed = true;
        }
        else
        {
           failureMessage = "Test could not be run, probe flagged as failing";
        }

        return new EvaluationResult(conditionPassed, failureMessage);
    }
}
