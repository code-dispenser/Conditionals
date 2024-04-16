using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.FormatConverters;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Common.Validation;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Conditionals.Core.Areas.Rules;

/// <inheritdoc cref="IRule{T}" />
public class Rule<T> : IRule<T>
{
    private readonly List<ConditionSet<T>> _conditionSets = [];

    ///<inheritdoc />
    public IReadOnlyList<ConditionSet<T>> ConditionSets => _conditionSets.AsReadOnly();

    ///<inheritdoc />
    public EventDetails? RuleEventDetails { get; }

    ///<inheritdoc />
    public string CultureID { get; } = GlobalStrings.Default_CultureID;

    ///<inheritdoc />
    public string TenantID { get; } = GlobalStrings.Default_TenantID;

    ///<inheritdoc />
    public string RuleName  { get; } = default!;

    ///<inheritdoc />
    public T FailureValue   { get; } = default!;

    ///<inheritdoc />
    public bool IsDisabled  { get; set; } = false;

    /// <summary>
    /// Initialises a new instance of the <see cref="Rule{T}" /> class.
    /// </summary>
    /// <param name="ruleName">The name of the rule.</param>
    /// <param name="failureValue">A value used for failed evaluations, assigned to an instance fo the the <see cref="RuleResult{T}" /> class.</param>
    /// <param name="ruleEventDetails">Optional, instance of an <see cref="EventDetails"/> class.</param>
    /// <param name="tenantID">Optional, specific Tenant ID that a rule may be associated with otherwise the default of All_Tenants will be used.</param>
    /// <param name="cultureID">Optional, specific CultureID associated to the rule if differing languages are used by condition failure messages. The default is en-GB.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="ruleName"/> or <paramref name="failureValue"/> is null or empty.</exception>
    public Rule(string ruleName, T failureValue, EventDetails? ruleEventDetails = null, string tenantID = GlobalStrings.Default_TenantID, string cultureID = GlobalStrings.Default_CultureID)
    {
        RuleName            = Check.ThrowIfNullEmptyOrWhitespace(ruleName);
        RuleEventDetails    = ruleEventDetails;
        CultureID           = String.IsNullOrWhiteSpace(cultureID) ? GlobalStrings.Default_CultureID : cultureID;
        TenantID            = String.IsNullOrWhiteSpace(tenantID) ? GlobalStrings.Default_TenantID : tenantID;
        FailureValue        = Check.ThrowIfNullEmptyOrWhitespace(failureValue);

    }

    /// <summary>
    /// Initialises a new instance of the <see cref="Rule{T}" /> class.
    /// </summary>
    /// <param name="ruleName">The name of the rule.</param>
    /// <param name="failureValue">A value used for failed evaluations, assigned to an instance fo the the <see cref="RuleResult{T}" /> class.</param>
    /// <param name="conditionSet">An instance of a <see cref="ConditionSet{T} "/> class holding the condition tree.</param>
    /// <param name="ruleEventDetails">Optional, instance of an <see cref="EventDetails"/> class.</param>
    /// <param name="tenantID">Optional, specific Tenant ID that a rule may be associated with otherwise the default of All_Tenants will be used.</param>
    /// <param name="cultureID">Optional, specific CultureID associated to the rule if differing languages are used by condition failure messages. The default is en-GB.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="ruleName"/> or <paramref name="failureValue"/> is null or empty.</exception>
    public Rule(string ruleName, T failureValue, ConditionSet<T> conditionSet, EventDetails? ruleEventDetails = null, string tenantID = GlobalStrings.Default_TenantID, string cultureID = GlobalStrings.Default_CultureID)
    {
        CultureID           = String.IsNullOrWhiteSpace(cultureID) ? GlobalStrings.Default_CultureID : cultureID;
        TenantID            = String.IsNullOrWhiteSpace(tenantID) ? GlobalStrings.Default_TenantID : tenantID;
        RuleName            = Check.ThrowIfNullEmptyOrWhitespace(ruleName);
        FailureValue        = Check.ThrowIfNullEmptyOrWhitespace(failureValue);
        RuleEventDetails    = ruleEventDetails;

        OrConditionSet(conditionSet);
    }

    ///<inheritdoc />
    public Rule<T> OrConditionSet(ConditionSet<T> conditionSet)
    {
        conditionSet = Check.ThrowIfNullEmptyOrWhitespace(conditionSet);
        _conditionSets.Add(conditionSet);

        return this;
    }

