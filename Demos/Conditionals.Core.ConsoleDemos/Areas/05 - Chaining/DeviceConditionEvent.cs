using Conditionals.Core.Areas.Events;
using Conditionals.Core.ConsoleDemos.Common.Models;

namespace Conditionals.Core.ConsoleDemos.Areas.Chaining;

public class DeviceConditionEvent(string senderName, bool isSuccessEvent, string jsonContextData, string tenantID, List<Exception> conditionExceptions, Exception? conversionException = null)

    : ConditionEventBase<Probe>(senderName, isSuccessEvent, jsonContextData, tenantID, conditionExceptions, conversionException) { }
