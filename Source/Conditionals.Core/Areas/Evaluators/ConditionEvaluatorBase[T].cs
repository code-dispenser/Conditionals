using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Common.Validation;
using System.Text.RegularExpressions;

namespace Conditionals.Core.Areas.Evaluators;

/// <summary>
/// Base class used for condition evaluators. The class has helper methods that replace any property tokens 
/// within the failure message with the respective property values.
/// </summary>
/// <inheritdoc cref="IConditionEvaluator{TContext}" />
public abstract class ConditionEvaluatorBase<TContext> : IConditionEvaluator<TContext>
{
    /// <summary>
    /// Gets a compiled Regex with a predefined pattern for failure message property token replacements.
    /// </summary>
    protected static Regex MessageRegex { get; } = new Regex("@{.*?}", RegexOptions.Compiled);

    /// <summary>
    /// Used to evaluate the condition.
    /// </summary>
    /// <param name="condition">The condition to be evaluated.</param>
    /// <param name="data">The data for the evaluation.</param>
    /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
    /// <param name="tenantID">Optional, the id of the tenant, useful in multitenant applications. The default is All_Tenants.</param>
    /// <returns>An asynchronous operation that returns an instance of the <see cref="EvaluationResult "/> class.</returns>
    public abstract Task<EvaluationResult> Evaluate(Condition<TContext> condition, TContext data, CancellationToken cancellationToken, string tenantID = GlobalStrings.Default_TenantID);


    /// <summary>
    /// Gets the property value as a string from the data context.
    /// </summary>
    /// <param name="context">The data context used in the condition.</param>
    /// <param name="propertyPath">The property path to the data on the data context.</param>
    /// <param name="replaceNullWith">Optional, string value used when properties are not found. If left blank any unmatched 
    /// property paths will default to "N/A".</param>
    /// <returns>The property value cast to a string.</returns>
    protected virtual string GetPropertyValueAsString(object context, string propertyPath, string replaceNullWith = "N/A")
    {
        propertyPath    = Check.ThrowIfNullEmptyOrWhitespace(propertyPath);
        context         = Check.ThrowIfNullEmptyOrWhitespace(context);

        object? objectValue   = context;
        object? propertyValue = null;

        foreach (var propertyName in propertyPath.Split(".", StringSplitOptions.TrimEntries))
        {
            var propertyInfo = objectValue.GetType().GetProperty(propertyName);

            if (propertyInfo == null) break;

            objectValue = propertyInfo.GetValue(objectValue, null);
            propertyValue = objectValue;

            if (objectValue == null) break;

        }

        return propertyValue?.ToString() ?? replaceNullWith;
    }

    /// <summary>
    /// Method used to build up a failure message replacing any property tokens with the property value.
    /// </summary>
    /// <param name="failureMessage">The failure message that was assigned to the condition.</param>
    /// <param name="contextData">The data used in the condition.</param>
    /// <param name="matcher">A regex for matching the replacement pattern. The condition base has a compiled MessageRegex for this purpose.</param>
    /// <param name="missingPropertyText">Optional, string value used when properties are not found. If left blank any unmatched 
    /// property paths will default to "N/A".</param>
    /// <returns>The failure message with any property tokens replace with the property value</returns>
    protected virtual string BuildFailureMessage(string failureMessage, object contextData, Regex matcher, string missingPropertyText = "N/A")
    {
        if (string.IsNullOrWhiteSpace(failureMessage)) return failureMessage;

        var matches = matcher.Matches(failureMessage);

        foreach (Match match in matches.Cast<Match>())
        {
            var propertyPath = match.Value.Substring(2, match.Value.Length - 3);
            var replacementValue = GetPropertyValueAsString(contextData, propertyPath, missingPropertyText);

            failureMessage = Regex.Replace(failureMessage, match.Value, replacementValue);
        }

        return failureMessage;
    }
}