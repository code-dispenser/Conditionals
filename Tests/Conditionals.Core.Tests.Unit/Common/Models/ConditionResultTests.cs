using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Common.Models;

public class ConditionResultTests
{
    [Fact]
    public void All_properties_should_get_set_via_the_constructor()
    {
        var conditionResult = new ConditionResult("TestCondition", typeof(string).FullName!, "s => s.ToUpper()", "john", "TestEvaluator",
                                                  false, "Name should be in upper case", 100, 110, GlobalStrings.Default_TenantID, null, [new SystemException()]);


        conditionResult.Should().Match<ConditionResult>(c => c.ConditionName == "TestCondition" && c.ContextType == "System.String"
                                                    && c.ExpressionToEvaluate == "s => s.ToUpper()" && c.EvaluationData == (object)"john" && c.EvaluatedBy == "TestEvaluator"
                                                    && c.IsSuccess == false && c.FailureMessage == "Name should be in upper case" && c.EvaluationMicroseconds == 100 && c.TotalMicroseconds == 110
                                                    && c.TenantID == GlobalStrings.Default_TenantID && c.ResultChain == null && c.Exceptions[0].GetType() == typeof(SystemException));
    }

    [Fact]
    public void Any_null_strings_should_be_set_to_an_empty_string()
    {
        var conditionResult = new ConditionResult(null!, null!, null!, null, null!, false, null!, 100, 110, null!, null, [new SystemException()]);

        conditionResult.Should().Match<ConditionResult>(c => c.ConditionName == string.Empty && c.ContextType == string.Empty
                                                    && c.ExpressionToEvaluate == string.Empty && c.EvaluatedBy == string.Empty && c.FailureMessage == string.Empty
                                                    && c.TenantID == string.Empty);
    }

    [Fact]
    public void A_null_exception_list_should_be_set_to_an_empty_list()
    {
        var conditionResult = new ConditionResult(null!, null!, null!, null, null!, false, null!, 100, 110, null!, null, null!);

        conditionResult.Should().Match<ConditionResult>(c => c.Exceptions.Count == 0);
    }
}
