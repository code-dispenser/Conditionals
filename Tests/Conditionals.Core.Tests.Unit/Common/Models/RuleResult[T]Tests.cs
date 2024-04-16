using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Common.Models;

public class RuleResultTests
{
    [Fact]
    public void All_properties_should_be_settable_via_the_constructor()
    {
        var conditionSetResult = new ConditionSetResult<string>("SetName", "SetValue", true, 1, 1000, "SomePath", null, ["Failed"], [new SystemException()]);

        var ruleResult = new RuleResult<string>("RuleName", true, "FailureValue", "SetName", "SetValue", "TenantID", 1000, 1, true, [.. conditionSetResult.FailureMessages],
                                                [.. conditionSetResult.Exceptions], conditionSetResult, null);

        ruleResult.Should().Match<RuleResult<string>>(r => r.RuleName == "RuleName" && r.FailureValue == "FailureValue" && r.IsSuccess == true && r.IsDisabled == true
                                                   && r.SetValue == "SetValue" && r.EvaluationCount == 1 && r.Exceptions.Count == 1 && r.FinalSetName == "SetName"
                                                   && r.ConditionSetChain!.EvaluationPrecedence == "SomePath" && r.PreviousRuleResult == null && r.RuleTimeMicroseconds == 1000
                                                   && r.FailureMessages.Count == 1 && r.RuleTimeMilliseconds == 1 && r.TenantID == "TenantID");
    }


    [Fact]
    public void The_properties_rule_chain_and_result_chain_should_be_internally_settable()
    {
        var ruleResultOne = new RuleResult<int>("RuleOne", true, 2, "SetOne", 42, "TenantID", 1000, 1, true, null!, null!, null, null);
        var conditionSetResult = new ConditionSetResult<int>("SetName", 20, true, 1, 1000, "SomePath", null, ["Failed"], [new SystemException()]);

        var ruleResultTwo = new RuleResult<int>("RuleTwo", true, 10, "SetName", 42, "TenantID", 1000, 1, true, null!, null!, null, null)
        {
            ConditionSetChain = conditionSetResult,
            PreviousRuleResult = ruleResultOne
        };

        ruleResultTwo.Should().Match<RuleResult<int>>(r => r.ConditionSetChain!.SetName == "SetName" && r.PreviousRuleResult!.RuleName == "RuleOne");

    }

    [Theory]
    [InlineData(null)]
    [InlineData("  ")]
    public void A_null_or_whitespace_value_for_the_final_set_name_should_default_to_an_empty_string(string? finalSetName)

        => new RuleResult<int>("RuleOne", true, 2, finalSetName!, 42, "TenantID", 1000, 1, true, [], [], null, null)
                .FinalSetName.Should().BeEmpty();

    [Theory]
    [InlineData(null)]
    [InlineData("  ")]
    public void A_null_or_whitespace_value_for_the_tenant_id_should_default_to_all_tenants(string? tenantID)

    => new RuleResult<int>("RuleOne", true, 2, "SetName", 42, tenantID!, 1000, 1, true, [], [], null, null)
            .TenantID.Should().Be(GlobalStrings.Default_TenantID);


    [Theory]
    [InlineData(true)]
    [InlineData(false)]

    public void The_result_value_should_be_the_set_value_if_the_rule_evaluated_to_true_otherwise_it_should_be_the_failure_value(bool isSuccess)
    {
        var result = new RuleResult<int>("RuleOne", isSuccess, failureValue: 10, "SetName", setValue: 42, "TenantID", 1000, 2, false, [], [], null, null);

        (result.IsSuccess ? result.ResultValue == 42 : result.ResultValue == 10).Should().BeTrue();
    }

}
