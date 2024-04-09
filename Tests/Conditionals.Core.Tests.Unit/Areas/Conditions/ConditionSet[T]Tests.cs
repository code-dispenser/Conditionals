using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Conditions;

public class ConditionSetTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_throw_an_argument_exception_if_the_set_name_is_null_empty_or_whitespace(string? setName)

      => FluentActions.Invoking(() => new ConditionSet<string>(setName!, "SetValue", new PredicateCondition<Customer>("ConditionName", c => c.CustomerName == "CustomerOne", "FailureMessage")))
                      .Should().ThrowExactly<ArgumentException>();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_throw_an_argument_exception_if_the_set_value_is_null_empty_or_whitespace(string? setValue)

        => FluentActions.Invoking(() => new ConditionSet<string>("SetName", setValue!, new PredicateCondition<Customer>("ConditionName", c => c.CustomerName == "CustomerOne", "FailureMessage")))
                        .Should().ThrowExactly<ArgumentException>();

    [Fact]

    public void Should_throw_an_argument_exception_if_the_boolean_condition_is_null()

        => FluentActions.Invoking(() => new ConditionSet<string>("SetName", "SetValue", null!))
                        .Should().ThrowExactly<ArgumentException>();

    [Fact]
    public async Task The_evaluate_method_should_throw_a_missing_all_condition_data_exception_if_the_condition_data_object_is_null()
    {
        var conditionSet = new ConditionSet<int>("SetOne", 42, new PredicateCondition<Customer>("ConditionName", c => c.CustomerName == "CustomerOne", "Customer name should be CustomerOne"));

        static IConditionEvaluator FakeEvaluatorResolver(string evaluatorTypeName, Type contextType)
        {
            return null!;
        }


        await FluentActions.Invoking(() => conditionSet.Evaluate(FakeEvaluatorResolver, null!)).Should().ThrowExactlyAsync<MissingAllConditionDataException>();
    }
    
    [Fact]
    public async Task The_evaluate_method_should_throw_a_missing_evaluator_resolver_exception_is_the_resolver_is_null()
    {
        var conditionData   = ConditionDataBuilder.AddForAny(StaticData.CustomerOne()).Create();
        var conditionSet    = new ConditionSet<int>("SetOne", 42, new PredicateCondition<Customer>("ConditionName", c => c.CustomerName == "CustomerOne", "Customer name should be CustomerOne"));

        await FluentActions.Invoking(() => conditionSet.Evaluate(null!, conditionData)).Should().ThrowExactlyAsync<MissingEvaluatorResolverException>();
    }


}
