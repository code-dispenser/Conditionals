using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Common.Utilities;
using Conditionals.Core.Common.Validation;
using System.Reflection;
using System.Text.Json;

namespace Conditionals.Core.Common.FormatConverters;

/// <summary>
/// Utility for converting Rules to Json and vice versa using an intermediary <see cref="JsonRule{T}" /> class.
/// </summary>
internal static class JsonRuleConverter
{

    #region JsonRule To Rule 

    
    internal static Rule<T> RuleFromJson<T>(string ruleJson)
    {
        ruleJson = Check.ThrowIfNullEmptyOrWhitespace(ruleJson);

        try
        {
            JsonRule<T>? jsonRule = JsonSerializer.Deserialize<JsonRule<T>>(ruleJson);
            return RuleFromJsonRule<T>(jsonRule!);
        }
        catch (MissingExpressionToEvaluateException) { throw; }
        catch (ContextTypeAssemblyNotFoundException) { throw; }
        catch (EventNotFoundException)               { throw; }
        catch (MissingConditionSetsException)        { throw; }
        catch (Exception exception)
        {
            throw new RuleFromJsonException(GlobalStrings.Rule_From_Json_Exception_Message, exception);
        }
    }

    internal static Rule<T> RuleFromJsonRule<T>(JsonRule<T> jsonRule)
    {
        var ruleName            = Check.ThrowIfNullEmptyOrWhitespace(jsonRule.RuleName);
        var failureValue        = jsonRule.FailureValue;
        var tenantID            = jsonRule.TenantID  ?? GlobalStrings.Default_TenantID;
        var cultureID           = jsonRule.CultureID ?? GlobalStrings.Default_CultureID;
        var ruleEventDetails    = EventDetailsFromJson(jsonRule.RuleEventDetails);
        int conditionSetCount   = jsonRule.ConditionSets.Count;

        if (jsonRule.RuleEventDetails != null && ruleEventDetails == null)
        {
            throw new EventNotFoundException(String.Format(GlobalStrings.Event_Not_Found_Exception_Message, jsonRule.RuleEventDetails.EventTypeName), null);//could not create the event details from the information in the JsonRule
        }
        var rule = (Rule<T>)Activator.CreateInstance(typeof(Rule<T>), ruleName, failureValue, ruleEventDetails, tenantID, cultureID)!;

        if (conditionSetCount == 0) throw new MissingConditionSetsException(GlobalStrings.Missing_ConditionSets_Exception_Message);

        for (int setIndex = 0; setIndex < conditionSetCount; setIndex++)
        {
            var jsonRuleConditionSet = jsonRule.ConditionSets[setIndex];
            var setName = jsonRuleConditionSet.SetName;
            var setValue = jsonRuleConditionSet.SetValue;

            var booleanConditions = ConvertToBooleanCondition(jsonRuleConditionSet.BooleanConditions);
            var conditionSet = (ConditionSet<T>)Activator.CreateInstance(typeof(ConditionSet<T>), [setName, setValue!, booleanConditions])!;

            rule.OrConditionSet(conditionSet);
        }

        return rule;
    }


    internal static BooleanConditionBase ConvertToBooleanCondition(JsonCondition jsonCondition)
    {
        return CreateBooleanConditionFromJson(jsonCondition);
    }

    private static BooleanConditionBase CreateBooleanConditionFromJson(JsonCondition jsonCondition)

            => jsonCondition.Operator switch
            {
                "AndAlso" => new AndAlsoConditions(
                    CreateBooleanConditionFromJson(jsonCondition.LeftOperand!),
                    CreateBooleanConditionFromJson(jsonCondition.RightOperand!)),

                "OrElse" => new OrElseConditions(
                    CreateBooleanConditionFromJson(jsonCondition.LeftOperand!),
                    CreateBooleanConditionFromJson(jsonCondition.RightOperand!)),

                _ when jsonCondition.ConditionName != null => CreateCondition(jsonCondition),

                _ => throw new ArgumentException("Invalid JSON condition")
            };

