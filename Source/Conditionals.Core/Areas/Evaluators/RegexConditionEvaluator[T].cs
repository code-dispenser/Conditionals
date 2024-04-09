using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using System.Text.RegularExpressions;

namespace Conditionals.Core.Areas.Evaluators;

/// <summary>
/// An implementation of the <see cref="ConditionEvaluatorBase{TContext}" /> abstract class that uses a regular expression to evaluate a <see cref="RegexCondition{TContext}" /> classes expression./>  
/// </summary>
/// <inheritdoc cref="ConditionEvaluatorBase{TContext}" /> 
public sealed class RegexConditionEvaluator<TContext> : ConditionEvaluatorBase<TContext>
{
    /// <summary>
    /// Evaluates instances of the <see cref="RegexCondition{TContext}" /> class.
    /// </summary>
    /// <param name="condition">The condition to be evaluated.</param>
    /// <param name="data">The data used in the evaluation.</param>
    /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
    /// <param name="tenantID">The id of the tenant who the data belongs to. </param>
    /// <returns>An asynchronous operation that returns an instance of the <see cref="EvaluationResult"/> class.</returns>
    public override Task<EvaluationResult> Evaluate(Condition<TContext> condition, TContext data, CancellationToken cancellationToken, string tenantID = "All_Tenants")
    {
        var regexOptions    = BuildRegexOptions(condition.AdditionalInfo);
        var expressionParts = condition.ExpressionToEvaluate.Split(GlobalStrings.Regex_Split_Chars, StringSplitOptions.TrimEntries);
        var failureMessage  = String.Empty;

        try
        {
            var regexPattern   = expressionParts[1];
            var propertyValue  = base.GetPropertyValueAsString(data!,expressionParts[0]);
            var isMatch        = false;

            isMatch = Regex.IsMatch(propertyValue,regexPattern, regexOptions);

            failureMessage = isMatch == false ? BuildFailureMessage(condition.FailureMessage, data!, MessageRegex) : failureMessage;

            return Task.FromResult(new EvaluationResult(isMatch, failureMessage, null));

        }
        catch(Exception exception) 
        {
            return Task.FromResult(new EvaluationResult(false, failureMessage, exception)); 
        }
    }
        

    private static RegexOptions BuildRegexOptions(Dictionary<string, string> additionalInfo)
    {
        if (false == additionalInfo.ContainsKey(GlobalStrings.Regex_Options_Key)) return RegexOptions.None;

        var options = additionalInfo[GlobalStrings.Regex_Options_Key].Split(GlobalStrings.Regex_Split_Char, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        RegexOptions regexOptions = RegexOptions.None;

        foreach (var option in options)
        {
            var parsedRegexOption = Enum.TryParse(option, true, out RegexOptions parsedOption) ? parsedOption : RegexOptions.None;
            regexOptions |= parsedRegexOption;
        }

        return regexOptions;
    }

}
