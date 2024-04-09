using Conditionals.Core.Common.Seeds;
using System.Runtime.CompilerServices;

namespace Conditionals.Core.Common.Validation;

internal static class Check
{
    public static T ThrowIfNullEmptyOrWhitespace<T>(T argument, [CallerArgumentExpression(nameof(argument))] string argumentName = "")

        => argument is null || typeof(T).Name == "String" && string.IsNullOrWhiteSpace(argument as string)
                ? throw new ArgumentException(GlobalStrings.Argument_Null_Empty_Exception_Message, argumentName)
                    : argument;

}
