using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;

namespace Conditionals.Core.Common.Extensions;
/// <summary>
/// Extensions on the <see cref="RuleResult{T}"/> class to enable chaining of rules and actions.
/// </summary>
public static class RuleResultExtensions
{
    /// <summary>
    /// Invokes the specified action if the rule result indicates success.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="thisRuleResult">The rule result instance.</param>
    /// <param name="act_onSuccess">The action to be invoked on success.</param>
    /// <returns>The rule result instance.</returns>
    public static RuleResult<T> OnSuccess<T>(this RuleResult<T> thisRuleResult, Action<RuleResult<T>> act_onSuccess)
    {
        if (true == thisRuleResult.IsSuccess) act_onSuccess(thisRuleResult);

        return thisRuleResult;
    }

    /// <summary>
    /// Asynchronously waits for the completion of the task representing the rule result instance,
    /// then invokes the specified action if the rule result indicates success.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="thisRuleResult">The task representing the rule result instance.</param>
    /// <param name="act_onSuccess">The action to be invoked on success.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a <see cref="RuleResult{T}"/> instance.</returns>
    public static async Task<RuleResult<T>> OnSuccess<T>(this Task<RuleResult<T>> thisRuleResult, Action<RuleResult<T>> act_onSuccess)
    {
        var awaitedResult = await thisRuleResult.ConfigureAwait(false);

        if (true == awaitedResult.IsSuccess) act_onSuccess(awaitedResult);

        return awaitedResult;
    }

    /// <summary>
    /// Asynchronously waits for the completion of the task representing the rule result instance,
    /// then invokes another rule evaluation if the rule result indicates success.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="thisRuleResult">The task representing the rule result instance.</param>
    /// <param name="ruleName">The name of the rule to be evaluated.</param>
    /// <param name="conditionEngine">The <see cref="ConditionEngine" /></param>
    /// <param name="contexts">The data contexts for the conditions <see cref="ConditionData" />.</param>
    /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
    /// <param name="precedencePrinter">Optional, concrete implementation of an IConditionPrecedencePrinter that provides
    /// the ability to create a string representation of the order of precedence of the conditions within a condition set.
    /// If used the ConditionSetResult property EvaluationPrecedence will hold the output.
    /// </param>
    /// <param name="tenantID">Optional, value used to identify the Tenant in multi-tenant applications; defaults to All_Tenants.</param>
    /// <param name="cultureID">Optional, value used to specify / filter cached rules that may have conditions with failure messages in differing languages</param>
    /// <returns>A task representing the asynchronous operation. The task result is a <see cref="RuleResult{T}"/> instance.</returns>
    public static async Task<RuleResult<T>> OnSuccess<T>(this Task<RuleResult<T>> thisRuleResult, string ruleName, ConditionEngine conditionEngine, ConditionData contexts, CancellationToken cancellationToken = default, IConditionPrecedencePrinter? precedencePrinter = null, string tenantID = GlobalStrings.Default_TenantID, string cultureID = GlobalStrings.Default_CultureID)
    {
        var awaitedResult = await thisRuleResult.ConfigureAwait(false);

        if (awaitedResult.IsSuccess)
        {
            var nextResult = await conditionEngine.EvaluateRule<T>(ruleName, contexts, cancellationToken, precedencePrinter, tenantID, cultureID).ConfigureAwait(false);
            nextResult.PreviousRuleResult = awaitedResult;
            return nextResult;
        }

        return awaitedResult;
    }

    /// <summary>
    /// Asynchronously waits for the completion of the task representing the rule result instance,
    /// then invokes the specified function if the rule result indicates success.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="thisRuleResult">The task representing the rule result instance.</param>
    /// <param name="onSuccess">The function to be invoked on success. It takes the current rule result as input and returns a task representing the next rule result.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a <see cref="RuleResult{T}"/> instance.</returns>
    public static async Task<RuleResult<T>> OnSuccess<T>(this Task<RuleResult<T>> thisRuleResult, Func<RuleResult<T>, Task<RuleResult<T>>> onSuccess)
    {
        var awaitedResult = await thisRuleResult.ConfigureAwait(false);

        if (true == awaitedResult.IsSuccess)
        {
            var nextResult = await onSuccess(awaitedResult).ConfigureAwait(false);
            nextResult.PreviousRuleResult = awaitedResult;
            return nextResult;
        }

        return awaitedResult;
    }


    /// <summary>
    /// Invokes the specified action if the rule result indicates failure.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="thisRuleResult">The rule result instance.</param>
    /// <param name="act_onFailure">The action to be invoked on failure.</param>
    /// <returns>The rule result instance.</returns>
    public static RuleResult<T> OnFailure<T>(this RuleResult<T> thisRuleResult, Action<RuleResult<T>> act_onFailure)
    {
        if (false == thisRuleResult.IsSuccess) act_onFailure(thisRuleResult);

        return thisRuleResult;
    }


    /// <summary>
    /// Asynchronously waits for the completion of the task representing the rule result instance,
    /// then invokes the specified action if the rule result indicates failure.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="thisRuleResult">The task representing the rule result instance.</param>
    /// <param name="act_onFailure">The action to be invoked on failure.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a <see cref="RuleResult{T}"/> instance.</returns>
    public static async Task<RuleResult<T>> OnFailure<T>(this Task<RuleResult<T>> thisRuleResult, Action<RuleResult<T>> act_onFailure)
    {
        var awaitedResult = await thisRuleResult.ConfigureAwait(false);

        if (false == awaitedResult.IsSuccess) act_onFailure(awaitedResult);

        return awaitedResult;
    }