    private static BooleanConditionBase CreateCondition(JsonCondition jsonCondition)
    {
        /*
            * Originally and in DevsRule I used expressions and compiled Funcs to create the conditions which were then cached given the overhead to create them.
            * As doing that just seemed to add more complexity versus the performance benefits, bearing in mind you would need to create thousands of rule conditions before getting
            * the benefit I decided to switch back to using simpler CreateInstance code. See commented out code at end of file
        */
        _ = Check.ThrowIfNullEmptyOrWhitespace(jsonCondition.ConditionName);

        var assemblyTypeNames = GeneralUtils.AssemblyTypeNames;

        EventDetails? eventDetails = EventDetailsFromJson(jsonCondition.ConditionEventDetails);

        var failureMessage = false == String.IsNullOrWhiteSpace(jsonCondition.FailureMessage) ? jsonCondition.FailureMessage.Trim() : GlobalStrings.Default_Condition_Failure_Message;
        var evaluatorTypeName = false == String.IsNullOrWhiteSpace(jsonCondition.EvaluatorTypeName) ? jsonCondition.EvaluatorTypeName.Trim() : "N/A";

        if (jsonCondition.ConditionEventDetails != null && eventDetails == null)
        {
            throw new EventNotFoundException(String.Format(GlobalStrings.Event_Not_Found_Exception_Message, jsonCondition.ConditionEventDetails.EventTypeName));
        }

        if (true == String.IsNullOrWhiteSpace(jsonCondition.ExpressionToEvaluate)) throw new MissingExpressionToEvaluateException(String.Format(GlobalStrings.Missing_Expression_ToEvaluate_Exception_Message, jsonCondition.ConditionName));

        if (String.IsNullOrWhiteSpace(jsonCondition.ContextTypeName)) throw new ContextTypeAssemblyNotFoundException(String.Format(GlobalStrings.Context_Type_Assembly_Not_Found_Exception_Message, jsonCondition.ConditionName));

        var contextSearchName = jsonCondition.ContextTypeName.Contains('.') == true ? jsonCondition.ContextTypeName : String.Concat(".", jsonCondition.ContextTypeName);

        var assemblyQualifiedName = assemblyTypeNames.Where(t => t.fullName.EndsWith(contextSearchName!)).FirstOrDefault().assemblyQualifiedName ?? String.Empty;

        if (String.IsNullOrWhiteSpace(assemblyQualifiedName)) throw new ContextTypeAssemblyNotFoundException(String.Format(GlobalStrings.Context_Type_Assembly_Not_Found_Exception_Message, jsonCondition.ConditionName));

        var conditionEnumType = Enum.Parse<ConditionType>(jsonCondition!.ConditionType!);
        var contextType = Type.GetType(assemblyQualifiedName);
        var conditionContextType = typeof(Condition<>).MakeGenericType(contextType!);

        return (BooleanConditionBase)Activator.CreateInstance(conditionContextType, BindingFlags.NonPublic | BindingFlags.Instance, null,
                                                                    [jsonCondition.ConditionName, jsonCondition.ExpressionToEvaluate, conditionEnumType, failureMessage,
                                                                        evaluatorTypeName, jsonCondition.AdditionalInfo!.ToDictionary(k => k.Key, v => v.Value), eventDetails
                                                                    ], null)!;
    }

    private static EventDetails? EventDetailsFromJson(JsonEventDetails? jsonEvent)
    {
        if (jsonEvent == null || jsonEvent.EventTypeName == null) return null;

        string searchName = jsonEvent.EventTypeName.Contains('.') == true ? jsonEvent.EventTypeName : String.Concat(".", jsonEvent.EventTypeName);

        string? assemblyQualifiedName = (GeneralUtils.EventTypeNames.Where(e => e.fullName.EndsWith(searchName)).SingleOrDefault()).assemblyQualifiedName;

        EventWhenType eventWhenType = Enum.TryParse<EventWhenType>(jsonEvent.EventWhenType, out var eventWhenValue) == true ? eventWhenValue : EventWhenType.Never;

        return String.IsNullOrWhiteSpace(assemblyQualifiedName) == false ? new EventDetails(assemblyQualifiedName, eventWhenType) : null;
    }

    #endregion 

    #region Rule To JsonRule


