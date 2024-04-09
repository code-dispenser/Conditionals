using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Conditions;

public class AndAlsoConditionsTests
{
    [Fact]
    public async Task Should_short_circuit_and_return_the_left_condition_result_on_a_failed_evaluation()
    {
        var andCondition = new AndAlsoConditions(new PredicateCondition<Person>("ConditionOne", p => p.Age == 100, "Person needs to be 100 years old to get a discount at this store"),
                                                 new PredicateCondition<Person>("ConditionTwo", p => p.FirstName == "John", "The person should be called John"));

        var conditionData = ConditionDataBuilder.AddForAny(StaticData.PersonUnder18()).Create();
        ConditionEvaluatorResolver resolver = (_, _) => new PredicateConditionEvaluator<Person>();


        var result = await andCondition.Evaluate(resolver, conditionData);

        result.Should().Match<ConditionResult>(r => r.ConditionName == "ConditionOne" && r.IsSuccess == false && r.ResultChain == null);
    }

    [Fact]
    public async Task Should_evaluate_the_right_condition_if_the_left_condition_passes()
    {
        var andCondition = new AndAlsoConditions(new PredicateCondition<Person>("ConditionOne", p => p.Age < 18, "Person needs to be under 18"),
                                                 new PredicateCondition<Person>("ConditionTwo", p => p.FirstName == "John", "The person should be called John"));

        var conditionData = ConditionDataBuilder.AddForAny(StaticData.PersonUnder18()).Create();
        ConditionEvaluatorResolver resolver = (_, _) => new PredicateConditionEvaluator<Person>();


        var result = await andCondition.Evaluate(resolver, conditionData);

        result.Should().Match<ConditionResult>(r => r.ConditionName == "ConditionTwo" && r.IsSuccess == true && r.ResultChain!.ConditionName == "ConditionOne" && r.ResultChain.IsSuccess == true);
    }
}
