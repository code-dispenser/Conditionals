using Conditionals.Core.Areas.Events;

namespace Conditionals.Core.ConsoleDemos.Areas.JsonRules
{
    public class DecimalRuleEvent(string senderName, bool isSuccessEvent, decimal successValue, decimal failureValue, string tenantID, List<Exception> executionExceptions) 
        : RuleEventBase<decimal>(senderName, isSuccessEvent, successValue, failureValue, tenantID, executionExceptions)
    {
    }
}