    internal static JsonRule<T> JsonRuleFromRule<T>(Rule<T> rule)
    {
        var jsonRule = new JsonRule<T>
        {
            RuleName       = rule.RuleName,
            FailureValue   = rule.FailureValue,
            CultureID      = rule.CultureID,
            TenantID       = rule.TenantID,
            IsDisabled     = rule.IsDisabled,
            ValueTypeName  = typeof(T).Name,

            RuleEventDetails = FromEventDetails(rule.RuleEventDetails)
        };

        foreach (var conditionSet in rule.ConditionSets)
        {
            var jsonConditionSet = new JsonConditionSet<T>
            {
                SetValue = conditionSet.SetValue,
                SetName  = conditionSet.SetName,
                BooleanConditions = ConvertToJsonCondition(conditionSet.BooleanConditions)
            };

            jsonRule.ConditionSets.Add(jsonConditionSet);
        }

        return jsonRule;
    }

    internal static JsonCondition ConvertToJsonCondition(BooleanConditionBase booleanCondition)

        => CreateJsonConditionFromBooleanCondition(booleanCondition);

    private static JsonCondition CreateJsonConditionFromBooleanCondition(BooleanConditionBase booleanCondition)

        => booleanCondition switch
        {
            AndAlsoConditions andAlsoCondition => new JsonCondition
            {
                Operator        = OperatorType.AndAlso.ToString(),
                LeftOperand     = CreateJsonConditionFromBooleanCondition(andAlsoCondition.Left),
                RightOperand    = CreateJsonConditionFromBooleanCondition(andAlsoCondition.Right),
                AdditionalInfo  = null,
            },
            OrElseConditions orElseCondition => new JsonCondition
            {
                Operator        = OperatorType.OrElse.ToString(),
                LeftOperand     = CreateJsonConditionFromBooleanCondition(orElseCondition.Left),
                RightOperand    = CreateJsonConditionFromBooleanCondition(orElseCondition.Right),
                AdditionalInfo  = null,
            },
            ICondition condition => CreateJsonCondition(condition),
            _ => throw new InvalidBooleanConditionTypeException(GlobalStrings.Invalid_Boolean_Condition_Type_Exception_Message)
        };


    private static JsonCondition CreateJsonCondition(ICondition condition)

        => new()
        {
            AdditionalInfo          = condition.AdditionalInfo,
            ConditionEventDetails   = FromEventDetails(condition.EventDetails),
            ConditionName           = condition.ConditionName,
            ConditionType           = condition.ConditionType.ToString(),
            ContextTypeName         = condition.ContextType.FullName,
            EvaluatorTypeName       = condition.EvaluatorTypeName,
            ExpressionToEvaluate    = condition.ExpressionToEvaluate,
            FailureMessage          = condition.FailureMessage,
        };


    private static JsonEventDetails? FromEventDetails(EventDetails? eventDetails)
    {
        if (eventDetails == null) return null;

        var eventFullName = Type.GetType(eventDetails.EventTypeName)!.FullName!;
        var jsonEventDetails = new JsonEventDetails() { EventTypeName = eventFullName, EventWhenType = eventDetails.EventWhenType.ToString() };

        return jsonEventDetails;
    }

    #endregion


    #region Old Code For Reference Just leave commented Out

    //public static BooleanConditionBase BooleanConditionFromJsonCondition(JsonCondition jsonCondition, IReadOnlyList<(string assemblyQualifiedName, string fullName)> assemblyTypeNames)
    //{

    //    EventDetails? eventDetails = EventDetails.FromJsonRule(jsonCondition.ConditionEventDetails);

    //    if (false == String.IsNullOrWhiteSpace(jsonCondition.ConditionEventDetails?.EventTypeName) && eventDetails == null)
    //    {
    //        throw new EventNotFoundException(String.Format(GlobalStrings.Event_Not_Found_Exception_Message, jsonCondition.ConditionEventDetails?.EventTypeName));
    //    }

    //    var failureMessage = false == String.IsNullOrWhiteSpace(jsonCondition.FailureMessage) ? jsonCondition.FailureMessage.Trim() : "Condition failed";
    //    var evaluatorTypeName = false == String.IsNullOrWhiteSpace(jsonCondition.EvaluatorTypeName) ? jsonCondition.EvaluatorTypeName.Trim() : "N/A";

    //    var contextSearchName = jsonCondition?.ContextTypeName?.Contains('.') == true ? jsonCondition?.ContextTypeName : String.Concat(".", jsonCondition?.ContextTypeName);

