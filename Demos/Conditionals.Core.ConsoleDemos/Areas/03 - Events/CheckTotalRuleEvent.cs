using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.Models;

namespace Conditionals.Core.ConsoleDemos.Areas.Events;

public class CheckTotalRuleEvent : RuleEventBase<None>
{
    public CheckTotalRuleEvent(string senderName, bool isSuccessEvent, None successValue, None failureValue, string tenantID, List<Exception> executionExceptions) : base(senderName, isSuccessEvent, successValue, failureValue, tenantID, executionExceptions)
    {
    }
}
