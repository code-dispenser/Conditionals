using Conditionals.Core.Areas.Rules;

namespace Conditionals.Core.ConsoleDemos.Common.Utilities;

public static class GeneralUtils
{
    public static void WriteLineSeparator(int dashCount = 100)

        => Console.WriteLine(String.Concat("\r\n", new String('-', dashCount), "\r\n"));

    public static void WriteLine(string outputText = "")

        => Console.WriteLine(outputText);

    public static async Task WriteToJsonFile<T>(Rule<T> rule, string filePath, bool writeIndented = true, bool useEscaped = true)

        => await File.WriteAllTextAsync(filePath, rule.ToJsonString(writeIndented, useEscaped));


    public static async Task<string> ReadJsonRuleFile(string filePath)

        => await File.ReadAllTextAsync(filePath);
}
