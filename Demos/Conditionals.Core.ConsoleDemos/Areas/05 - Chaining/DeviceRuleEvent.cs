using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.Models;

namespace Conditionals.Core.ConsoleDemos.Areas.Chaining
{
    public class DeviceRuleEvent(string senderName, bool isSuccessEvent, None successValue, None failureValue, string tenantID, List<Exception> executionExceptions) 
        
        : RuleEventBase<None>(senderName, isSuccessEvent, successValue, failureValue, tenantID, executionExceptions) {}
}
