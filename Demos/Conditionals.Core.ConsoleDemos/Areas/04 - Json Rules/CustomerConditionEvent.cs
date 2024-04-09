using Conditionals.Core.Areas.Events;
using Conditionals.Core.ConsoleDemos.Common.Models;

namespace Conditionals.Core.ConsoleDemos.Areas.JsonRules;

public class CustomerConditionEvent(string senderName, bool isSuccessEvent, string jsonContextData, string tenantID, List<Exception> conditionExceptions, Exception? conversionException = null) 
    : ConditionEventBase<Customer>(senderName, isSuccessEvent, jsonContextData, tenantID, conditionExceptions, conversionException)
{
}
