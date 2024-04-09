using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.Extensions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.ConsoleDemos.Areas.Conditions;
using Conditionals.Core.ConsoleDemos.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.StaticData;
using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Chaining;

public class ChainingRuleResults(ConditionEngine conditionEngine)
{
    private readonly ConditionEngine _conditionEngine = conditionEngine;

    public async Task RunChainingResults()
    {
        /*
            * In various demos we have used extension methods on the RuleResult<T> in both sync and async versions to run actions on failure, success or on either case. 
            * These methods always returned the original result.  There are also overloaded variants of these that allow us to run rules and then operate on those results.
            * Nb as both the RuleResult<T> and ConditionResult<T> takes a type parameter as well as having properties for holding previous typed results currently when 
            * chaining new rule evaluations, the types must be the same i.e there is no Map<T1,T2> overloads available
        */

        var probeConditions = new PredicateCondition<Probe>("ProbeConditions", p => p.ResponseTimeMs < 10 && p.ErrorCount < 3, "Probe ID: @{ProbeID} is starting to fail")
                                                .AndAlso(new CustomCondition<Probe>("ValueCondition", "CalibrationTest", "Probe outside of expected norm", nameof(ProbeConditionEvaluator),
                                                                new Dictionary<string, string> { ["MeanValue"]="50", ["MinValue"]="20", ["MaxValue"]="80" }))
                                                .AndAlso(new PredicateCondition<Probe>("BatteryCondition", d => d.BatteryLevel >= 5, "Low battery", EventDetails.Create<DeviceConditionEvent>(EventWhenType.OnFailure)));
        /*
            * Make sure we have the custom evaluator registered, incase the other demo has been commented out, the register uses an AddOrUpdate so its not an issue. 
            * The evaluator used DI, so it has been registered in the containers in the program file
        */
        _conditionEngine.RegisterCustomEvaluatorForDependencyInjection(nameof(ProbeConditionEvaluator), typeof(ProbeConditionEvaluator));//its a closed generic 

        var deviceRule = new Rule<None>("DeviceHealthRule", None.Value, new ConditionSet<None>("ProbeHealth", None.Value, probeConditions), EventDetails.Create<DeviceRuleEvent>());

        _conditionEngine.AddOrUpdateRule(deviceRule);

        var conditionEventSubscription = _conditionEngine.SubscribeToEvent<DeviceConditionEvent>(DeviceConditionEventHandler);
        var ruleEventSubscription      = _conditionEngine.SubscribeToEvent<DeviceRuleEvent>(DeviceRuleEventHandler); 

        var tenantDevice        = DemoData.GetTenantDevice(101);//The data is for a specific Tenant, the rule is for any tenant
        var primaryProbeData    = ConditionDataBuilder.AddForAny(tenantDevice.Probes[0]).Create(tenantDevice.TenantID.ToString());
        var secondaryProbeData  = ConditionDataBuilder.AddForAny(tenantDevice.Probes[1]).Create(tenantDevice.TenantID.ToString());

        /*
            * Be careful when chaining and using the Tee type methods that allow you to run an action and to return the result acted upon. For example the code below works fine as 
            * the primary probe fails and it prints the correct message, it runs the next rule which passes and prints the correct message. But lets say the primary probe succeeds, 
            * we would then see Primary probe is ok, the OnFailure rule would not run, the rule result now gets passed to the final OnResult which would printout Secondary probe is Ok.
            * even though it was never run.
        */
        WriteLine("---- First run -----");
        var lastResult = await _conditionEngine.EvaluateRule<None>("DeviceHealthRule", primaryProbeData)
                                    .OnResult(act_onSuccess: result => WriteLine("Primary probe is OK"),
                                             act_onFailure: result => WriteLine($"Primary probe failing message: {String.Concat("\r\n", String.Join("\r\n", result.FailureMessages), "\r\nChecking secondary probe\r\n")}"))
                                    .OnFailure("DeviceHealthRule", _conditionEngine, secondaryProbeData)
                                    .OnResult(act_onSuccess: result => WriteLine("Secondary probe is OK"),
                                              act_onFailure: result => WriteLine($"Secondary probe failing message: {String.Concat("\r\n", String.Join("\r\n", result.FailureMessages))}"));

        await Task.Delay(25);// just added to keep event output together;

        WriteLine();
        /*
            * We could also obtain the same result and messages by checking the previous result in a statement block in the code
        */
        WriteLine("---- Second run -----");
        lastResult = await _conditionEngine.EvaluateRule<None>("DeviceHealthRule", primaryProbeData)
                                .OnResult(act_onSuccess: result => WriteLine("Primary probe is OK"),
                                        act_onFailure: result => WriteLine($"Primary probe failing message: {String.Concat("\r\n", String.Join("\r\n", result.FailureMessages), "\r\nChecking secondary probe\r\n")}"))
                                .OnFailure("DeviceHealthRule", _conditionEngine, secondaryProbeData)
                                .OnResult(act_onSuccess: result =>
                                {
                                    var message = result.PreviousRuleResult == null ? "Primary probe is OK" : "Secondary probe OK";
                                    WriteLine(message);
                                    WriteLine();
                                },
                                act_onFailure: result =>
                                {
                                    var message = result.PreviousRuleResult == null ? "Primary probe failing message" : "Secondary probe failing message";
                                    WriteLine($"{message}: {String.Concat("\r\n", String.Join("\r\n", result.FailureMessages))}");
                                    WriteLine();
                                });

        await Task.Delay(25);// just added to keep event output together;

        /*
            * Another way to achieve the same results is to use another extension that accepts a Func<RuleResult<T>,Task<RuleResult<T>>
            * so we can nest other chains in a statement block.
            * The chains can be any depth you like.
        */

        WriteLine("---- Last run -----");
        lastResult =  await _conditionEngine.EvaluateRule<None>("DeviceHealthRule", primaryProbeData)
                                            .OnSuccess(_ => Console.WriteLine("Primary probe is OK"))
                                            .OnFailure(async (result) =>
                                            {
                                                Console.WriteLine($"Primary probe failing message: {result.FailureMessages[0] +"\r\n"}");
                                                Console.WriteLine("Checking secondary probe\r\n");

                                                return await _conditionEngine.EvaluateRule<None>("DeviceHealthRule", secondaryProbeData)
                                                                .OnSuccess(_ => Console.WriteLine("Secondary probe is OK"))
                                                                .OnFailure(result => Console.WriteLine($"Secondary probe failure message, {result.FailureMessages[0] + "\r\n"}"));
                                            });



        conditionEventSubscription.Dispose();
        ruleEventSubscription.Dispose();

        /*
            * The rule event runs twice each time as its success or failure and we run two rules.
            * The condition just gets run once for each as its only on failure and only on the single batter condition.
            * As all events are async fire and forgot where the messages get written may not be the same each time.
        */ 
    }

    private async Task DeviceConditionEventHandler(DeviceConditionEvent deviceConditionEvent, CancellationToken cancellationToken)
    {
        _= deviceConditionEvent.TryGetData(out var data);

        await Console.Out.WriteLineAsync($"Handled the event locally for the {deviceConditionEvent.SenderName} condition, probe id {(data as Probe)?.ProbeID} owned by Tenant ID: {deviceConditionEvent.TenantID}");
    }
    private async Task DeviceRuleEventHandler(DeviceRuleEvent deviceRuleEvent, CancellationToken cancellationToken)
    {
        await Console.Out.WriteLineAsync($"Handled the event locally for rule: {deviceRuleEvent.SenderName} which evaluated to {deviceRuleEvent.IsSuccessEvent} for Tenant ID: {deviceRuleEvent.TenantID}");
    }
}
