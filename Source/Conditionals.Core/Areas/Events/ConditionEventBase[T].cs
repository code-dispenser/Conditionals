using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.Seeds;
using System.Text.Json;

namespace Conditionals.Core.Areas.Events;

/// <summary>
/// Base class used for condition events.
/// Initialises the implementation of the <see cref="ConditionEventBase{T} "/> abstract class.
/// </summary>
/// <typeparam name="T">The type of event that will either be an implementation of <see cref="RuleEventBase{T}"/> or <see cref="ConditionEventBase{T}"/></typeparam>
/// <param name="senderName">The name of the condition that raised the event.</param>
/// <param name="isSuccessEvent">A boolean indicating whether the condition evaluation succeeded or failed.</param>
/// <param name="jsonContextData">The data used in the evaluation of the condition in Json format.</param>
/// <param name="tenantID">The tenantID of the data used in the evaluation of the condition.</param>
/// <param name="conditionExceptions">A list that contains any exceptions that may have occurred during the processing and evaluation of a condition.</param>
/// <param name="conversionException">A property used to store either an exception that may occur trying to serialize the condition data or any exception that may occur during the deserialization.</param>
/// <inheritdoc cref="IEvent" />
public abstract class ConditionEventBase<T>(string senderName, bool isSuccessEvent, string jsonContextData, string tenantID, List<Exception> conditionExceptions, Exception? conversionException = null) : IEvent
{
 
    private readonly string?         _jsonContextData     = jsonContextData;
    private readonly List<Exception> _conditionExceptions = conditionExceptions ?? [];
    
    /// <summary>
    /// Gets the list of exceptions that may have occurred at any point during the evaluation of a condition.
    /// </summary>
    public List<Exception> ExecutionExceptions => [.. _conditionExceptions];
    
    ///<inheritdoc />
    public string TenantID          { get; } = tenantID;
    
    ///<inheritdoc />
    public bool IsSuccessEvent      { get; } = isSuccessEvent;
    
    ///<inheritdoc />
    public string SenderName        { get; } = senderName;
    
    ///<inheritdoc />
    public Exception?  ConversionException { get; private set; }   = conversionException;

    /// <summary>
    /// Tries to get a deserialized copy of the data that was used in the evaluation of the condition if the <see cref="ConversionException" /> property is null.
    /// If an exception occurs during the deserialization the <see cref="ConversionException" /> will be set with the exception.
    /// </summary>
    /// <param name="contextData">The type of data.</param>
    /// <returns>True if the data is non null and there were no serialization or deserialization issues, otherwise false.</returns>
    public bool TryGetData(out T? contextData)
    {
        contextData = default;

        if (_jsonContextData is null || ConversionException is not null) return false;
        
        try
        {
            contextData = JsonSerializer.Deserialize<T>(_jsonContextData!);
            if (contextData == null) throw new DeserializationToNullException(GlobalStrings.Json_deserialized_To_Null_Exception_Message);
        }
        catch (Exception exception) 
        {
            ConversionException = exception; 
            return false;
        }
  
        return true;
    }

}
