using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Extensions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.StaticData;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Conditions;

public class CustomPredicateConditions(ConditionEngine conditionEngine)
{
    private readonly ConditionEngine _conditionEngine = conditionEngine;

    public async Task RunCustomPredicateConditions()
    { 
        Dictionary<string,string> additionalData = new() { ["KeyOne"] = "Value One", ["KeyTwo"] = "Value Two" };

        /*
            * Conditions are evaluated using Evaluators, the built-in evaluators have the EvaluatorName property set automatically. For custom conditions you need
            * to assign an Evaluator. For Custom conditions you will be using your own evaluator by implementing either the ConditionEvaluatorBase<TContext> abstract class or the 
            * IConditionEvaluator<TContext> interface which the abstract class inherits. It is preferable to implement the abstract base class as it has a couple of useful methods for 
            * the failure method token replacements instead of you creating your own (which you are free to do).
        */
        var conditions = new CustomPredicateCondition<Customer>("CustomPredicateCondition", c => c.CustomerName == "SomeName", "Expected the Customer name to be \"SomeName\" but the value was @{CustomerName}",
                                                                     "MyCustomPredicateEvaluator", additionalData);

        var rule = new Rule<None>("WithCustomPredicateCondition", None.Value,new ConditionSet<None>("SetOne", None.Value, conditions));

        var conditionData = ConditionData.SingleContext(DemoData.GetCustomer(4));
        /*
            * We need to register our custom evaluator with the condition engine. If we wanted to use dependency injection we would need to register the evaluator with
            * both the condition engine RegisterCustomEvaluatorForDependencyInjection and the IOC container being used.
            * The RegisterCustomEvaluator being used below will check/create and cache evaluators.
            * 
            * We need a unique name that is used to fetch and store the evaluator details in cache. If the evaluator is an open generic we need to register it correctly 
            * i.e typeof(MyCustomPredicateEvaluator<>). If we had closed our evaluator then it would have been typeof(MyCustomPredicateEvaluator) 
            * i.e public class MyCustomPredicateEvaluator : ConditionEvaluatorBase<Customer>.
            *  
            * Missing evaluators get added as an exception to a failed condition result which may or may not fail the rule if the rule has
            * multiple condition sets.
        */

        _conditionEngine.RegisterCustomEvaluator("MyCustomPredicateEvaluator", typeof(MyCustomPredicateEvaluator<>));

        _conditionEngine.AddOrUpdateRule(rule);

        _= await _conditionEngine.EvaluateRule<None>(rule.RuleName, conditionData)
                                .OnFailure(f =>
                                {
                                    WriteLine($"No of evaluations {f.EvaluationCount}, failure count {f.FailureMessages.Count}," +
                                              $"failure messages: {String.Concat("\r\n",String.Join("\r\n",f.FailureMessages), "\r\n")}");
                                });
    }
}
