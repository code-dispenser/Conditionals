using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;

namespace Conditionals.Core.Common.Builders;

/// <summary>
/// Interface for a builder to allow a fluent interface.
/// </summary>
public interface IAddData
{
    /// <summary>
    /// Specifies that the data context can be used for any condition requiring data of the supplied type. Although dynamic
    /// is used to allow differing data types used within a condition set, the actual type of data is checked against what the 
    /// condition expects.
    /// </summary>
    /// <param name="data">Data for the condition(s)</param>
    /// <returns>An implementation of the <see cref="IAddData"/> interface to enable chaining.</returns>
    IAddData AndForAny(dynamic data);

    /// <summary>
    /// Specifies a data context for a named condition.
    /// </summary>
    /// <param name="named">The name of the condition that the data is for.</param>
    /// <param name="data">Data for the condition.</param>
    /// <returns>An implementation of the <see cref="IAddData"/> interface to enable chaining.</returns>
    IAddData AndForCondition(string named, dynamic data);

    /// <summary>
    /// Finishes the build process by creating a RuleData object containing the data contexts for the conditions.
    /// </summary>
    /// <param name="forTenantID">Optional, specifies the tenantID of the data, used in the evaluation of a condition. 
    /// The default of All_Tenants is used if not specified. The tenantID value is added to all events.</param>
    /// <returns>An instance of the <see cref="ConditionData"/> class with an instantiated array of <see cref="DataContext" /> instances.</returns>
    ConditionData Create(string forTenantID = GlobalStrings.Default_TenantID);
}

/// <summary>
/// Builder that aids in the creation of <see cref="ConditionData" />.
/// </summary>
/// <inheritdoc cref="IAddData"/>
public class ConditionDataBuilder : IAddData
{
    private readonly List<DataContext> _contextData = [];

    private ConditionDataBuilder() { }


    /// <summary>
    /// Specifies that the data context can be used for any condition requiring data of the supplied type. Although dynamic
    /// is used to allow differing data types used within a condition set, the actual type of data is checked against what the 
    /// condition expects.
    /// </summary>
    /// <param name="data">Data for the condition(s)</param>
    /// <returns>An implementation of the <see cref="IAddData"/> interface to enable chaining.</returns>
    public static IAddData AddForAny(dynamic data)

        => new ConditionDataBuilder().AndForAny(data);
    
    /// <inheritdoc />
    public IAddData AndForAny(dynamic data)

        => AddConditionData(data, "");

    /// <summary>
    /// Specifies a data context for a named condition.
    /// </summary>
    /// <param name="named">The name of the condition that the data is for.</param>
    /// <param name="data">Data for the condition.</param>
    /// <returns>An implementation of the <see cref="IAddData"/> interface to enable chaining.</returns>
    public static IAddData AddForCondition(string named, dynamic data)

        => new ConditionDataBuilder().AndForCondition(named, data);

    /// <inheritdoc />
    public IAddData AndForCondition(string named, dynamic data)

        => AddConditionData(data, named);

    /// <inheritdoc />
    public ConditionData Create(string forTenantID = GlobalStrings.Default_TenantID)

        => new([.. _contextData], forTenantID);

    private ConditionDataBuilder AddConditionData(dynamic data, string conditionName = "")
    {
        if (false == String.IsNullOrWhiteSpace(conditionName) && false == _contextData.Exists(c => c.ConditionName == conditionName))
        {
            _contextData.Add(new DataContext(data, conditionName));
            return this;
        }
        
        if (false == _contextData.Exists(c => c.Data.GetType() == data.GetType())) _contextData.Add(new DataContext(data, conditionName));
 
        return this;
    }
}
