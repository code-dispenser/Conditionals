using Conditionals.Core.Areas.Events;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;

namespace Conditionals.Core.Tests.SharedDataAndFixtures.Events;

public sealed class ConditionEventCustomer(string senderName, bool successEvent, string jsonContextData, string tenantID, List<Exception> executionExceptions, Exception? conversionException = null)

    : ConditionEventBase<Customer>(senderName, successEvent, jsonContextData, tenantID, executionExceptions, conversionException) { }

public sealed class ConditionEventPerson(string senderName, bool successEvent, string jsonContextData, string tenantID, List<Exception> executionExceptions, Exception? conversionException = null)

    : ConditionEventBase<Person>(senderName, successEvent, jsonContextData, tenantID, executionExceptions, conversionException)
{ }
public sealed class RuleEventInt(string senderName, bool isSuccessEvent, int successValue, int failureValue, string tenantID, List<Exception> executionExceptions)

    : RuleEventBase<int>(senderName, isSuccessEvent, successValue, failureValue, tenantID, executionExceptions) { }

public sealed class DuplicateRuleEventInt(string senderName, bool isSuccessEvent, int successValue, int failureValue, string tenantID, List<Exception> executionExceptions)

    : RuleEventBase<int>(senderName, isSuccessEvent, successValue, failureValue, tenantID, executionExceptions)
{ }
public sealed class BadEventInt : RuleEventBase<int>
{
    public BadEventInt(string senderName, bool isSuccessEvent, int successValue, int failureValue, string tenantID, List<Exception> executionExceptions) : base(senderName,isSuccessEvent,successValue,failureValue,tenantID,executionExceptions)
    {
        throw new SystemException();
    }
}

public sealed class BadConditionEventCustomer : ConditionEventBase<Customer>
{
    public BadConditionEventCustomer(string senderName, bool isSuccessEvent, string jsonContextData, string tenantID, List<Exception> conditionExceptions, Exception? conversionException = null) : base(senderName, isSuccessEvent, jsonContextData, tenantID, conditionExceptions, conversionException)
    {
        throw new SystemException();
    }
}
public sealed class ConditionEventBadObjectSerialization(string senderName, bool isSuccessEvent, string jsonContextData, string tenantID, List<Exception> conditionExceptions, Exception? conversionException = null) 
    : ConditionEventBase<BadObject>(senderName, isSuccessEvent, jsonContextData, tenantID, conditionExceptions, conversionException)
{
}