using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Common.Models;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Conditions;

public class MyCustomPredicateEvaluator<TContext> : ConditionEvaluatorBase<TContext>
{
    public MyCustomPredicateEvaluator()
    {
        /*
            * We could register this with a IOC container and use Dependency injection
            * This evaluator is an Open Generic but we could have also closed it i.e MyCustomPredicateEvaluator : ConditionEvaluatorBase<Customer>
            * Closing it would allow working with the correct data type instead of working with the generic TContext.
        */
    }
    public override async Task<EvaluationResult> Evaluate(Condition<TContext> condition, TContext data, CancellationToken cancellationToken, string tenantID = "All_Tenants")
    {
        /*
            * Each condition has a ConditionType property (enum). ConditionType.LambdaPredicate means the condition has a compiled predicate. 
            * In this instance we used the CustomPredicateCondition which automatically sets the ConditionType to LambdaPredicate. 
            * CustomCondition sets the ConditionType to CustomExpression and as such the CompiledPredicate property is null.
         */

        /*
            * You can access any additional information needed for the evaluator that you may have added to the Dictionary<string,string> AdditionalInfo property of the condition. 
        */ 
        var valueOne = condition.AdditionalInfo.TryGetValue("KeyOne", out var keyValue) ? keyValue : "[null]";

        var isSuccess = condition.CompiledPredicate!(data);

        /*
            * To keep to conventions the failureMessage should be empty for a passing evaluation
            * You can either use the base method to build the failure message as it may contain replacement tokens or create your own.
            * The MessageRegex is a compiled regex again you can provide your own.
            * The last parameter of the BuildFailureMessage is optional and defaults to "N/A" and is used if the property is missing or the property value is null
            * or there was a problem
        */ 

        var failureMessage = isSuccess ? String.Empty : BuildFailureMessage(condition.FailureMessage, data!, MessageRegex, "[N/A]");

        /*
            * You should also wrap problematic code in a try catch and assign the exception to the evaluation result exception property.
            * It may be that you encounter some error, correct it, return a passing evaluation result but with the exception property set.
            * Other code could check for exceptions in passing results and take some action/send a notification etc. The choice is yours.
        */

        WriteLine("Just about to return the evaluation result from the custom predicate evaluator");
        WriteLine($"The addition info dictionary had a value for KeyOne of: {valueOne}");
        WriteLine();

        return await Task.FromResult(new EvaluationResult(isSuccess, failureMessage, Exception: null)); 
    }
}
