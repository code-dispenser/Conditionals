using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.FormatConverters;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.ConsoleDemos.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.StaticData;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Contexts
{
    public class SampleOrdersNamedDataContexts(ConditionEngine conditionEngine)
    {
        private readonly ConditionEngine _conditionEngine = conditionEngine;

        public async Task CheckOrders()
        {
            /*
                * @{PROPERTYNAME} is a token than can be replaced with the respective property value for failure messages
            */ 
            var conditions = new PredicateCondition<Order>("Order225", o => o.OrderTotal > 100M, failureMessage: "Order No. @{OrderID} dated @{OrderDate} value @{OrderTotal} was not over 100.00")
                             .OrElse(new PredicateCondition<Order>("Order567", o => o.OrderTotal > 100M, "Order No, @{OrderID} dated @{OrderDate} value @{OrderTotal} was not over 100.00"))
                             .OrElse(new PredicateCondition<Order>("Order723", o => o.OrderTotal > 100M, "Order No, @{OrderID} dated @{OrderDate} value @{OrderTotal} was not over 100.00"));

            var applyDiscountRule = new Rule<decimal>("ApplyDiscountRule", 0.00M, new ConditionSet<decimal>("OrderSet", 0.15M, conditions));

            Order[] sampleOrders = DemoData.SampleOrders.Skip(2).Take(3).ToArray();
            
            /*
                * The AddForCondition adds a DataContext to the DataContext[] that has both its Data and ConditionName properties set.
                * Each condition will query the DataContext array for an item matching its name (case sensitive), taking the first match incase of duplicates.
                * If there is no named data context the condition it will then try to match by data type, again taking the first item if more that one is found.
                * If no data context is found, the condition result is flagged as failed with the exception being added to the result. 
                * Nb. This does not stop the processing of other conditions in any condition sets nor prevent the rule from completing.
            */
            
            var conditionData = ConditionDataBuilder.AddForCondition("Order567", sampleOrders[1])
                                                        .AndForCondition("Order723", sampleOrders[2])
                                                            .AndForCondition("Order225", sampleOrders[0])
                                                                .Create();

            _conditionEngine.AddOrUpdateRule(applyDiscountRule);

            /*
                * If you want to have the EvaluationPrecedence property set on each condition set result then you can pass in an instance of an IConditionPrecedencePrinter
                * I used the built-in one, but you are free to create your own.
                * Each condition set can show the order precedence/nesting of its contained conditions.
                * You can also use an instance of an IConditionPrecedencePrinter to check the order precedence by using its PrintPrecedenceOrder method
                * i.e new ConditionPrecedencePrinter(PrecedencePrintType.ConditionNameOnly).PrintPrecedenceOrder(conditions);
            */

            var ruleResult = await _conditionEngine.EvaluateRule<decimal>(applyDiscountRule.RuleName, conditionData, new ConditionPrecedencePrinter(PrecedencePrintType.ConditionNameOnly));

            WriteLine($"Rule succeeded {ruleResult.IsSuccess}, discount to apply {(ruleResult.IsSuccess ? ruleResult.SetValue : ruleResult.FailureValue) * 100:0'%'}");
            WriteLine($"Evaluation count {ruleResult.EvaluationCount}, {ruleResult.FailureMessages.Count} failures: {String.Concat("\r\n", String.Join("\r\n",ruleResult.FailureMessages), "\r\n")}");
            WriteLine($"Data for successful condition: {ruleResult.ConditionSetChain!.ResultChain!.EvaluationData}"); 
            WriteLine($"Evaluation order: {ruleResult.ConditionSetChain!.EvaluationPrecedence}");

        }
    }
}
