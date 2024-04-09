using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Evaluators;

public class PredicateConditionEvaluatorTests
{
    [Fact]
    public async Task Should_throw_a_predicate_condition_compilation_exception_if_the_condition_type_is_not_a_lambda_type()
    {
        var customCondition = new CustomCondition<Customer>("CustomCondition", "Some expression", "Failure message", "PredicateConditionEvaluator");
        var conditionData   = ConditionDataBuilder.AddForAny(StaticData.CustomerOne()).Create();

        var theResult = await customCondition.Evaluate(PredicateConditionEvaluatorTests.GetEvaluator, conditionData);

        theResult.Should().Match<ConditionResult>(r => r.Exceptions[0].GetType() == typeof(PredicateConditionCompilationException));
        
    }

    [Fact]
    public async Task The_build_failure_message_in_the_base_should_do_any_message_token_replacements()
    {
        var condition       = new PredicateCondition<Customer>("ConditionOne", c => c.CustomerName == "WrongName", "The customer name was @{CustomerName} but expected WrongName");
        ConditionData data  = new([new(StaticData.CustomerOne())]);

        var result = await condition.Evaluate(PredicateConditionEvaluatorTests.GetEvaluator, data);

        result.FailureMessage.Should().Be("The customer name was CustomerOne but expected WrongName");

    }
    private static IConditionEvaluator GetEvaluator(string name, Type type)

        => name switch
        {
            GlobalStrings.Predicate_Condition_Evaluator => (IConditionEvaluator)Activator.CreateInstance(typeof(PredicateConditionEvaluator<>).MakeGenericType(type))!,
            _ => null!
        };
}
