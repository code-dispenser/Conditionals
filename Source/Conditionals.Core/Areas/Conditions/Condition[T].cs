using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Common.Validation;
using System.Diagnostics;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.Json;

namespace Conditionals.Core.Areas.Conditions;

///<inheritdoc cref="BooleanConditionBase" />
///<inheritdoc cref="ICondition"/>
/// <typeparam name="TContext">The type of data used for in the condition evaluation.</typeparam>
public class Condition<TContext> : BooleanConditionBase, ICondition
{
    private static readonly ParsingConfig _parsingConfig =  new() { AllowEqualsAndToStringMethodsOnObject = true };
    ///<inheritdoc />
    public Dictionary<string, string> AdditionalInfo { get; }//TODO immutable fix?
    ///<inheritdoc />
    public EventDetails? EventDetails   { get; }
    ///<inheritdoc />
    public string EvaluatorTypeName     { get; }
    ///<inheritdoc />
    public string ExpressionToEvaluate  { get; }
    ///<inheritdoc />
    public string ConditionName         { get; }
    ///<inheritdoc />
    public ConditionType ConditionType  { get; }
    ///<inheritdoc />
    public Type ContextType             { get; }
    ///<inheritdoc />
    public string FailureMessage        { get; }

    /// <summary>
    /// Gets the compiled func predicate.
    /// </summary>
    public Func<TContext, bool>? CompiledPredicate { get; }

    internal Condition(string conditionName, string expressionToEvaluate, ConditionType conditionType, string failureMessage, string evaluatorTypeName)

        : this(conditionName, expressionToEvaluate, conditionType, failureMessage, evaluatorTypeName, []) { }

    internal Condition(string conditionName, string expressionToEvaluate, ConditionType conditionType, string failureMessage, string evaluatorTypeName, Dictionary<string, string> additionalInfo)

        : this(conditionName, expressionToEvaluate, conditionType, failureMessage, evaluatorTypeName, additionalInfo, null) { }

    internal Condition(string conditionName, string expressionToEvaluate, ConditionType conditionType, string failureMessage, string evaluatorTypeName, Dictionary<string, string> additionalInfo, EventDetails? eventDetails = null)
    {

        ConditionName           = Check.ThrowIfNullEmptyOrWhitespace(conditionName).Trim();
        ExpressionToEvaluate    = Check.ThrowIfNullEmptyOrWhitespace(expressionToEvaluate);
        ConditionType           = conditionType;
        FailureMessage          = Check.ThrowIfNullEmptyOrWhitespace(failureMessage);
        EvaluatorTypeName       = Check.ThrowIfNullEmptyOrWhitespace(evaluatorTypeName).Trim();
        ContextType             = typeof(TContext);
        EventDetails            = eventDetails;
        AdditionalInfo          = additionalInfo == null ? [] : new Dictionary<string, string>(additionalInfo);

        if (conditionType == ConditionType.LambdaPredicate) CompiledPredicate = Condition<TContext>.BuildPredicateFromString(expressionToEvaluate);
    }

    private static Func<TContext, bool> BuildPredicateFromString(string conditionExpression)
    {
       
        string[] expressionParts = conditionExpression.Split("=>", StringSplitOptions.TrimEntries);

        var identifier = expressionParts[0];

        ParameterExpression parameter = Expression.Parameter(typeof(TContext), identifier);

        LambdaExpression lambdaExpression = DynamicExpressionParser.ParseLambda(_parsingConfig, [parameter], typeof(bool), conditionExpression);

        return (Func<TContext, bool>)lambdaExpression.Compile();
    }

    private async Task<EvaluationResult> EvaluateWith(IConditionEvaluator<TContext> evaluator, TContext data, string tenantID, CancellationToken cancellationToken)

        => await evaluator.Evaluate(this, data, cancellationToken, tenantID).ConfigureAwait(false);

