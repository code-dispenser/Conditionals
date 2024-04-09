using Conditionals.Core.Common.Seeds;

namespace Conditionals.Core.Common.Models;


internal class JsonRule<T>
{
    public string RuleName                  { get; set; } = default!;
    public string TenantID                  { get; set; } = GlobalStrings.Default_TenantID;
    public string CultureID                 { get; set; } = GlobalStrings.Default_CultureID;
    public bool IsDisabled                  { get; set; } = false;
    public T FailureValue                   { get; set; } = default!;
    public string ValueTypeName             { get; set; } = default!;
    public JsonEventDetails? RuleEventDetails       { get; set; } = null;
    public List<JsonConditionSet<T>> ConditionSets  { get; set; } = [];

}


internal class JsonEventDetails
{
    public string EventTypeName { get; set; } = default!;
    public string EventWhenType { get; set; } = default!;
}
internal class JsonConditionSet<T>
{
    public string SetName   { get; set; } = default!;
    public T SetValue       { get; set; } = default!;

    public JsonCondition BooleanConditions { get; set; } = null!;


}
internal class JsonCondition
{
    public string? Operator             { get; set; }
    public JsonCondition? LeftOperand   { get; set; }
    public JsonCondition? RightOperand  { get; set; }
    public string? ConditionName        { get; set; }
    public string? ContextTypeName      { get; set; }
    public string? ExpressionToEvaluate { get; set; }
    public string? FailureMessage       { get; set; }
    public string? EvaluatorTypeName    { get; set; }
    public string? ConditionType        { get; set; }

    public Dictionary<string, string>? AdditionalInfo { get; set; } = [];
    public JsonEventDetails? ConditionEventDetails    { get; set; } = null;

}