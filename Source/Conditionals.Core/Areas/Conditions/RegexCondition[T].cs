using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Common.Validation;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Conditionals.Core.Areas.Conditions;

///<summary>
/// Used to create a condition that uses a regular expression.
///</summary>
///<inheritdoc cref="Condition{TContext}" />
public sealed class RegexCondition<TContext>(string conditionName, Expression<Func<TContext, object>> propertyPath, string regexPattern, string failureMessage, RegexOptions regexOptions = RegexOptions.None, EventDetails? eventDetails = null) 
    
    : Condition<TContext>(conditionName, BuildExpressionToEvaluateString(propertyPath, regexPattern), ConditionType.CustomExpression,failureMessage,GlobalStrings.Regex_Condition_Evaluator,BuildAdditionalInfo(regexOptions), eventDetails)
{
    /// <summary>
    /// Initialises a new instance of the <see cref="RegexCondition{TContext} "/> class.
    /// </summary>
    /// <param name="conditionName">The name of the condition.</param>
    /// <param name="propertyPath">The property path of the property value to be evaluated against the regex pattern.</param>
    /// <param name="regexPattern">The regex pattern to use in the evaluation.</param>
    /// <param name="failureMessage">The failure message for failed evaluations.</param>
    public RegexCondition(string conditionName, Expression<Func<TContext, object>> propertyPath, string regexPattern, string failureMessage)

        : this(conditionName, propertyPath, regexPattern, failureMessage, RegexOptions.None, null) { }

    private static Dictionary<string,string> BuildAdditionalInfo(RegexOptions regexOptions = RegexOptions.None)
    {
        Dictionary<string, string> additionalInfo = [];

        StringBuilder stringBuilder = new();

        foreach (RegexOptions option in Enum.GetValues(typeof(RegexOptions)))
        {
            if (((regexOptions & option) == option) && (option != RegexOptions.None))
            {
                stringBuilder.Append(option.ToString());
                stringBuilder.Append(" | ");
            }
        }

        if (stringBuilder.Length > 0) additionalInfo[GlobalStrings.Regex_Options_Key] = stringBuilder.ToString().TrimEnd(' ','|', ' ');
      
        return additionalInfo;
    }

    private static string BuildExpressionToEvaluateString(Expression<Func<TContext,object>> propertyPath, string regexPattern)
        
        => String.Concat(GetPropertyPath(Check.ThrowIfNullEmptyOrWhitespace(propertyPath)), " [IsMatch] ", Check.ThrowIfNullEmptyOrWhitespace(regexPattern)); 


    private static string GetPropertyPath(Expression<Func<TContext, object>> expression)
    {
        MemberExpression memberExpression;

        if (expression.Body is UnaryExpression unaryExpression)
        {
            memberExpression = (MemberExpression)unaryExpression.Operand;
        }
        else
        {
            memberExpression = (MemberExpression)expression.Body;
        }

        var propertyPath = GetMemberAccessPath(memberExpression);

        return propertyPath;
    }

    private static string GetMemberAccessPath(MemberExpression memberExpression)
    {
        Stack<string> stack = new();
        MemberExpression? currentExpression = memberExpression;

        while (currentExpression != null)
        {
            stack.Push(currentExpression.Member.Name);
            currentExpression = currentExpression.Expression as MemberExpression;
        }

        return string.Join(".", stack);
    }
    //private static string GetMemberAccessPath(MemberExpression memberExpression)
    //{
    //    if (memberExpression.Expression is MemberExpression innerMemberExpression)
    //    {
    //        return GetMemberAccessPath(innerMemberExpression) + "." + memberExpression.Member.Name;
    //    }

    //    return memberExpression.Member.Name;
    //}
}