    /// <summary>
    /// Used to evaluate each boolean condition in the condition tree.
    /// </summary>
    /// <param name="evaluatorResolver">An implementation of the <see cref="ConditionEvaluatorResolver" /> delegate that returns a condition evaluator by name and 
    /// context type <see cref="ConditionEvaluatorResolver" />.</param>
    /// <param name="conditionData">The data for all conditions <see cref="ConditionData" />.</param>
    /// <param name="eventPublisher">Optional, an implementation of the <see cref="EventPublisher"/> EventPublisher delegate for publishing events <see cref="EventPublisher" />.</param>
    /// <param name="previousResult">Optional, the previous instance of a <see cref="ConditionResult" /> class, the default is null</param>
    /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
    /// <exception cref="MissingAllConditionDataException"></exception>
    /// <exception cref="MissingEvaluatorResolverException"></exception>
    /// <returns>An asynchronous operation that returns an instance of the <see cref="ConditionResult" /> class.</returns>
    public sealed override async Task<ConditionResult> Evaluate(ConditionEvaluatorResolver evaluatorResolver, ConditionData conditionData, EventPublisher? eventPublisher = null, ConditionResult? previousResult = null, CancellationToken cancellationToken = default)
    {
        if (conditionData is null)      throw new MissingAllConditionDataException(GlobalStrings.Missing_All_Condition_Data_Exception_Message);
        if (evaluatorResolver is null)  throw new MissingEvaluatorResolverException(GlobalStrings.Missing_Evaluator_Resolver_Exception_Message);

        ConditionResult conditionResult;
        DataContext?    context     = default;
        List<Exception> exceptions  = [];

        long startEvalTicks;
        long endEvalTicks;
        long endingTicks;
        long evaluationMicroseconds = 0;
        long totalMicroseconds;

        var assemblyQualifiedName = ContextType.AssemblyQualifiedName;

        long startAllTicks = Stopwatch.GetTimestamp();

        try
        {
            context = Condition<TContext>.GetContextDataForCondition(conditionData, ConditionName);

            if (context is null) throw new MissingConditionDataException(string.Format(GlobalStrings.Missing_Condition_Data_Exception_Message, ConditionName, ContextType.Name));

            dynamic evaluatorInstance = evaluatorResolver(EvaluatorTypeName, ContextType) ?? throw new MissingEvaluatorException(String.Format(GlobalStrings.Missing_Evaluator_Exception_Message, this.EvaluatorTypeName));

            startEvalTicks                      = Stopwatch.GetTimestamp();
            EvaluationResult evaluationResult   = await EvaluateWith(evaluatorInstance, context.Data, conditionData.TenantID, cancellationToken);
            endEvalTicks                        = Stopwatch.GetTimestamp();

            if (evaluationResult.Exception is not null) exceptions.Add(evaluationResult.Exception);

            var failureMessage = evaluationResult.IsSuccess ? evaluationResult.FailureMessage
                                                : String.IsNullOrWhiteSpace(evaluationResult.FailureMessage) ? FailureMessage : evaluationResult.FailureMessage;
            if (this.EventDetails is not null)
            {
                try
                {
                    RaiseEvent(this.ConditionName, this.ContextType, eventPublisher, this.EventDetails, evaluationResult, context.Data, conditionData.TenantID, cancellationToken);
                }
                catch (Exception exception) 
                {
                    var eventName = Type.GetType(this.EventDetails.EventTypeName)!.Name;
                    exceptions.Add(new RaiseEventException(String.Format(GlobalStrings.Raise_Event_Exception_Message,eventName),exception));
                }
            }

            endingTicks             = Stopwatch.GetTimestamp();
            evaluationMicroseconds  = (endEvalTicks - startEvalTicks) / (Stopwatch.Frequency / 1_000_000);
            totalMicroseconds       = (endingTicks - startAllTicks) / (Stopwatch.Frequency / 1_000_000);

            conditionResult = new ConditionResult(ConditionName, assemblyQualifiedName!, ExpressionToEvaluate, context.Data, EvaluatorTypeName, evaluationResult.IsSuccess,
                                                  failureMessage!, evaluationMicroseconds, totalMicroseconds, conditionData.TenantID, previousResult, exceptions);
        }
        catch (Exception exception)
        {
            exceptions.Add(exception);
            endingTicks = Stopwatch.GetTimestamp();
            totalMicroseconds = (endingTicks - startAllTicks) / (Stopwatch.Frequency / 1_000_000);

            conditionResult = new ConditionResult(ConditionName, assemblyQualifiedName!, ExpressionToEvaluate, context?.Data, EvaluatorTypeName, false,
                                                        FailureMessage, evaluationMicroseconds, totalMicroseconds, conditionData.TenantID, previousResult, exceptions);
        }

        return await Task.FromResult(conditionResult);
    }

    private static DataContext? GetContextDataForCondition(ConditionData conditionData, string conditionName)
    {

        var namedData = conditionData.Contexts.Where(c => c.ConditionName == conditionName && c.Data.GetType() == typeof(TContext)).FirstOrDefault();

        return namedData ?? conditionData.Contexts.Where(c => c.Data.GetType() == typeof(TContext)).FirstOrDefault();

    }

    private static void RaiseEvent(string conditionName, Type contextType, EventPublisher eventPublisher, EventDetails eventDetails, EvaluationResult result, dynamic evaluationData, string tenantID, CancellationToken cancellationToken)
    {
        var eventWhenType = eventDetails.EventWhenType;

        if ((eventWhenType == EventWhenType.OnSuccess && result.IsSuccess == true) || (eventWhenType == EventWhenType.OnFailure && result.IsSuccess == false) || (eventWhenType == EventWhenType.OnSuccessOrFailure))
        {
            string? jsonDataClone           = null;
            Exception? conversionException  = null;

            try
            {
                jsonDataClone = JsonSerializer.Serialize(evaluationData,contextType);
            }
            catch (Exception ex) { conversionException = ex; }

            var executionExceptions = new List<Exception>();

            if (result.Exception != null) executionExceptions.Add(result.Exception);

            var eventType       = Type.GetType(eventDetails.EventTypeName);
            var stringType      = typeof(string);
            var constructorInfo = eventType!.GetConstructor([stringType, typeof(bool), stringType, stringType, typeof(List<Exception>), typeof(Exception)]);
            var eventToPublish  = (ConditionEventBase<TContext>)constructorInfo!.Invoke([conditionName, result.IsSuccess, jsonDataClone!, tenantID, executionExceptions, conversionException!]);

            _ = Task.Run(() => eventPublisher(eventToPublish, cancellationToken));

        }
    }


}
