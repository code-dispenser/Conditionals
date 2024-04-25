[![.NET](https://github.com/code-dispenser/Conditionals/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/code-dispenser/Conditionals/actions/workflows/dotnet.yml) [![Coverage Status](https://coveralls.io/repos/github/code-dispenser/Conditionals/badge.svg?branch=main)](https://coveralls.io/github/code-dispenser/Conditionals?branch=main) [![Nuget download][download-image]][download-url]

[download-image]: https://img.shields.io/nuget/dt/Conditionals.Core
[download-url]: https://www.nuget.org/packages/Conditionals.Core

<h1>
<img src="https://raw.github.com/code-dispenser/Conditionals/main/Assets/icons-64.png" align="center" alt="Conditionals icon" /> Conditionals
</h1>
<!--
# ![icon](https://raw.githubusercontent.com/code-dispenser/Conditionals/main/Assets/icons-64.png) Conditionals
-->
<!-- H1 for git hub, but for nuget the markdown is fine as it centers the image, uncomment as appropriate and do the same at the bottom of this file for the icon author -->

## Overview

Conditionals is essentially a Boolean expression-based rules engine, based on my other project [Devs' Rule](https://github.com/code-dispenser/DevsRule). It operates by evaluating conditions, which can be conceptualised as "condition trees." These 
trees consist of left and right operands, combined with the logical short-circuiting AndAlso (&&) and OrElse (||) operators.

**Note:** Conditionals is my preferred library. The main differences include typed result values, condition expression trees rather than just conditions ANDed together, the removal of events that 
you could wait on and a completely different JSON structure due to the condition trees.

Rules can be created in code (the preferred method) and exported (or manually created) as JSON files, either code based rules or JSON rule files can be ingested by the condition engine. This provides flexibility by allowing rule changes without 
the need to redeploy the application. Simply supply the condition engine periodically with any rule changes. These changes can be fetched at runtime from a file store or a database, for example.

A **Rule** ([Rule&lt;T&gt;](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Rules/Rule%5BT%5D.cs)) is comprised of one or more **ConditionSets** ([ConditionSet&lt;T&gt;](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Conditions/ConditionSet%5BT%5D.cs)), with each condition set holding a condition tree formed using a base class [BooleanConditionBase](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Conditions/BooleanConditionBase.cs). Condition sets within a rule are processed using short-circuiting OrElse (||) logic. This means that when a condition set evaluates to true, processing will end and the rule result will be returned. Otherwise, the next condition set will be evaluated.

Each **Condition** ([Condition&lt;TContext&gt;](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Conditions/Condition%5BT%5D.cs)) within a Boolean expression condition tree is evaluated using a **ConditionEvaluator** derived from ([ConditionEvaluatorBase&lt;TContext&gt;](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Evaluators/ConditionEvaluatorBase%5BT%5D.cs)) class. The [ConditionEngine](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Engine/ConditionEngine.cs) contains a lambda predicate condition evaluator ([PredicateConditionEvaluator&lt;TContext&gt;](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Evaluators/PredicateConditionEvaluator%5BT%7D.cs)) and 
a regular expression condition evaluator ([RegexConditionEvaluator&lt;TContext&gt;](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Evaluators/RegexConditionEvaluator%5BT%5D.cs)). You can add your own custom evaluators to enhance both lambda predicate conditions or for the evaluation of your own custom expression conditions. Custom evaluators can use dependency 
injection via your chosen IOC container. In essence, a custom condition could be anything which is then evaluated with the appropriate evaluator, providing a true or false [EvaluationResult](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Common/Models/AllSimpleTypes.cs) to 
be passed back up the evaluation chain.

There are essentially three types of conditions, a Lambda **PredicateCondition** ([PredicateCondition&lt;TContext&gt;](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Conditions/PredicateConditions%5BT%5D.cs)), **RegexCondition** ([RegexCondition&lt;TContext&gt;](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Conditions/RegexCondition%5BT%5D.cs)) and **CustomCondition** ([CustomCondition&lt;TContext&gt;](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Conditions/CustomConditions%5BT%5D.cs)), each of which returns an [EvaluationResult](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Common/Models/AllSimpleTypes.cs)


The normal/full processing path in pseudocode would be as follows, but you can perform an evaluation at any stage if preferred:

```
var result = await ConditionEngine.Evaluate -> (get)Rule.Evaluate -> (foreach)ConditionSet.Evaluate -> (recurse)BooleanConditionTree.Evaluate
```

Both rules and individual conditions can have an associated event. These events, depending on configuration, are raised on success, on failure or on either. Events can be subscribed to 
for use in forms and view models which can handle the events using local event handlers. Additionally, there's the ability to register dynamic event handlers. These dynamic event handlers are registered with your 
chosen IOC container and are instantiated with any injected dependencies every time their associated event is published via the condition engine.

Once a rule is evaluated, a rule result ([RuleResult&lt;TValue&gt;](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Common/Models/RuleResult%5BT%5D.cs)) is returned containing a boolean indicating success or failure, along with any associated success or failure output value. The rule results contains the full 
evaluation path, showing information regarding each condition evaluation, timings, failure messages, exceptions, and the input data used, etc.

The library also contains extension methods contained within the [RuleResultExtensions](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Common/Extensions/RuleResult%5BT%5DExtensions.cs) static class targeting the rule result class so you can chain rules together, running other rules dependant on the previous result as well as 
the ability to chain void actions that just pass along the previous result.

## Installation

Download and install the latest version of the [Conditionals.Core](https://www.nuget.org/packages/Conditionals.Core) package from [nuget.org](https://www.nuget.org/) using your preferred client tool.

## Example usage

At its simplest, it's a matter of creating an instance of the [ConditionEngine](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Engine/ConditionEngine.cs), adding a rule, and then having that rule evaluated with your instance data. Rules are built bottom up by creating and adding conditions to condition sets and then adding the conditions sets to a rule. A Rule can have a failure value (default) for failing conditions with its overall success value being assigned from the passing condition set.

For the majority of applications it is envisaged that the [ConditionEngine](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Engine/ConditionEngine.cs) will be added to an IOC container as a Singleton/SingleInstance and injected into the required areas of the application, however, there is no technical reason preventing you from having multiple isolated instances of the [ConditionEngine](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Engine/ConditionEngine.cs), each maintaining its own set of cached rules and evaluators if that better meets your requirements.

**Note:** The following is the first example taken from demo project included in the [Conditionals Git repository](https://github.com/code-dispenser/Conditionals/tree/main) which provides examples for all aspects of the library 
```c#
var conditions = new PredicateCondition<Customer>("AgeCondition", c => new DateTime(c.DOB.Year, c.DOB.Month, c.DOB.Day).AddYears(18) < DateTime.Now, "You must be over 18 to apply")
                    .AndAlso(new PredicateCondition<Address>("CountryCondition", a => a.Country == "United Kingdom", "You must be a resident of the United Kingdom"))
                    .AndAlso(new PredicateCondition<OrderHistoryView>("OrderCondition", o => o.TotalOrders >= 5, "You must have made at least five purchases against your account"));
/*
    * conditions form boolean expression pairs that short-circuit using AndAlso (&&) or OrElse (||) i.e the above nesting has a left and right, with the left having a left and right
    * ((AgeCondition AndAlso CountryCondition) AndAlso OrderCondition). There can be any depth of these pairings which will be shown/discussed later.
*/ 
        
var storeCardRule = new Rule<None>("StoreCreditCardRule",None.Value,new ConditionSet<None>("ApplicantRequirements",None.Value,conditions));

/*
    * We need three separate data contexts for this rule so we will use the ConditionDataBuilder. As these are three separate data types and only a single instance of each
    * we do not need match the type (instance of type) to the condition (by name). Again this will be discussed later, these can be added in any order.
*/
  
var customerID = 2;//Choose customer with ID 1,2,3, or 4
var conditionData = ConditionDataBuilder.AddForAny(DemoData.GetAddress(customerID))
                                            .AndForAny(DemoData.GetOrderHistory(customerID))
                                                .AndForAny(DemoData.GetCustomer(customerID))
                                                    .Create();

_conditionEngine.AddOrUpdateRule(storeCardRule);

/*
        * The return type is a RuleResult<None> meaning its a result that has no success or failure values just a bool IsSuccess property.
        * As we are using chaining via the extension methods we can access the result directly from Action<RuleResult<None>> so in this instance
        * we can just discard the returned result.
*/

_ = await _conditionEngine.EvaluateRule<None>(storeCardRule.RuleName, conditionData)
                            .OnResult(success => WriteLine($"Applicant {customerID}, application approved in {success.RuleTimeMilliseconds}ms with {success.EvaluationCount} evaluations"),
                                        failure =>
                                        {
                                            WriteLine($"Applicant {customerID} application rejected. {failure.EvaluationCount} evaluation(s) in {failure.RuleTimeMilliseconds}ms");
                                            WriteLine($"Rejected due to: {String.Join("/r/n", failure.FailureMessages)}");

                                        });
```

**Note:** Once a rule is cached in the condition engine we only need to call the condition engine with the name of the rule, passing in the condition data.
```c#
var ruleResult = await _conditionEngine.EvaluateRule<None>(""StoreCreditCardRule", conditionData)
```
Rules in JSON format can also be added using the **IngestRuleFromJson&lt;T&gt;(string ruleJson)** or **IngestRuleFromJson(string ruleJson)** methods on the condition engine. These methods transform the JSON into a **Rule** and 
then adds it using the **AddOrUpdateRule** method.

The [Rule&lt;T&gt;](https://github.com/code-dispenser/Conditionals/blob/main/Source/Conditionals.Core/Areas/Rules/Rule%5BT%5D.cs) class also has a **ToJsonString** method that you can use to output the rule to JSON. These JSON strings/files can then be altered and imported back into the running system 
when needed, updating the existing rules.

In conjunction with the [documentation](https://github.com/code-dispenser/Conditionals/wiki), it is recommended that you download the source code from the [Conditionals Git repository](https://github.com/code-dispenser/Conditionals) 
and explore the scenarios within the demo project. These sample scenarios and their comments should answer most of your questions.

Any feedback, positive or negative, is welcome, especially surrounding scenarios/usage.

## Acknowledgments

Currently, this library uses the method "DynamicExpressionParser.ParseLambda" from the [System.Linq.Dynamic.Core project](https://www.nuget.org/packages/System.Linq.Dynamic.Core) to create the 
compiled lambda predicate from the string representation in any JSON rule files used. Many thanks to all of the contributors on that project for making it much easier for me to create this 
project/nuget package.


<img src="https://raw.githubusercontent.com/code-dispenser/Conditionals/main/Assets/icons-64.png" align="middle" height="32px" alt="Conditionals icon" />
<a target="_blank" href="https://icons8.com/icon/kxE6S5YOUvM6/if">If</a> icon by <a target="_blank" href="https://icons8.com">Icons8</a>

<!--
![icon](https://raw.githubusercontent.com/code-dispenser/Conditionals/main/Assets/icon-48.png) If [icon by Icons8](https://icons8.com)
-->



