using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.ConsoleDemos.Areas.Chaining;
using Conditionals.Core.ConsoleDemos.Areas.Conditions;
using Conditionals.Core.ConsoleDemos.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.TenantsAndCulture;

public class TenantsAndCulture(ConditionEngine conditionEngine)
{
    public readonly ConditionEngine _conditionEngine = conditionEngine; 

    public async Task RunTenantIDAndCulture()
    {
        /*
            * By default and only of concern for those running multitenant and perhaps multilingual applications
            * are the rule properties TenantID and CultureID. By default these are set to All_Tenants and en-GB respectively;
            * When adding rules to the condition engine, the engine will add and fetch the rule from its cache using 
            * the following cache key naming strategy [RuleName]_[TenantID]_[CultureID]. 
            * Using the defaults for a rule named Discounts the cache key would be Discounts_All_Tenants_en_GB.
            * 
            * To allow for failure messages that you may wish to show to end users the rule has a CultureID property
            * that you can use to store and filter rules that you may have created with condition failure messages in differing
            * Please note a RuleNotFound exception will be raised if a rule is not found.
            * Its advisable to use the condition engines ContainsRule method to check if using either the TenantID or CultureID
            * properties.
            * 
            * When evaluating rules, any associated rule event will be assigned the TenantID from the Rule.
            * Condition events are slightly different, they are assigned the TenantID from the ConditionData property, the reason for this
            * is that a rule could be created for all tenants, but then evaluated for each Tenant using tenant specific data etc 
        */

        var probeConditions = new PredicateCondition<Probe>("ProbeConditions", p => p.ResponseTimeMs < 10 && p.ErrorCount < 3, "Probe ID: @{ProbeID} is starting to fail", 
                                                            EventDetails.Create<DeviceConditionEvent>());

        var deviceRule = new Rule<None>("DeviceHealthRule", None.Value, new ConditionSet<None>("ProbeHealth", None.Value, probeConditions), EventDetails.Create<DeviceRuleEvent>(), "Our_Tenants", "en-US");
        /*
            * We named a rule earlier called "DeviceHealthRule" and added to the engine, if you are running all the demos then it will still be in the cache
            * It was added using the defaults, lets check using both the ContainsRule and TryGetRule methods.
        */

        var isRuleInCache = _conditionEngine.ContainsRule("DeviceHealthRule");
        var rule          = _conditionEngine.TryGetRule<None>("DeviceHealthRule", out var cachedRule) ? cachedRule : null;

        WriteLine($"Is the rule DeviceHealthRule (All_Tenants, en-GB) in cache: {isRuleInCache}");
        
        if (rule is not null) WriteLine($"The rule: {rule.RuleName}, TenantID: {rule.TenantID}, CultureID: {rule.CultureID} was retrieved from the condition engine."); 
        
        _conditionEngine.AddOrUpdateRule(deviceRule);//This adds a new rule as the TenantID and CultureID are different.
        
        isRuleInCache = _conditionEngine.ContainsRule("DeviceHealthRule");

        WriteLine($"Is the rule DeviceHealthRule (All_Tenants, en-GB) in cache: {isRuleInCache}");

        isRuleInCache = _conditionEngine.ContainsRule("DeviceHealthRule","Our_Tenants","en-US");

        WriteLine($"Is the rule DeviceHealthRule (Our_Tenants, en-US) in cache: {isRuleInCache}");
        WriteLine();

        var tenantDevice            = DemoData.GetTenantDevice(111);
        var probeData               = ConditionDataBuilder.AddForAny(tenantDevice.Probes[0]).Create(tenantDevice.TenantID.ToString());

        var ruleSubscription        = _conditionEngine.SubscribeToEvent<DeviceRuleEvent>(DeviceRuleEventHandler);
        var conditionSubscription   = _conditionEngine.SubscribeToEvent<DeviceConditionEvent>(DeviceConditionEventHandler); 

        var ruleResult              = await _conditionEngine.EvaluateRule<None>("DeviceHealthRule", probeData, CancellationToken.None, null, "Our_Tenants", "en-US");

        WriteLine($"The rule: {ruleResult.RuleName}, created for tenant: {ruleResult.TenantID}, evaluated to {ruleResult.IsSuccess}");
        WriteLine($"The data in the condition was for the tenant: {ruleResult.SetResultChain?.ResultChain?.TenantID}");
        WriteLine();

        async Task DeviceConditionEventHandler(DeviceConditionEvent deviceConditionEvent, CancellationToken cancellationToken)
        {
            _= deviceConditionEvent.TryGetData(out var data);

            await Console.Out.WriteLineAsync($"Handled the event locally for the condition: {deviceConditionEvent.SenderName}, probe id {(data as Probe)?.ProbeID} owned by Tenant ID: {deviceConditionEvent.TenantID}");
        }
        async Task DeviceRuleEventHandler(DeviceRuleEvent deviceRuleEvent, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync($"Handled the event locally for rule: {deviceRuleEvent.SenderName} which evaluated to {deviceRuleEvent.IsSuccessEvent} for Tenant ID: {deviceRuleEvent.TenantID}");
        }

        ruleSubscription.Dispose();
        conditionSubscription.Dispose();
    }
}
