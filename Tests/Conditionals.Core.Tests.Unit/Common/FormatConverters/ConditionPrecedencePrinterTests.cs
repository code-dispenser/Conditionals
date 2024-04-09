using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Common.FormatConverters;
using Conditionals.Core.Common.Seeds;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Common.FormatConverters;

public class ConditionPrecedencePrinterTests
{
    [Fact]
    public void Should_create_a_string_that_represents_the_correct_order_of_precedence_of_boolean_expressions_showing_the_condition_names()
    {
        var customConditionOne      = new CustomCondition<bool>("ConditionOne", "x && y", "Should be true", "CustomEvaluator");
        var customConditionTwo      = new CustomCondition<bool>("ConditionTwo", "a + b = c", "Should be 42", "CustomEvaluator");
        var customConditionThree    = new CustomCondition<bool>("ConditionThree", "true", "Should be true", "CustomEvaluator");
        var customConditionFour     = new CustomCondition<bool>("ConditionFour", "false", "Should be false", "CustomEvaluator");

        var condition = customConditionOne.AndAlso(customConditionTwo).AndAlso(customConditionThree).OrElse(customConditionFour);

        var printer = new ConditionPrecedencePrinter(PrecedencePrintType.ConditionNameOnly);

        var precedenceOrder = printer.PrintPrecedenceOrder(condition);

        precedenceOrder.Should().Be("(((ConditionOne AndAlso ConditionTwo) AndAlso ConditionThree) OrElse ConditionFour)");
    }
    [Fact]
    public void Should_create_a_string_that_represents_the_correct_order_of_precedence_of_boolean_expressions_showing_the_expressions()
    {
        var customConditionOne      = new CustomCondition<bool>("ConditionOne", "x && y", "Should be true", "CustomEvaluator");
        var customConditionTwo      = new CustomCondition<bool>("ConditionTwo", "a + b = c", "Should be 42", "CustomEvaluator");
        var customConditionThree    = new CustomCondition<bool>("ConditionThree", "some expression", "Should be true", "CustomEvaluator");
        var customConditionFour     = new CustomCondition<bool>("ConditionFour", "x => x", "Should be false", "PredicateConditionEvaluator");

        var condition = customConditionOne.AndAlso(customConditionTwo).AndAlso(customConditionThree).OrElse(customConditionFour);

        var printer = new ConditionPrecedencePrinter(PrecedencePrintType.ExpressionToEvaluateOnly);

        var precedenceOrder = printer.PrintPrecedenceOrder(condition);

        precedenceOrder.Should().Be("(((x && y AndAlso a + b = c) AndAlso some expression) OrElse x => x)");
    }

    [Fact]
    public void Should_throw_argument_null_exception_if_the_condition_cannot_be_cast_to_the_condition_interface()
    {
        var customConditionOne = new CustomCondition<bool>("ConditionOne", "x && y", "Should be true", "CustomEvaluator");

        var condition   = customConditionOne.AndAlso(null!);
        var printer     = new ConditionPrecedencePrinter(PrecedencePrintType.ConditionNameOnly);

        FluentActions.Invoking(() => printer.PrintPrecedenceOrder(condition)).Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Should_return_an_empty_string_if_the_input_condition_is_null()
    {
        var printer = new ConditionPrecedencePrinter(PrecedencePrintType.ConditionNameOnly);

        var precedenceOrder = printer.PrintPrecedenceOrder(null!);
        
        precedenceOrder.Should().BeEmpty();
    }
}
