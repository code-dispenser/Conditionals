using Conditionals.Core.Areas.Events;
using Conditionals.Core.ConsoleDemos.Common.Models;

namespace Conditionals.Core.ConsoleDemos.Areas.Events;

public class CheckTotalConditionEvent(string senderName, bool isSuccessEvent, string jsonContextData, string tenantID, List<Exception> conditionExceptions, Exception? conversionException = null)
    : ConditionEventBase<Order>(senderName, isSuccessEvent, jsonContextData, tenantID, conditionExceptions, conversionException)
{ }