    /// <summary>
    /// Asynchronously waits for the completion of the task representing the rule result instance,
    /// then invokes another rule evaluation if the rule result indicates failure.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="thisRuleResult">The task representing the rule result instance.</param>
    /// <param name="ruleName">The name of the rule to be evaluated.</param>
    /// <param name="conditionEngine">The <see cref="ConditionEngine" /></param>
    /// <param name="contexts">The data contexts for the conditions <see cref="ConditionData" />.</param>
    /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
    /// <param name="precedencePrinter">Optional, concrete implementation of an IConditionPrecedencePrinter that provides
    /// the ability to create a string representation of the order of precedence of the conditions within a condition set.
    /// If used the ConditionSetResult property EvaluationPrecedence will hold the output.
    /// </param>
    /// <param name="tenantID">Optional, value used to identify the Tenant in multi-tenant applications; defaults to All_Tenants.</param>
    /// <param name="cultureID">Optional, value used to specify / filter cached rules that may have conditions with failure messages in differing languages</param>
    /// <returns>A task representing the asynchronous operation. The task result is a <see cref="RuleResult{T}"/> instance.</returns>
    public static async Task<RuleResult<T>> OnFailure<T>(this Task<RuleResult<T>> thisRuleResult, string ruleName, ConditionEngine conditionEngine, ConditionData contexts, CancellationToken cancellationToken = default, IConditionPrecedencePrinter? precedencePrinter = null, string tenantID = GlobalStrings.Default_TenantID, string cultureID = GlobalStrings.Default_CultureID)
    {
        var awaitedResult = await thisRuleResult.ConfigureAwait(false);

        if (false == awaitedResult.IsSuccess)
        {
            var nextResult = await conditionEngine.EvaluateRule<T>(ruleName, contexts, cancellationToken, precedencePrinter, tenantID, cultureID).ConfigureAwait(false);
            nextResult.PreviousRuleResult = awaitedResult;
            return nextResult;
        }

        return awaitedResult;
    }

    /// <summary>
    /// Asynchronously waits for the completion of the task representing the rule result instance,
    /// then invokes the specified function if the rule result indicates failure.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="thisRuleResult">The task representing the rule result instance.</param>
    /// <param name="onFailure">The function to be invoked on failure. It takes the current rule result as input and returns a task representing the next rule result.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a <see cref="RuleResult{T}"/> instance.</returns>
    public static async Task<RuleResult<T>> OnFailure<T>(this Task<RuleResult<T>> thisRuleResult, Func<RuleResult<T>, Task<RuleResult<T>>> onFailure)
    {
        var awaitedResult = await thisRuleResult.ConfigureAwait(false);

        if (false == awaitedResult.IsSuccess)
        {
            var nextResult = await onFailure(awaitedResult).ConfigureAwait(false);
            nextResult.PreviousRuleResult = awaitedResult;
            return nextResult;
        }

        return awaitedResult;
    }

    /// <summary>
    /// Invokes the specified actions based on the success or failure of the rule result.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="thisRuleResult">The rule result instance.</param>
    /// <param name="act_onSuccess">The action to be invoked on success.</param>
    /// <param name="act_onFailure">The action to be invoked on failure.</param>
    /// <returns>The rule result instance.</returns>
    public static RuleResult<T> OnResult<T>(this RuleResult<T> thisRuleResult, Action<RuleResult<T>> act_onSuccess, Action<RuleResult<T>> act_onFailure)

        => thisRuleResult.IsSuccess ? OnSuccess(thisRuleResult, act_onSuccess) : OnFailure(thisRuleResult, act_onFailure);


    /// <summary>
    /// Asynchronously waits for the completion of the task representing the rule result instance,
    /// then invokes the specified actions based on the success or failure of the rule result.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="thisRuleResult">The task representing the rule result instance.</param>
    /// <param name="act_onSuccess">The action to be invoked on success.</param>
    /// <param name="act_onFailure">The action to be invoked on failure.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a <see cref="RuleResult{T}"/> instance.</returns>
    public static async Task<RuleResult<T>> OnResult<T>(this Task<RuleResult<T>> thisRuleResult, Action<RuleResult<T>> act_onSuccess, Action<RuleResult<T>> act_onFailure)
    {
        var awaitedResult = await thisRuleResult.ConfigureAwait(false);

        return awaitedResult.IsSuccess ? OnSuccess(awaitedResult, act_onSuccess) : OnFailure(awaitedResult,act_onFailure);

    }

    /// <summary>
    /// Invokes the specified action on either success or failure of the rule result.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="thisRuleResult">The rule result instance.</param>
    /// <param name="act_onEither">The action to be invoked on either success or failure.</param>
    /// <returns>The rule result instance.</returns>
    public static RuleResult<T> OnSuccessOrFailure<T>(this RuleResult<T> thisRuleResult, Action<RuleResult<T>> act_onEither)
    {
        act_onEither(thisRuleResult);
        return thisRuleResult;
    }

    /// <summary>
    /// Asynchronously waits for the completion of the task representing the rule result instance,
    /// then invokes the specified action on either success or failure of the rule result.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="thisRuleResult">The task representing the rule result instance.</param>
    /// <param name="act_onEither">The action to be invoked on either success or failure.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a <see cref="RuleResult{T}"/> instance.</returns>

    public static async Task<RuleResult<T>> OnSuccessOrFailure<T>(this Task<RuleResult<T>> thisRuleResult, Action<RuleResult<T>> act_onEither)
    {
        var awaitedResult = await thisRuleResult.ConfigureAwait(false);
        
        act_onEither(awaitedResult);
        
        return awaitedResult;
    }


}
   
