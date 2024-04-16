using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Common.FormatConverters;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.FormatConverters;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Integration.Areas.Conditions;

public class ConditionSetTests
{
    [Theory]
    [InlineData(PrecedencePrintType.ConditionNameOnly)]
    [InlineData(PrecedencePrintType.ExpressionToEvaluateOnly)]
    public async Task The_returned_condition_set_result_should_contain_the_evaluation_precedence_if_the_precedence_printer_is_not_null(PrecedencePrintType printType)
    {
        var customConditionOne = new PredicateCondition<Customer>("CustomerName", c => c.CustomerName == "CustomerOne", "The customer name should be CustomerOne");
        var customConditionTwo = new PredicateCondition<Supplier>("SupplierNo", s => s.SupplierNo == 111, "The supplier number should be 111");
        var precedencePrinter  = new ConditionPrecedencePrinter(printType);
        
        var condition         = customConditionOne.AndAlso(customConditionTwo);
        var conditionSet      = new ConditionSet<None>("SetName",None.Value,condition);
        var conditionData     = new ConditionData([new DataContext(StaticData.CustomerOne()),new DataContext(StaticData.SupplierOne())]);

        static IConditionEvaluator resolver(string name, Type type)
        {
            var evaluatorType = typeof(PredicateConditionEvaluator<>).MakeGenericType(type);
            return (IConditionEvaluator)Activator.CreateInstance(evaluatorType)!;
        }

        var conditionSetResult = await conditionSet.Evaluate(resolver, conditionData, null, precedencePrinter);

        if (printType == PrecedencePrintType.ConditionNameOnly)
        {
            conditionSetResult.EvaluationPrecedence.Should().Be("(CustomerName AndAlso SupplierNo)");
        }

        if (printType == PrecedencePrintType.ExpressionToEvaluateOnly)
        {
            conditionSetResult.EvaluationPrecedence.Should().Be("(c => (c.CustomerName == \"CustomerOne\") AndAlso s => (s.SupplierNo == 111))");
        }
    }

    [Fact]
    public async Task The_returned_condition_set_result_should_contain_a_not_available_precedence_string_if_the_precedence_printer_null()
    {
        var customConditionOne = new PredicateCondition<Customer>("CustomerName", c => c.CustomerName == "CustomerOne", "The customer name should be CustomerOne");

        var precedencePrinter  = new ConditionPrecedencePrinter(PrecedencePrintType.ConditionNameOnly);
        var conditionSet = new ConditionSet<None>("SetName", None.Value, customConditionOne);
        var conditionData = new ConditionData([new DataContext(StaticData.CustomerOne())]);

        static IConditionEvaluator resolver(string name, Type type)
        {
            var evaluatorType = typeof(PredicateConditionEvaluator<>).MakeGenericType(type);
            return (IConditionEvaluator)Activator.CreateInstance(evaluatorType)!;
        }

        var conditionSetResult = await conditionSet.Evaluate(resolver, conditionData, null, null);

        conditionSetResult.EvaluationPrecedence.Should().Be(GlobalStrings.Not_Available_Text);
    }
    [Fact]
    public async Task The_returned_condition_set_result_should_contain_the_precedence_string_containing_an_exception_message_if_the_precedence_printer_errors()
    {
        var customConditionOne = new PredicateCondition<Customer>("CustomerName", c => c.CustomerName == "CustomerOne", "The customer name should be CustomerOne");

        var precedencePrinter   = new ConditionPrecedencePrinter(PrecedencePrintType.ConditionNameOnly);
        var conditionSet        = new ConditionSet<None>("SetName", None.Value, customConditionOne);
        var conditionData       = new ConditionData([new DataContext(StaticData.CustomerOne())]);

        static IConditionEvaluator resolver(string name, Type type)
        {
            var evaluatorType = typeof(PredicateConditionEvaluator<>).MakeGenericType(type);
            return (IConditionEvaluator)Activator.CreateInstance(evaluatorType)!;
        }

        var conditionSetResult = await conditionSet.Evaluate(resolver, conditionData, null, new ErroneousPrecedencePrinter());

        conditionSetResult.EvaluationPrecedence.Should().Be("Error trying to create the precedence string using the precedence printer: 'ErroneousPrecedencePrinter'. Exception message: The method or operation is not implemented.");
    }

    [Fact]
    public async Task The_returned_condition_set_result_should_contain_any_condition_evaluation_exceptions()
    {
        var customConditionOne  = new CustomPredicateCondition<Customer>("CustomerName", c => c.CustomerName == "CustomerOne", "The customer name should be CustomerOne","Non-existent evaluator");

        var precedencePrinter   = new ConditionPrecedencePrinter(PrecedencePrintType.ConditionNameOnly);
        var conditionSet        = new ConditionSet<None>("SetName", None.Value, customConditionOne);
        var conditionData       = new ConditionData([new DataContext(StaticData.CustomerOne())]);

        static IConditionEvaluator resolver(string name, Type type)
            => name switch
            {
                GlobalStrings.Predicate_Condition_Evaluator => (IConditionEvaluator)Activator.CreateInstance(typeof(PredicateConditionEvaluator<>).MakeGenericType(type))!,
                _ => null!
            };

        var conditionSetResult = await conditionSet.Evaluate(resolver, conditionData);

        conditionSetResult.Exceptions.Count.Should().Be(1);
    }


}
