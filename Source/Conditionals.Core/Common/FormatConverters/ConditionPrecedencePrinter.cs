using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Common.Seeds;
using System.Text;

namespace Conditionals.Core.Common.FormatConverters;

/// <summary>
/// An implementation of the <see cref="IConditionPrecedencePrinter"/> interface. 
/// Initializes a new instance of the <see cref="ConditionPrecedencePrinter"/> class with the specified <see cref="PrecedencePrintType" /> enumerated value.
/// </summary>
/// <param name="printType">The <see cref="PrecedencePrintType" /> enumerated value.</param>
public class ConditionPrecedencePrinter(PrecedencePrintType printType = PrecedencePrintType.ConditionNameOnly) : IConditionPrecedencePrinter
{
    private readonly StringBuilder _stringBuilder = new();
    private readonly PrecedencePrintType _printType = printType;

    ///<inheritdoc />
    public string PrintPrecedenceOrder(BooleanConditionBase booleanCondition)
    {
        _stringBuilder.Clear();

        if (booleanCondition == null) return String.Empty;

        BuildPath(booleanCondition);

        return _stringBuilder.ToString();
    }
    private void BuildPath(BooleanConditionBase booleanCondition)
    {
        switch (booleanCondition)
        {
            case AndAlsoConditions: WriteAndAlsoCondition((AndAlsoConditions)booleanCondition); break;
            case OrElseConditions: WriteOrElseCondition((OrElseConditions)booleanCondition); break;
            default: WriteCondition(booleanCondition); break;
        }
    }
    private void WriteOrElseCondition(OrElseConditions orElseCondition)
    {
        _stringBuilder.Append('(');
        BuildPath(orElseCondition.Left);
        _stringBuilder.Append($" {OperatorType.OrElse.ToString()} ");
        BuildPath(orElseCondition.Right);
        _stringBuilder.Append(')');
    }

    private void WriteAndAlsoCondition(AndAlsoConditions andAlsoCondition)
    {
        _stringBuilder.Append('(');
        BuildPath(andAlsoCondition.Left);
        _stringBuilder.Append($" {OperatorType.AndAlso.ToString()} ");
        BuildPath(andAlsoCondition.Right);
        _stringBuilder.Append(')');
    }
    private void WriteCondition(BooleanConditionBase booleanCondition)
    {
        var condition = booleanCondition as ICondition ?? throw new NullReferenceException(GlobalStrings.ICondition_Cast_Null_Reference_Exception_Message);

        var printString = _printType switch
        {
            PrecedencePrintType.ConditionNameOnly => condition.ConditionName,
            _ => condition.ExpressionToEvaluate
        };

        _stringBuilder.Append(printString);
    }
}