    ///<inheritdoc />
    public async Task<RuleResult<T>> Evaluate(ConditionEvaluatorResolver evaluatorResolver, ConditionData conditionData, EventPublisher? eventPublisher = null, IConditionPrecedencePrinter? precedencePrinter = null, CancellationToken cancellationToken = default)
    {
        RuleResult<T> result;
        RaiseEventException? eventException = null;

        long startingTicks  = Stopwatch.GetTimestamp();
        var currentSet      = this.ConditionSets[0];
        var setResult       = default(ConditionSetResult<T>);

        if (true == this.IsDisabled) return Rule<T>.BuildResult(this.RuleName, this.FailureValue, true, this.TenantID, null, currentSet.SetName, currentSet.SetValue, startingTicks, null);

        if (conditionData is null) throw new MissingAllConditionDataException(GlobalStrings.Missing_All_Condition_Data_Exception_Message);
        if (evaluatorResolver is null) throw new MissingEvaluatorResolverException(GlobalStrings.Missing_Evaluator_Resolver_Exception_Message);

        foreach (var conditionSet in this.ConditionSets)
        {
            currentSet = conditionSet;

            var currentSetResult = await conditionSet.Evaluate(evaluatorResolver, conditionData, eventPublisher, precedencePrinter, cancellationToken);

            if (setResult is null)//first loop so we need to set the result for the previousSetResult property if needed i.e if the first set fails
            {
                setResult = currentSetResult;
                if (true == setResult.IsSuccess) break; //Akin to a logical short-circuit OrElse operator 
                continue;                               //failed so move on to the next
            }

            currentSetResult.PreviousSetResult = setResult;
            setResult = currentSetResult;
            if (true == setResult.IsSuccess) break;
        }

        if (this.RuleEventDetails is not null && eventPublisher is not null)
        {
            try
            {
                RaiseEvent(eventPublisher, this.RuleEventDetails, this.RuleName, setResult!.IsSuccess, setResult.SetValue, this.FailureValue, this.TenantID, GetExecutionExceptions(setResult), cancellationToken);
            }
            catch (Exception exception)
            {
                var eventName = Type.GetType(this.RuleEventDetails.EventTypeName)!.Name;
                eventException = new RaiseEventException(String.Format(GlobalStrings.Raise_Event_Exception_Message, eventName), exception);
            }
        }

        result = BuildResult(this.RuleName, this.FailureValue, false, this.TenantID, setResult, setResult!.SetName, setResult.SetValue, startingTicks, eventException);

        return result;
    }

    private static RuleResult<T> BuildResult(string ruleName, T failureValue, bool isDisabled, string tenantID, ConditionSetResult<T>? finalSet, string setName, T setValue, long startingTicks, Exception? exception)
    {
        List<Exception> exceptions = [];
        List<string> failureMessages = [];
        int evaluationCount = 0;

        var checkResult = finalSet;

        while (checkResult is not null)
        {
            exceptions.InsertRange(0, checkResult.Exceptions);
            failureMessages.InsertRange(0, checkResult.FailureMessages);
            evaluationCount += checkResult.TotalEvaluations;
            checkResult = checkResult.PreviousSetResult;
        }

        if (exception is not null) exceptions.Insert(0, exception);

        var isSuccess = finalSet is not null && finalSet.IsSuccess;

        var endingTicks = Stopwatch.GetTimestamp();

        var totalTimeForRule = (endingTicks - startingTicks) / (Stopwatch.Frequency / 1_000_000);

        return new RuleResult<T>(ruleName, isSuccess, failureValue, setName, setValue, tenantID, totalTimeForRule, evaluationCount, isDisabled, failureMessages, exceptions, finalSet, null);
    }

    private static void RaiseEvent(EventPublisher eventPublisher, EventDetails eventDetails, string senderName, bool successEvent, T successValue, T failureValue, string tenantID, List<Exception> exceptions, CancellationToken cancellationToken)
    {
        var eventWhenType = eventDetails.EventWhenType;

        if ((eventWhenType == EventWhenType.OnSuccess && successEvent == true) || (eventWhenType == EventWhenType.OnFailure && successEvent == false) || (eventWhenType == EventWhenType.OnSuccessOrFailure))
        {
            List<Exception> executionExceptions = [.. exceptions];
            var eventType = Type.GetType(eventDetails.EventTypeName)!;
            var stringType = typeof(string);
            var paramType = typeof(T);
            var constructorInfo = eventType.GetConstructor([stringType, typeof(bool), paramType, paramType, stringType, typeof(List<Exception>)]);
            var eventToPublish = (RuleEventBase<T>)constructorInfo!.Invoke([senderName, successEvent, successValue!, failureValue!, tenantID, executionExceptions]);

            _ = Task.Run(() => eventPublisher(eventToPublish, cancellationToken));

        }
    }

    private static List<Exception> GetExecutionExceptions(ConditionSetResult<T> setResult)
    {
        var exceptions = new List<Exception>();
        var result = setResult;

        while (result != null)
        {
            if (result.Exceptions.Count > 0) exceptions.AddRange(result.Exceptions);
            result = result.PreviousSetResult;
        }

        return exceptions;
    }

    internal JsonRule<T> ToJsonRule()

        => JsonRuleConverter.JsonRuleFromRule(this);

    ///<inheritdoc />
    public string ToJsonString(bool writeIndented = false, bool useEscaped = true)

        => JsonSerializer.Serialize<JsonRule<T>>(ToJsonRule(), new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented =  writeIndented,
            Encoder = useEscaped ? JavaScriptEncoder.Default : JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });


    internal Rule<T> DeepCloneRule()
    {
        //if (this.ConditionSets.Count == 0 || ConditionSets[0].BooleanConditions is null) throw new InCompleteRuleException(GlobalStrings.In_Complete_Rule_Conversion_Exception_Message);

        EventDetails? ruleEventDetails = this.RuleEventDetails?.DeepCloneEvent();

        var ruleClone = new Rule<T>(this.RuleName, this.FailureValue, ruleEventDetails, this.TenantID, this.CultureID);

        foreach (var conditionSet in this.ConditionSets)
        {
            var conditionClone = BooleanConditionBase.DeepCloneCondition(conditionSet.BooleanConditions);
            var conditionSetClone = new ConditionSet<T>(conditionSet.SetName, conditionSet.SetValue, conditionClone);

            ruleClone.OrConditionSet(conditionSetClone);
        }

        return ruleClone;
    }

}
