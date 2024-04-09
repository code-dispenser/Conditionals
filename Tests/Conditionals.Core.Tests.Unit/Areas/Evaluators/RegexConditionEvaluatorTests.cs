using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Evaluators;

public class RegexConditionEvaluatorTests
{
    [Fact]
    public async Task Should_perform_a_regex_is_match_using_the_provided_property_path_data_pattern_and_any_regex_options()
    {
        var condition = new RegexCondition<Customer>("ConditionOne", c => c.Address!.AddressLine, "^[A-Z][a-z]{2,49}$",
                                                        "The address line of: @{Address.AddressLine} did not match the required pattern",RegexOptions.IgnoreCase);

        var customer = new Customer("Test",123,123,5, new Address("ADDRESSLINE","town","City","Postcode"));

        ConditionData data = new([new(customer)]);

        var result = await condition.Evaluate(RegexConditionEvaluatorTests.GetEvaluator, data);
        //AddressLine is ADDRESSLINE but ignore case option is on so should pass
        result.Should().Match<ConditionResult>(r => r.IsSuccess == true && r.FailureMessage == String.Empty);

    }

    private static IConditionEvaluator GetEvaluator(string name, Type type)

        => (IConditionEvaluator)Activator.CreateInstance(typeof(RegexConditionEvaluator<>).MakeGenericType(type))!;
}
