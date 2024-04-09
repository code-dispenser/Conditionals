using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.StaticData;

namespace Conditionals.Core.ConsoleDemos.Areas.Conditions;

public class CustomConditions(ConditionEngine conditionEngine)
{
    private readonly ConditionEngine _conditionEngine = conditionEngine;

    public async Task RunCustomConditions()
    {
        /*
            * Custom conditions allow you to specify free text in the ExpressionToEvaluate property in a format that your custom evaluators understands.
            * You can use this and the Dictionary<string,string> AdditionalInfo property to pass instructions to your evaluator.
            * 
            * Please see the code in the ProbeConditionEvaluator.cs file 
        */
        Dictionary<string, string> additionalInfo = new() { ["MeanValue"]="50", ["MinValue"]="20", ["MaxValue"]="80" };

        var probeCondition  = new CustomCondition<Probe>("ProbeValueCondition", "CalibrationTest", "Probe outside of expected norm", nameof(ProbeConditionEvaluator), additionalInfo);
        var probeRule       = new Rule<None>("ProbeRule", None.Value, new ConditionSet<None>("ProbeSet", None.Value, probeCondition));
        
        var conditionData   = ConditionData.SingleContext(DemoData.GetTenantDevice(1).Probes[0]);

        /*
            * The ProbeConditionEvaluator has been registered in the IOC containers, see the Program.cs file
            * We now just need to register it with the condition engine so it knows to get instances from the IOC container.
            * 
            * The RegisterCustomEvaluatorForDependencyInjection only caches the name and type. The condition engine calls the IOC container
            * Func, passing the type each time an instance is required.
            * 
            * Missing evaluators get added as an exception to a failed condition result which may or may not fail the rule if the rule has
            * multiple condition sets

        */
        _conditionEngine.RegisterCustomEvaluatorForDependencyInjection(nameof(ProbeConditionEvaluator), typeof(ProbeConditionEvaluator));

        _conditionEngine.AddOrUpdateRule(probeRule);
        
        /*
            * Messages should be printed to screen from the ProbeConditionEvaluator during its evaluation of the condition. 
        */ 
        var result = await _conditionEngine.EvaluateRule<None>(probeRule.RuleName, conditionData);

    }

}
