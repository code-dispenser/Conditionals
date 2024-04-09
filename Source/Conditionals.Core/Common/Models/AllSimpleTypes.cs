using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Common.Validation;

namespace Conditionals.Core.Common.Models;
/// <summary>
/// Represents the result of a condition evaluation.
/// Initialises a new instance of the <see cref="EvaluationResult"/> class.
/// </summary>
/// <param name="IsSuccess">Indicates success or failure</param>
/// <param name="FailureMessage">The failure message associated to a failed evaluation.</param>
/// <param name="Exception">Optional exception that may have occurred during the evaluation.</param>
public record EvaluationResult(bool IsSuccess, string? FailureMessage = null, Exception? Exception = null);


/// <summary>
/// Data for any condition of the appropriate type or for a named condition.
/// Initialises a new instance of the <see cref="DataContext" /> class.
/// </summary>
/// <param name="Data">The data used in the evaluation of a condition.</param>
/// <param name="ConditionName">Optional name of the condition that the data is specifically for, otherwise available for any condition requiring this type of data.</param>
public record DataContext(dynamic Data, string ConditionName = "");

/// <summary>
/// Represents a key for an item in cache.
/// Initialises a new instance of the <see cref="CacheKey" /> class.
/// The key is the composition of all parameters.
/// </summary>
/// <param name="ItemName">The name of the item to cache.</param>
/// <param name="TenantID">Optional, a specific tenantID value otherwise the default value of All_Tenants will be applied,</param>
/// <param name="CultureID">Optional, a specific cultureID value otherwise the default value en-GB will be applied.</param>
public record CacheKey(string ItemName, string TenantID = GlobalStrings.Default_TenantID, string CultureID = GlobalStrings.Default_CultureID);

/// <summary>
/// An item held in cache.
/// Initialises a new instance of the <see cref="CacheItem" /> class.
/// </summary>
/// <param name="Value">The object that will be held in cache.</param>
public record CacheItem(object Value);

/// <summary>
/// Represents a type used as a return value when no return value is required.
/// </summary>
public readonly record struct None
{
    /// <summary>
    /// Gets the singleton instance of <see cref="None"/>.
    /// </summary>
    public static None Value { get; } = new();

    /// <summary>
    /// Returns a string representation of the <see cref="None"/> value.
    /// </summary>
    /// <returns>A string representation of the <see cref="None"/> value, which is the character "Ø".</returns>
    public override readonly string ToString() => "Ø";
}

/// <summary>
/// Container for all of the data required in the evaluation of conditions.
/// </summary>
/// <param name="dataContexts">An array of <see cref="DataContext" /> for the evaluation of conditions</param>
/// <param name="tenantID">Optional, a specific tenantID value otherwise the default value of All_Tenants will be applied. 
/// The tenantID value is assigned to all events.
/// </param>
public class ConditionData(DataContext[] dataContexts, string? tenantID = GlobalStrings.Default_TenantID)
{
    /// <summary>
    /// Get the array of <see cref="DataContext" /> instances.
    /// </summary>
    public DataContext[] Contexts   { get; } = Check.ThrowIfNullEmptyOrWhitespace(dataContexts);
    /// <summary>
    /// Gets the TenantID value. This value is provided to any raised events.
    /// </summary>
    public string TenantID          { get; } = String.IsNullOrWhiteSpace(tenantID) ? GlobalStrings.Default_TenantID : tenantID;

    /// <summary>
    /// Gets the number of <see cref="DataContext" /> in the array.
    /// </summary>
    public int  Length => Contexts.Length;

    /// <summary>
    /// Factory method for when only a single data context is required.
    /// </summary>
    /// <param name="contextData">The single data object required for a condition evaluation.</param>
    /// <param name="tenantID">Optional, a specific tenantID value otherwise the default value of All_Tenants will be applied. 
    /// The tenantID is assigned to all events.
    /// </param>
    /// <returns>An instance of the <see cref="ConditionData" /> class.</returns>
    public static ConditionData SingleContext(dynamic contextData, string tenantID = GlobalStrings.Default_TenantID)
    
        => new([new DataContext(contextData)], tenantID);
    
}


