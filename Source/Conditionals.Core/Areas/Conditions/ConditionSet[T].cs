using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Common.Validation;

namespace Conditionals.Core.Areas.Conditions;

///<inheritdoc cref="IConditionSet{T}" />

/// <summary>
/// Initialises a new condition set.
/// </summary>
/// <typeparam name="T">The data type of the set value.</typeparam>
/// <param name="setName">The name of the set.</param>
/// <param name="setValue">The value for the set.</param>
/// <param name="booleanCondition">The root of the condition tree. <see cref="BooleanConditionBase" /></param>
public class ConditionSet<T>(string setName, T setValue, BooleanConditionBase booleanCondition) : IConditionSet<T>
{
    ///<inheritdoc />
    public BooleanConditionBase BooleanConditions { get; } = Check.ThrowIfNullEmptyOrWhitespace(booleanCondition);
    ///<inheritdoc />
    public string SetName { get; } = Check.ThrowIfNullEmptyOrWhitespace(setName);
    ///<inheritdoc />
    public T     SetValue { get; } = Check.ThrowIfNullEmptyOrWhitespace(setValue);

    /// <summary>
    /// Used to evaluate the condition tree.
    /// </summary>
    /// <param name="evaluatorResolver">An implementation of the ConditionEvaluatorResolver delegate that returns a condition evaluator by name and context type <see cref="ConditionEvaluatorResolver" />.</param>
    /// <param name="conditionData">The data for all conditions <see cref="ConditionData" /></param>
    /// <param name="eventPublisher">Optional, an implementation of the EventPublisher delegate for publishing events <see cref="EventPublisher" />.</param>
    /// <param name="precedencePrinter">Optional, an implementation of the IConditionPrecedencePrinter <see cref="IConditionPrecedencePrinter" />
    /// that will create a string showing the order of precedence for the conditions that are to be evaluated. 
    /// </param>
    /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
    /// <exception cref="MissingAllConditionDataException"></exception>
    /// <exception cref="MissingEvaluatorResolverException"></exception>
    /// <returns>Asynchronous operation returning a <see cref="ConditionResult" /></returns>
    public async Task<ConditionSetResult<T>> Evaluate(ConditionEvaluatorResolver evaluatorResolver, ConditionData conditionData, EventPublisher? eventPublisher = null, IConditionPrecedencePrinter? precedencePrinter = null, CancellationToken cancellationToken = default)
    {
        if (conditionData is null) throw new MissingAllConditionDataException(GlobalStrings.Missing_All_Condition_Data_Exception_Message);
        if (evaluatorResolver is null) throw new MissingEvaluatorResolverException(GlobalStrings.Missing_Evaluator_Resolver_Exception_Message);

        var orderOfPrecedence = BuildPrecedenceString(booleanCondition, precedencePrinter);
        var conditionResult = await BooleanConditions.Evaluate(evaluatorResolver, conditionData, eventPublisher, null, cancellationToken);

        return BuildConditionSetResult(conditionResult, this.SetName, this.SetValue, orderOfPrecedence);
    }
    private static string BuildPrecedenceString(BooleanConditionBase booleanCondition, IConditionPrecedencePrinter? precedencePrinter)
    {
        try
        {
            if (precedencePrinter is null || booleanCondition is null) return GlobalStrings.Not_Available_Text;

            return precedencePrinter.PrintPrecedenceOrder(booleanCondition);
        }
        catch (Exception exception)
        {
            return string.Format(GlobalStrings.Precedence_Printer_Exception_Message, precedencePrinter!.GetType().Name, exception.Message);
        }
    }

    private static ConditionSetResult<T> BuildConditionSetResult(ConditionResult conditionResult, string setName, T setValue, string orderOfPrecedence)
    {
        List<Exception> exceptions = [];
        List<string> failureMessages = [];
        long totalTime = 0;
        int evaluationCount = 0;

        bool isSuccess = conditionResult.IsSuccess;

        var result = conditionResult;

        while (result != null)
        {
            if (result.Exceptions.Count > 0) exceptions.InsertRange(0,result.Exceptions);
            if (result.IsSuccess == false) failureMessages.Insert(0,result.FailureMessage);
            evaluationCount++;
            totalTime += result.TotalMicroseconds;

            result = result.ResultChain;
        }

        return new ConditionSetResult<T>(setName, setValue, isSuccess, evaluationCount, totalTime, orderOfPrecedence, conditionResult, [.. failureMessages], [.. exceptions]);
    }
}
