using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Integration.Areas.Evaluators;

public class RegexConditionEvaluatorTests
{
    [Fact]
    public async Task A_missing_regex_options_additional_info_dictionary_key_should_just_use_the_default_of_regex_options_none()
    {
        //var condition = new RegexCondition<Customer>("ConditionOne", c => c.Address!.AddressLine, "^[A-Z][a-z]{2,49}$",
        //                                        "The address line of: @{Address.AddressLine} did not match the required pattern", RegexOptions.IgnoreCase);
        //var rule = new Rule<None>("RegexRule", None.Value, new ConditionSet<None>("ConditionSetOne", None.Value, condition));
        //var json = rule.ToJsonString(true);

        var conditionEngine = new ConditionEngine();
        var json            = StaticData.JsonWithMissingRegexOptionsKey;
        var customer        = new Customer("Test", 123, 123, 5, new Address("ADDRESSLINE", "town", "City", "Postcode"));
        ConditionData data  = new([new(customer)]);

        conditionEngine.IngestRuleFromJson(json);
        //RegexOptions.IgnoreCase missing so evaluation should fail.
        var result = await conditionEngine.EvaluateRule<None>("RegexRule", data);

        result.Should().Match<RuleResult<None>>(r => r.EvaluationCount == 1 && r.FailureMessages[0] == "The address line of: ADDRESSLINE did not match the required pattern");
    }

    [Fact]
    public async Task A_invalid_regex_option_should_should_get_converted_to_none()
    {
        var conditionEngine = new ConditionEngine();
        var json            = StaticData.JsonWithIncorrectRegexOptionValue;
        var customer        = new Customer("Test", 123, 123, 5, new Address("ADDRESSLINE", "town", "City", "Postcode"));
        ConditionData data  = new([new(customer)]);

        conditionEngine.IngestRuleFromJson(json);
        //The option value of IgnoreCase is correct so the evaluation should fail.
        var result = await conditionEngine.EvaluateRule<None>("RegexRule", data);

        result.Should().Match<RuleResult<None>>(r => r.EvaluationCount == 1 && r.FailureMessages[0] == "The address line of: ADDRESSLINE did not match the required pattern");
    }


    [Fact]
    public async Task Any_unhandled_error_should_get_caught_with_the_evaluate_method_returning_a_result_containing_the_exception()
    {
        var conditionEngine = new ConditionEngine();
        var json            = StaticData.JsonWithCorruptedExpressionString;
        var customer        = new Customer("Test", 123, 123, 5, new Address("ADDRESSLINE", "town", "City", "Postcode"));
        ConditionData data  = new([new(customer)]);

        conditionEngine.IngestRuleFromJson(json);
        //The option value of IgnoreCase is correct so the evaluation should fail.
        var result = await conditionEngine.EvaluateRule<None>("RegexRule", data);

        result.Should().Match<RuleResult<None>>(r => r.IsSuccess == false &&  r.EvaluationCount == 1 && r.ConditionSetChain!.ResultChain!.Exceptions[0] != null);

    }

}