    //    var assemblyQualifiedName = assemblyTypeNames.Where(t => t.fullName.EndsWith(contextSearchName!)).FirstOrDefault().assemblyQualifiedName ?? String.Empty;

    //    if (String.IsNullOrWhiteSpace(assemblyQualifiedName)) throw new ContextTypeAssemblyNotFound(String.Format(GlobalStrings.Context_Type_Assembly_Not_Found_Exception_Message, jsonCondition!.ConditionName));

    //    var contextType = Type.GetType(assemblyQualifiedName);

    //    Type[] typeArgs = { contextType! };

    //    MethodInfo methodInfo = typeof(TestExpressions).GetMethod(nameof(ConditionFromJsonCondition), BindingFlags.NonPublic | BindingFlags.Static)!
    //                                    .MakeGenericMethod(typeArgs);

    //    return (BooleanConditionBase)methodInfo.Invoke(null, new object[] { jsonCondition!, eventDetails! })!;

    //}
    //private static Condition<TContext> ConditionFromJsonCondition<TContext>(JsonCondition jsonCondition, EventDetails? eventDetails)
    //{
    //    var failureMessage = false == String.IsNullOrWhiteSpace(jsonCondition.FailureMessage) ? jsonCondition.FailureMessage.Trim() : "Condition failed";
    //    var evaluatorTypeName = false == String.IsNullOrWhiteSpace(jsonCondition.EvaluatorTypeName) ? jsonCondition.EvaluatorTypeName.Trim() : "N/A";

    //    var conditionType = Enum.Parse<ConditionType>(jsonCondition!.ConditionType!);
        
    //    //Create and then add to and check cache
    //    var conditionCreator = CreateConditionCreator<TContext>();

    //    return conditionCreator(jsonCondition.ConditionName!, jsonCondition.ExpressionToEvaluate!, conditionType, failureMessage, evaluatorTypeName, jsonCondition.AdditionalInfo, eventDetails!);
    //}
    //private static Func<string, string, ConditionType, string, string, Dictionary<string, string>, EventDetails, Condition<TContext>> CreateConditionCreator<TContext>()
    //{
    //    Type eventDetailsType   = typeof(EventDetails);
    //    Type conditionEnumType  = typeof(ConditionType);
    //    Type conditionCxtType   = typeof(Condition<>).MakeGenericType(typeof(TContext));
    //    Type stringType         = typeof(string);
    //    Type dictionaryType     = typeof(Dictionary<,>).MakeGenericType(stringType, stringType);
    //    Type boolType           = typeof(bool);
    //    Type[] paramTypes       = new Type[] { stringType, stringType, conditionEnumType, stringType, stringType,  dictionaryType, eventDetailsType };

    //    var conditionNameParam      = Expression.Parameter(stringType, "conditionName");
    //    var toEvaluateParam         = Expression.Parameter(stringType, "expressionToEvaluate");
    //    var failureMessageParam     = Expression.Parameter(stringType, "failureMessage");
    //    var evaluatorTypeNameParam  = Expression.Parameter(stringType, "evaluatorTypeName");
    //    var additionalInfoParam     = Expression.Parameter(dictionaryType, "additionalInfo");
    //    var eventDetailsParam       = Expression.Parameter(eventDetailsType, "eventDetails");
    //    var conditionTypeParam      = Expression.Parameter(conditionEnumType, "conditionType");

    //    //internal Condition(string conditionName, string expressionToEvaluate, ConditionType conditionType, string failureMessage, string evaluatorTypeName, Dictionary<string, string> additionalInfo, EventDetails? eventDetails = null)
    //    var constructor = conditionCxtType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, paramTypes)!;
    //    var newCondition = Expression.New(constructor, conditionNameParam, toEvaluateParam, conditionTypeParam, failureMessageParam, evaluatorTypeNameParam, additionalInfoParam, eventDetailsParam);

    //    var lambdaFunc = Expression.Lambda<Func<string, string, ConditionType, string, string, Dictionary<string, string>, EventDetails, Condition<TContext>>>
    //        (newCondition, conditionNameParam, toEvaluateParam, conditionTypeParam, failureMessageParam, evaluatorTypeNameParam, additionalInfoParam, eventDetailsParam);

    //    return lambdaFunc.Compile();

    //}


    #endregion


}



