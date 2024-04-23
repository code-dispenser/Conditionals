namespace Conditionals.Core.Common.Seeds;

internal static class GlobalStrings
{
    public const string Default_TenantID    = "All_Tenants";
    public const string Default_CultureID   = "en-GB";
    
    public const string Not_Available_Text = "N/A";

    public const string Predicate_Condition_Evaluator = "PredicateConditionEvaluator";
    public const string Regex_Condition_Evaluator     = "RegexConditionEvaluator";

    public const string Regex_Options_Key             = "RegexOptions";
    public const string Regex_Split_Char              = "|";
    public const string Regex_Split_Chars             = "[IsMatch]";

    public const string ICondition_Cast_Null_Reference_Exception_Message    = "The boolean condition could not be cast to an ICondition.";
    public const string Argument_Null_Empty_Exception_Message               = "The argument cannot be null or empty.";
    public const string Missing_Condition_Data_Exception_Message            = "The condition '{0}' is missing the following data context type: {1}.";
    public const string Missing_All_Condition_Data_Exception_Message        = "Cannot proceed without condition data.";
    public const string Precedence_Printer_Exception_Message                = "Error trying to create the precedence string using the precedence printer: '{0}'. Exception message: {1}";
    public const string Invalid_Boolean_Condition_Type_Exception_Message    = "Invalid boolean condition type. Please see Inner exception for more details.";
    public const string Json_deserialized_To_Null_Exception_Message         = "The data context deserialized to null, check that the input was not null.";
    public const string Missing_Evaluator_Resolver_Exception_Message        = "You cannot perform evaluations without obtaining an evaluator via an evaluation resolver.";
    public const string Missing_Evaluator_Exception_Message                 = "No evaluator named: {0} was found and/or was created.";
    public const string Raise_Event_Exception_Message                       = "Trying to create and raise the event: {0} caused an exception, please see the inner exception for more details.";
    public const string Disposing_Removed_Item_Exception_Message            = "An exception occurred whilst trying to dispose of the item '{0}' that was removed from cache; please see the inner exception for details.";
    public const string No_Rule_In_Cache_Exception_Message                  = "The rule named {0} as not found in the condition engines cache.";
    public const string Predicate_Condition_Compilation_Exception_Message   = "The compiled flag is set to false when it should be true for a predicate condition. Check that the json rule condition has the IsLambdaPredicate set to true";
    public const string Missing_Condition_Evaluator_Exception_Message       = "The condition evaluator named '{0}' has not been registered with the condition engine or could not be created.";
    public const string Rule_From_Json_Exception_Message                    = "The rule could not be created from the json string; please see the inner exception for details.";
    public const string Missing_Expression_ToEvaluate_Exception_Message     = "The condition named '{0}' is missing it's ExpressionToEvaluate property value.";
    public const string Context_Type_Assembly_Not_Found_Exception_Message   = "The assembly for the type of data context used for the condition {0} could not be found in the assemblies for the current app domain.";
    public const string Event_Not_Found_Exception_Message                   = "The event named {0} could not be found whilst ingesting the json rule";
    public const string Invalid_System_DataType_Exception_Message           = "The data type for the rule named: {0} is invalid.";

    public const string Missing_ConditionSets_Exception_Message             = "No condition sets were found in the rule.";

    //public const string In_Complete_Rule_Conversion_Exception_Message       = "A rule needs to contain at least one condition set with a condition before it can be converted";

    public const string Rule_Name_Property_Is_Missing_Or_Null_Message       = "No rule name";


    public const string Default_Condition_Failure_Message                   = "Condition failed";

    public const string CacheKey_Part_ConditionCreator = "ConditionCreator";
    public const string CacheKey_Part_Rule = "Rule";
    public const string CacheKey_Part_Evaluator = "Evaluator";
    public const string CacheKey_Part_Evaluator_Type = "EvaluatorType";
    public const string CacheKey_Part_Evaluator_Type_DI = "EvaluatorType_DI";

}
