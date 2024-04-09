using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Extensions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.ConsoleDemos.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.StaticData;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Conditions;

public class PredicateConditions(ConditionEngine conditionEngine)
{
    private readonly ConditionEngine _conditionEngine = conditionEngine;

    public async Task RunPredicatesConditions()
    {
        /*
            * PredicateCondition<TContext> as previously seen will likely be the most used condition.
            * Conditions of any type and data contexts can be used together to form any number of boolean conditions.
            * These conditions ultimately form an Abstract Syntax Tree made up of left and right boolean conditions.
            * Currently this library uses the Dynamic Linq Library in order to parse lambda expressions from strings, such as contained within JSON.
            * Due to this usage the lambda predicates are limited by any limitations inherent within this package.
            * Currently the only issue I have come across in normal day to day usage is with enum, but it really is a none issue. 
            * Use ToString() on the enum type to match against the enum value description/text. This also makes the json easier to read.
        */

        var customerCondition = new PredicateCondition<Customer>("CustomerCondition", c => c.CustomerName == "WrongName" || c.CustomerType.ToString() == "Student", "The customer name or customer type must match");
        var customerRule      = new Rule<None>("CustomerRule", None.Value, new ConditionSet<None>("CustomerSet", None.Value, customerCondition));
        var conditionData     = ConditionData.SingleContext(DemoData.GetCustomer(1));

        /*
            * A rule can also be evaluated by using its Evaluate method passing in a delegate that will fetch/create an instance of an evaluator needed to evaluate the condition.
            * public delegate IConditionEvaluator ConditionEvaluatorResolver(string evaluatorTypeName, Type contextType);
            * 
            * In this instance a PredicateEvaluator<Customer> is needed, so instead of adding this rule and caching it with the condition engine and using the engines evaluate method
            * we will use the condition engines  _conditionEngine.GetEvaluatorByName method that matches the ConditionEvaluatorResolver delegate
            * We could also create a local function or delegate to new up a PredicateConditionEvaluator<Customer>();
            * ConditionEvaluatorResolver resolver = (_,_) => new PredicateConditionEvaluator<Customer>();  
         */

        _ = await customerRule.Evaluate(_conditionEngine.EvaluatorResolver, conditionData)
                                .OnSuccess(result => WriteLine($"Predicate with enum as string. Is success: {result.IsSuccess}, data used: {result.ConditionSetChain!.ResultChain!.EvaluationData}"));
                    

        try
        {
            /*
                * A condition set also has an evaluate method, lets use that with a local delegate. 
            */
            ConditionEvaluatorResolver resolver = (_, _) => new PredicateConditionEvaluator<Customer>();

            var enumCondition = new PredicateCondition<Customer>("EnumCondition", c => c.CustomerType == CustomerType.Student, "The customer name or customer type must match");
            
            _ = await new ConditionSet<None>("EnumSet", None.Value, enumCondition).Evaluate(resolver, conditionData);
        }
        catch(Exception ex)
        {
            WriteLine();
            WriteLine($"Using enum directly error: {ex.Message}");
            WriteLine("This is the error we would also receive using enum values directly in the JSON.");
            WriteLine("Our non Json underlying code also stores the expression as a string, before compiling and caching it, hence the exception");
        }

        await Task.CompletedTask;
    }
}
