using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Evaluators;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Evaluators;

public class ConditionEvaluatorBaseTests
{

    [Fact]
    public async Task A_null_or_empty_failure_message_passed_to_the_base_build_message_method_should_return_an_empty_failure_message_that_gets_reset_to_the_original_by_the_condition()
    {
        var condition = new CustomPredicateCondition<Customer>("ConditionOne", c => c.CustomerName == "WrongName", "The customer name was @{CustomerName} but expected WrongName","ConditionEvaluatorBaseTester");
        ConditionData data = new([new(StaticData.CustomerOne())]);

        var result = await condition.Evaluate((_, _) => new ConditionEvaluatorBaseTester<Customer>(true, "[Bad Property Token]"), data);

        result.FailureMessage.Should().Be("The customer name was @{CustomerName} but expected WrongName");
    }

    [Fact]
    public async Task A_bad_replacement_token_or_missing_context_property_should_use_the_property_replacement_text()
    {
        var condition = new CustomPredicateCondition<Customer>("ConditionOne", c => c.CustomerName == "WrongName", "The customer name was @{BadPropertyName} but expected WrongName", "ConditionEvaluatorBaseTester");
        ConditionData data = new([new(StaticData.CustomerOne())]);

        var result = await condition.Evaluate((_,_) => new ConditionEvaluatorBaseTester<Customer>(false,"[Bad Property Token]"), data);

        result.FailureMessage.Should().Be("The customer name was [Bad Property Token] but expected WrongName");
    }

    [Fact]
    public async Task A_property_token_that_has_a_null_property_value_can_use_a_replacement_text_the_default_is_not_available()
    {
        var condition       = new CustomPredicateCondition<Customer>("ConditionOne", c => c.CustomerName == "CustomerOne", "The customer name was @{CustomerName} but expected CustomerOne","ConditionEvaluatorBaseTester");
        var customer        = new Customer(null!, 123, 123, 123, null);
        ConditionData data  = new([new(customer)]);

        var result = await condition.Evaluate((_, _) => new ConditionEvaluatorBaseTester<Customer>(false), data);

        result.FailureMessage.Should().Be("The customer name was N/A but expected CustomerOne");
    }


}

