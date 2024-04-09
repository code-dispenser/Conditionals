using Conditionals.Core.Areas.Events;
using Conditionals.Core.Tests.SharedDataAndFixtures.Events;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Events;

public class RuleEventBaseTests
{
    [Fact]
    public void All_properties_should_be_set_via_the_base_constructor()
    {
        var ruleEvent = new RuleEventInt("RuleOne", true, 42, 10, "TenantID", [new SystemException()]);

        var baseEvent = ruleEvent as RuleEventBase<int>;

        baseEvent.Should().Match<RuleEventBase<int>>(r => r.SenderName == "RuleOne" && r.IsSuccessEvent == true && r.SuccessValue == 42 && r.FailureValue == 10
                                                 && r.TenantID == "TenantID" && r.ExecutionExceptions[0].GetType() == typeof(SystemException));
    }

    [Fact]
    public void A_null_passed_as_the_list_of_execution_exceptions_should_default_to_an_empty_list()
    {
        var ruleEvent = new RuleEventInt("RuleOne", true, 42, 10, "TenantID", null!);

        var baseEvent = ruleEvent as RuleEventBase<int>;

        baseEvent.ExecutionExceptions.Should().BeEmpty();
    }
    
}
