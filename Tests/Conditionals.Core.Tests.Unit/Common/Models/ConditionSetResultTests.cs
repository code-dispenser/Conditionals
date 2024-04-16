using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Common.Models;

public class ConditionSetResultTests
{
    [Fact]
    public void All_properties_should_get_set_via_the_constructor()
    {
        var conditionResult = new ConditionResult("TestCondition", typeof(string).FullName!, "s => s.ToUpper()", "john", "TestEvaluator",
                                             false, "Name should be in upper case", 1000, 110, GlobalStrings.Default_TenantID, null, [new SystemException()]);

        var conditionSetResult = new ConditionSetResult<string>("SetName", "SetValue", true, 1, 1000, "SomePath", conditionResult, ["Failed"], [new SystemException()]);


        conditionSetResult.Should().Match<ConditionSetResult<string>>(c => c.SetName == "SetName" && c.SetValue == "SetValue" && c.IsSuccess == true
                                                                  && c.TotalMilliseconds == 1 && c.TotalEvaluations == 1 && c.TotalMicroseconds == 1000 && c.ResultChain == conditionResult
                                                                  && c.EvaluationPrecedence == "SomePath" && c.FailureMessages.Count == 1 && c.Exceptions.Count == 1);

    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    public void Evaluation_precedence_if_null_or_whitespace_should_default_to_an_empty_string(string? evaluationPrecedence)
    {
        var conditionSetResult = new ConditionSetResult<string>("SetName", "SetValue", true, 1, 1000, evaluationPrecedence!, null, ["Failed"], [new SystemException()]);

        conditionSetResult.EvaluationPrecedence.Should().BeEmpty();
    }

    [Fact]
    public void A_null_failure_message_list_should_default_to_an_empty_list()
    {
        var conditionSetResult = new ConditionSetResult<string>("SetName", "SetValue", true, 1, 1000, "Some Path", null, null!, [new SystemException()]);

        conditionSetResult.FailureMessages.Count.Should().Be(0);
    }

    [Fact]
    public void A_null_exception_list_should_default_to_an_empty_list()
    {
        var conditionSetResult = new ConditionSetResult<string>("SetName", "SetValue", true, 1, 1000, "Some Path", null, null!, null!);

        conditionSetResult.Exceptions.Count.Should().Be(0);
    }

    [Fact]
    public void The_previous_set_property_should_be_internally_settable()
    {
        var conditionSetResultOne = new ConditionSetResult<string>("SetOne", "SetValue", true, 1, 1000, "Some Path", null, null!, null!);
        var conditionSetResultTwo = new ConditionSetResult<string>("SetTwo", "SetValue", true, 1, 1000, "Some Path", null, null!, null!)
        {
            PreviousSetResult = conditionSetResultOne
        };

        conditionSetResultTwo.PreviousSetResult.SetName.Should().Be("SetOne");
    }
}
