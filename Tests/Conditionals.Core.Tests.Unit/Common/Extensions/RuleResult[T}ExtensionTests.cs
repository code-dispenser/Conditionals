using Conditionals.Core.Common.Extensions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Common.Extensions;

public class RuleResultExtensionTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_perform_the_action_only_on_success(bool isSuccess)
    {
        var actionCount = 0;
        var ruleResult = new RuleResult<None>("RuleOne", isSuccess, None.Value, "FinalSetName", None.Value, GlobalStrings.Default_TenantID, 1000, 1, false, [], [], null, null);

        ruleResult.OnSuccess(r => actionCount++);

        if (true  == isSuccess) actionCount.Should().Be(1);
        if (false == isSuccess) actionCount.Should().Be(0);

    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_perform_the_action_only_on_failure(bool isSuccess)
    {
        var actionCount = 0;
        var ruleResult = new RuleResult<None>("RuleOne", isSuccess, None.Value, "FinalSetName", None.Value, GlobalStrings.Default_TenantID, 1000, 1, false, [], [], null, null);

        ruleResult.OnFailure(r => actionCount--);

        if (true  == isSuccess) actionCount.Should().Be(0);
        if (false == isSuccess) actionCount.Should().Be(-1);

    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_perform_the_action_dependant_on_the_is_success_value(bool isSuccess)
    {
        var actionCount = 0;
        var ruleResult = new RuleResult<None>("RuleOne", isSuccess, None.Value, "FinalSetName", None.Value, GlobalStrings.Default_TenantID, 1000, 1, false, [], [], null, null);

        ruleResult.OnResult(act_onSuccess: r => actionCount++, act_onFailure: r => actionCount--);

        if (true  == isSuccess) actionCount.Should().Be(1);
        if (false == isSuccess) actionCount.Should().Be(-1);

    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_perform_the_action_only_on_success_of_an_awaited_result(bool isSuccess)
    {
        var actionCount = 0;
        var ruleResult = await Task.FromResult(new RuleResult<None>("RuleOne", isSuccess, None.Value, "FinalSetName", None.Value, GlobalStrings.Default_TenantID, 1000, 1, false, [], [], null, null))
                            .OnSuccess(r => actionCount++); 

        if (true  == isSuccess) actionCount.Should().Be(1);
        if (false == isSuccess) actionCount.Should().Be(0);

    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_perform_the_action_only_on_failure_of_an_awaited_result(bool isSuccess)
    {
        var actionCount = 0;
        var ruleResult = await Task.FromResult(new RuleResult<None>("RuleOne", isSuccess, None.Value, "FinalSetName", None.Value, GlobalStrings.Default_TenantID, 1000, 1, false, [], [], null, null))
                                .OnFailure(r => actionCount--);

        if (true  == isSuccess) actionCount.Should().Be(0);
        if (false == isSuccess) actionCount.Should().Be(-1);

    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_perform_the_action_dependant_on_the_is_success_value_of_an_awaited_result(bool isSuccess)
    {
        var actionCount = 0;
        var ruleResult = await Task.FromResult(new RuleResult<None>("RuleOne", isSuccess, None.Value, "FinalSetName", None.Value, GlobalStrings.Default_TenantID, 1000, 1, false, [], [], null, null))
                                    .OnResult(act_onSuccess: r => actionCount++, act_onFailure: r => actionCount--);      

        if (true  == isSuccess) actionCount.Should().Be(1);
        if (false == isSuccess) actionCount.Should().Be(-1);

    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_perform_the_action_irrespective_of_the_success_value(bool isSuccess)
    {
        var actionCount = 0;
        var ruleResult = new RuleResult<None>("RuleOne", isSuccess, None.Value, "FinalSetName", None.Value, GlobalStrings.Default_TenantID, 1000, 1, false, [], [], null, null)
                                .OnSuccessOrFailure(act_onEither: r => actionCount++);
     
        if (true  == isSuccess) actionCount.Should().Be(1);
        if (false == isSuccess) actionCount.Should().Be(1);

    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_perform_the_action_irrespective_of_the_success_value_of_an_awaited_result(bool isSuccess)
    {
        var actionCount = 0;
        var ruleResult = await Task.FromResult(new RuleResult<None>("RuleOne", isSuccess, None.Value, "FinalSetName", None.Value, GlobalStrings.Default_TenantID, 1000, 1, false, [], [], null, null))
                                    .OnSuccessOrFailure(act_onEither: r => actionCount++);

        if (true  == isSuccess) actionCount.Should().Be(1);
        if (false == isSuccess) actionCount.Should().Be(1);

    }
}
