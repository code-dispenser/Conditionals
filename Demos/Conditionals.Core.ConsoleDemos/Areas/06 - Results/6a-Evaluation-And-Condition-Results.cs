using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.ConsoleDemos.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.StaticData;
using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Results;

public class EvaluationAndConditionResults
{
    public static async Task EvaluatingConditions()
    {
        /*
            * The first result leading to the final RuleResult<T> is an internal EvaluationResult. Every evaluation of a condition returns an evaluation result
            * that holds a boolean IsSuccess property, a FailureMessage (string empty for successful evaluations) and an Exception property, null by default.
            * It is possible that the condition is evaluated successfully but with exceptions i.e you could have your evaluation return an EvaluationResult 
            * with IsSuccess = true, FailureMessage = String.Empty but with an Exception.
            * 
            * The EvaluationResult is used to create the ConditionResult that contains the evaluation results property values as well additional timing and condition information.
        */

        var studentRate = new PredicateCondition<Customer>("IsStudent", c => c.CustomerType.ToString() == "Student", "Customer @{CustomerName} is not a student");

        /*
         *    var conditionData = new ConditionData(new DataContext[]{ new DataContext(DemoData.GetCustomer(1)) });
         * or var conditionData = new ConditionData([new(DemoData.GetCustomer(1))]);
         * or
        */
        var conditionData = ConditionDataBuilder.AddForAny(DemoData.GetCustomer(1)).Create();

        /*
            * Lets use a delegate lambda instead of the condition engine and just new up the evaluator that is used by PredicateConditions.
            * we do not need the name or type as we know them so we can just use discards
        */

        ConditionEvaluatorResolver resolver = (_, _) => new PredicateConditionEvaluator<Customer>();

        var conditionResult = await studentRate.Evaluate(resolver, conditionData);

        WriteLine($"Condition name: {conditionResult.ConditionName}, Is success: {conditionResult.IsSuccess}, Evaluated by: {conditionResult.EvaluatedBy}, (data for) Tenant id: {conditionResult.TenantID}");
        /*
            * The result also contains the data used for the evaluation.
        */
        WriteLine($"Expression to evaluate: {conditionResult.ExpressionToEvaluate}");
        WriteLine($"Data used: {conditionResult.EvaluationData}");
        
        /*
            * As well as the failure message the result also has a List<Exception>. The evaluation result could contain one exception, however,
            * the condition may have an associated event that gets raised after the evaluation result, if there is an error trying to create the event rather
            * that corrupt the processing of the entire rule it adds the exception to its list and continues processing, without altering the result of the condition.
        */
        WriteLine($"Failure message: {(String.IsNullOrWhiteSpace(conditionResult.FailureMessage) ? "String.Empty" : conditionResult.FailureMessage)}, Exceptions count: {conditionResult.Exceptions.Count}");
        /*
            * The result also contains various timings such as the time the evaluation took.
            * The the overall time includes requesting and getting the evaluator via the resolver, numerous checks and raising any events and building the result.
        */

        WriteLine($"The time the evaluation took in microseconds: {conditionResult.EvaluationMicroseconds}");
        WriteLine($"The overall time that the condition took to process in microseconds: {conditionResult.TotalMicroseconds}");
        /*
            * When there are multiple conditions each condition result will contain a link to the result that was processed before it.
            * IsStudent is the first evaluation which is true, its an And operation so the IsPensioner is evaluated which evaluates to false
            * so from the last evaluation to the first is IsPensioner <-- IsStudent
        */

        WriteLine();
        WriteLine("Added another condition to the condition using AndAlso.");
        WriteLine("With multiple conditions the ResultChain property holds the previous condition result:");
        conditionResult = await studentRate.AndAlso(new PredicateCondition<Customer>("IsPensioner", c => c.CustomerType.ToString() == "Pensioner", "Customer @{CustomerName} is not a pensioner"))
                                   .Evaluate(resolver,conditionData);

        WriteLine($"Conditions: ({conditionResult.IsSuccess}) {conditionResult.ConditionName}  <--  ({conditionResult.ResultChain!.IsSuccess}) {conditionResult.ResultChain!.ConditionName}"); 
    }
}
