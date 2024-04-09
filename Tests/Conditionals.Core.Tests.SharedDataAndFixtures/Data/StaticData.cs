using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using System.Text.Json;

namespace Conditionals.Core.Tests.SharedDataAndFixtures.Data;

public static class StaticData
{
    public static Customer CustomerOne()    => new("CustomerOne", 111, 111.11M, 1);
    public static Customer CustomerTwo()    => new("CustomerTwo", 222, 222.22M, 2);
    public static Customer CustomerThree()  => new("CustomerThree", 333, 333.33M, 3);

    public static readonly string Customer_One_Name_Message = "The customer name should be CustomerOne";
    public static readonly string Customer_Two_Name_Message = "The customer name should be CustomerTwo";
    public static readonly string Customer_Three_Name_Message = "The customer name should be CustomerThree";

    public static readonly string Customer_One_No_Message = "The customer No. should be 111";
    public static readonly string Customer_Two_No_Message = "The customer No. should be 222";
    public static readonly string Customer_Three_No_Message = "The customer No. should be 333";


    public static readonly string Customer_One_Spend_Message = "The customer total spend should be 111.11";
    public static readonly string Customer_Two_Spend_Message = "The customer total spend should be 222.22";
    public static readonly string Customer_Three_Spend_Message = "The customer total spend should be 333.33";

    public static readonly string Customer_One_Member_Years = "The customer member years should be 1";
    public static readonly string Customer_Two_Member_Years = "The customer member years should be 2";
    public static readonly string Customer_Three_Member_Years = "The customer member years should be 3";


    public static Supplier SupplierOne()    => new("SupplierOne", 111, 111.11M);
    public static Supplier SupplierTwo()    => new("SupplierTwo", 222, 222.22M);
    public static Supplier SupplierThree()  => new("SupplierThree", 333, 333.33M);

    public static readonly string Supplier_One_Name_Message = "The supplier name should be SupplierOne";
    public static readonly string Supplier_Two_Name_Message = "The supplier name should be SupplierTwo";
    public static readonly string Supplier_Three_Name_Message = "The supplier name should be SupplierThree";

    public static readonly string Supplier_One_No_Message = "The supplier No. should be 111";
    public static readonly string Supplier_Two_No_Message = "The supplier No. should be 222";
    public static readonly string Supplier_Three_No_Message = "The supplier No. should be 333";

    public static readonly string Supplier_One_Spend_Message = "The supplier total spend should be 111.11";
    public static readonly string Supplier_Two_Spend_Message = "The supplier total spend should be 222.22";
    public static readonly string Supplier_Three_Spend_Message = "The supplier total spend should be 333.33";


    public static Person PersonUnder18() => new("John", "Doe", DateOnly.ParseExact("2020-01-01", "yyyy-MM-dd"));



    public static string CustomerOneAsJsonString()

    => JsonSerializer.Serialize<Customer>(StaticData.CustomerOne());

    public static readonly string JsonRuleText = """
                                                {
                                                  "RuleName": "RuleOne",
                                                  "TenantID": "All_Tenants",
                                                  "CultureID": "en-GB",
                                                  "IsDisabled": false,
                                                  "FailureValue": 10,
                                                  "ValueTypeName": "Int32",
                                                  "RuleEventDetails": {
                                                    "EventTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Events.RuleEventInt",
                                                    "EventWhenType": "OnFailure"
                                                  },
                                                  "ConditionSets": [
                                                    {
                                                      "SetName": "SetOne",
                                                      "SetValue": 42,
                                                      "BooleanConditions": {
                                                        "Operator": "AndAlso",
                                                        "LeftOperand": {
                                                          "Operator": null,
                                                          "LeftOperand": null,
                                                          "RightOperand": null,
                                                          "ConditionName": "CustomerName",
                                                          "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Customer",
                                                          "ExpressionToEvaluate": "c =\u003E (c.CustomerName == \u0022CustomerOne\u0022)",
                                                          "FailureMessage": "CustomerName should be CustomerOne",
                                                          "EvaluatorTypeName": "PredicateConditionEvaluator",
                                                          "ConditionType": "LambdaPredicate",
                                                          "AdditionalInfo": {},
                                                          "ConditionEventDetails": {
                                                            "EventTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Events.ConditionEventCustomer",
                                                            "EventWhenType": "OnSuccessOrFailure"
                                                          }
                                                        },
                                                        "RightOperand": {
                                                          "Operator": null,
                                                          "LeftOperand": null,
                                                          "RightOperand": null,
                                                          "ConditionName": "CustomerNo",
                                                          "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Customer",
                                                          "ExpressionToEvaluate": "Some expression",
                                                          "FailureMessage": "The customer number should be greater than 100",
                                                          "EvaluatorTypeName": "CustomEvaluator",
                                                          "ConditionType": "CustomExpression",
                                                          "AdditionalInfo": {
                                                            "Key": "Value"
                                                          },
                                                          "ConditionEventDetails": null
                                                        },
                                                        "ConditionName": null,
                                                        "ContextTypeName": null,
                                                        "ExpressionToEvaluate": null,
                                                        "FailureMessage": null,
                                                        "EvaluatorTypeName": null,
                                                        "ConditionType": null,
                                                        "AdditionalInfo": null,
                                                        "ConditionEventDetails": null
                                                      }
                                                    }
                                                  ]
                                                }
                                                """;


    public static readonly string JsonWithoutExpressionToEvaluate = """
                                                                    {
                                                                        "RuleName": "RuleWithoutExpressionToEvaluate",
                                                                        "TenantID": "All_Tenants",
                                                                        "CultureID": "en-GB",
                                                                        "IsDisabled": false,
                                                                        "FailureValue": {},
                                                                        "ValueTypeName": "None",
                                                                        "RuleEventDetails": null,
                                                                        "ConditionSets": [
                                                                        {
                                                                            "SetName": "SetOne",
                                                                            "SetValue": {},
                                                                            "BooleanConditions": {
                                                                            "Operator": null,
                                                                            "LeftOperand": null,
                                                                            "RightOperand": null,
                                                                            "ConditionName": "PersonName",
                                                                            "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Person",
                                                                            "FailureMessage": "Name should be John",
                                                                            "EvaluatorTypeName": "PredicateConditionEvaluator",
                                                                            "ConditionType": "LambdaPredicate",
                                                                            "AdditionalInfo": {},
                                                                            "ConditionEventDetails": null
                                                                            }
                                                                        }
                                                                        ]
                                                                    }
                                                                    """;


    public static readonly string JsonWithoutAContextTypeName = """
                                                                    {
                                                                        "RuleName": "RuleWithoutAContextTypeName",
                                                                        "TenantID": "All_Tenants",
                                                                        "CultureID": "en-GB",
                                                                        "IsDisabled": false,
                                                                        "FailureValue": {},
                                                                        "ValueTypeName": "None",
                                                                        "RuleEventDetails": null,
                                                                        "ConditionSets": [
                                                                        {
                                                                            "SetName": "SetOne",
                                                                            "SetValue": {},
                                                                            "BooleanConditions": {
                                                                            "Operator": null,
                                                                            "LeftOperand": null,
                                                                            "RightOperand": null,
                                                                            "ConditionName": "PersonName",
                                                                            "ExpressionToEvaluate": "p =\u003E (p.FirstName == \u0022John\u0022)",
                                                                            "FailureMessage": "Name should be John",
                                                                            "EvaluatorTypeName": "PredicateConditionEvaluator",
                                                                            "ConditionType": "LambdaPredicate",
                                                                            "AdditionalInfo": {},
                                                                            "ConditionEventDetails": null
                                                                            }
                                                                        }
                                                                        ]
                                                                    }
                                                                    """;

    public static readonly string JsonWithABadRuleEvent = """
                                                                    {
                                                                        "RuleName": "RuleWithABadRuleEvent",
                                                                        "TenantID": "All_Tenants",
                                                                        "CultureID": "en-GB",
                                                                        "IsDisabled": false,
                                                                        "FailureValue": 10,
                                                                        "ValueTypeName": "Int32",
                                                                          "RuleEventDetails": {
                                                                            "EventTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Events.BadRuleEventInt",
                                                                            "EventWhenType": "OnFailure"
                                                                          },
                                                                        "ConditionSets": [
                                                                        {
                                                                            "SetName": "SetOne",
                                                                            "SetValue": 20,
                                                                            "BooleanConditions": {
                                                                            "Operator": null,
                                                                            "LeftOperand": null,
                                                                            "RightOperand": null,
                                                                            "ConditionName": "PersonName",
                                                                            "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Person",
                                                                            "ExpressionToEvaluate": "p =\u003E (p.FirstName == \u0022John\u0022)",
                                                                            "FailureMessage": "Name should be John",
                                                                            "EvaluatorTypeName": "PredicateConditionEvaluator",
                                                                            "ConditionType": "LambdaPredicate",
                                                                            "AdditionalInfo": {},
                                                                            "ConditionEventDetails": null
                                                                            }
                                                                        }
                                                                        ]
                                                                    }
                                                                    """;

    public static readonly string JsonWithoutConditionName = """
                                                                    {
                                                                        "RuleName": "RuleWithoutAConditionName",
                                                                        "TenantID": "All_Tenants",
                                                                        "CultureID": "en-GB",
                                                                        "IsDisabled": false,
                                                                        "FailureValue": {},
                                                                        "ValueTypeName": "None",
                                                                        "RuleEventDetails": null,
                                                                        "ConditionSets": [
                                                                        {
                                                                            "SetName": "SetOne",
                                                                            "SetValue": {},
                                                                            "BooleanConditions": {
                                                                            "Operator": null,
                                                                            "LeftOperand": null,
                                                                            "RightOperand": null,
                                                                            "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Person",
                                                                            "ExpressionToEvaluate": "p =\u003E (p.FirstName == \u0022John\u0022)",
                                                                            "FailureMessage": "Name should be John",
                                                                            "EvaluatorTypeName": "PredicateConditionEvaluator",
                                                                            "ConditionType": "LambdaPredicate",
                                                                            "AdditionalInfo": {},
                                                                            "ConditionEventDetails": null
                                                                            }
                                                                        }
                                                                        ]
                                                                    }
                                                                    """;

    public static readonly string JsonWithoutAFailureMessage = """
                                                                    {
                                                                        "RuleName": "RuleWithoutAFailureMessage",
                                                                        "TenantID": "All_Tenants",
                                                                        "CultureID": "en-GB",
                                                                        "IsDisabled": false,
                                                                        "FailureValue": {},
                                                                        "ValueTypeName": "None",
                                                                        "RuleEventDetails": null,
                                                                        "ConditionSets": [
                                                                        {
                                                                            "SetName": "SetOne",
                                                                            "SetValue": {},
                                                                            "BooleanConditions": {
                                                                            "Operator": null,
                                                                            "LeftOperand": null,
                                                                            "RightOperand": null,
                                                                            "ConditionName": "PersonName",
                                                                            "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Person",
                                                                            "ExpressionToEvaluate": "p =\u003E (p.FirstName == \u0022John\u0022)",
                                                                            "EvaluatorTypeName": "PredicateConditionEvaluator",
                                                                            "ConditionType": "LambdaPredicate",
                                                                            "AdditionalInfo": {},
                                                                            "ConditionEventDetails": null
                                                                            }
                                                                        }
                                                                        ]
                                                                    }
                                                                    """;

    public static readonly string JsonWithoutAnEvaluatorTypeName = """
                                                                    {
                                                                        "RuleName": "RuleWithoutAnEvaluatorTypeName",
                                                                        "TenantID": "All_Tenants",
                                                                        "CultureID": "en-GB",
                                                                        "IsDisabled": false,
                                                                        "FailureValue": {},
                                                                        "ValueTypeName": "None",
                                                                        "RuleEventDetails": null,
                                                                        "ConditionSets": [
                                                                        {
                                                                            "SetName": "SetOne",
                                                                            "SetValue": {},
                                                                            "BooleanConditions": {
                                                                            "Operator": null,
                                                                            "LeftOperand": null,
                                                                            "RightOperand": null,
                                                                            "ConditionName": "PersonName",
                                                                            "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Person",
                                                                            "ExpressionToEvaluate": "p =\u003E (p.FirstName == \u0022John\u0022)",
                                                                            "ConditionType": "LambdaPredicate",
                                                                            "AdditionalInfo": {},
                                                                            "ConditionEventDetails": null
                                                                            }
                                                                        }
                                                                        ]
                                                                    }
                                                                    """;

    public static readonly string JsonWithABadConditionEvent = """
                                                                    {
                                                                        "RuleName": "RuleWithABadConditionEvent",
                                                                        "TenantID": "All_Tenants",
                                                                        "CultureID": "en-GB",
                                                                        "IsDisabled": false,
                                                                        "FailureValue": {},
                                                                        "ValueTypeName": "None",
                                                                        "RuleEventDetails": null,
                                                                        "ConditionSets": [
                                                                        {
                                                                            "SetName": "SetOne",
                                                                            "SetValue": {},
                                                                            "BooleanConditions": {
                                                                            "Operator": null,
                                                                            "LeftOperand": null,
                                                                            "RightOperand": null,
                                                                            "ConditionName": "PersonName",
                                                                            "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Person",
                                                                            "ExpressionToEvaluate": "p =\u003E (p.FirstName == \u0022John\u0022)",
                                                                            "EvaluatorTypeName": "PredicateConditionEvaluator",
                                                                            "ConditionType": "LambdaPredicate",
                                                                            "AdditionalInfo": {},
                                                                              "ConditionEventDetails": {
                                                                                "EventTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Events.BadConditionEvent",
                                                                                "EventWhenType": "OnSuccessOrFailure"
                                                                              }
                                                                            }
                                                                        }
                                                                        ]
                                                                    }
                                                                    """;


    public static readonly string JsonWithAShortPersonContextTypeName = """
                                                                    {
                                                                        "RuleName": "RuleWithAShortContextTypeName",
                                                                        "TenantID": "All_Tenants",
                                                                        "CultureID": "en-GB",
                                                                        "IsDisabled": false,
                                                                        "FailureValue": {},
                                                                        "ValueTypeName": "None",
                                                                        "RuleEventDetails": null,
                                                                        "ConditionSets": [
                                                                        {
                                                                            "SetName": "SetOne",
                                                                            "SetValue": {},
                                                                            "BooleanConditions": {
                                                                            "Operator": null,
                                                                            "LeftOperand": null,
                                                                            "RightOperand": null,
                                                                            "ConditionName": "PersonName",
                                                                            "ContextTypeName": "Person",
                                                                            "ExpressionToEvaluate": "p =\u003E (p.FirstName == \u0022John\u0022)",
                                                                            "EvaluatorTypeName": "PredicateConditionEvaluator",
                                                                            "ConditionType": "LambdaPredicate",
                                                                            "AdditionalInfo": {}
                                                                            }
                                                                        }
                                                                        ]
                                                                    }
                                                                    """;
    public static readonly string JsonWithABadContextTypeName = """
                                                                    {
                                                                        "RuleName": "RuleWithABadContextTypeName",
                                                                        "TenantID": "All_Tenants",
                                                                        "CultureID": "en-GB",
                                                                        "IsDisabled": false,
                                                                        "FailureValue": {},
                                                                        "ValueTypeName": "None",
                                                                        "RuleEventDetails": null,
                                                                        "ConditionSets": [
                                                                        {
                                                                            "SetName": "SetOne",
                                                                            "SetValue": {},
                                                                            "BooleanConditions": {
                                                                            "Operator": null,
                                                                            "LeftOperand": null,
                                                                            "RightOperand": null,
                                                                            "ConditionName": "PersonName",
                                                                             "ContextTypeName": "BadContextTypeName",
                                                                            "ExpressionToEvaluate": "p =\u003E (p.FirstName == \u0022John\u0022)",
                                                                            "FailureMessage": "Name should be John",
                                                                            "EvaluatorTypeName": "PredicateConditionEvaluator",
                                                                            "ConditionType": "LambdaPredicate",
                                                                            "AdditionalInfo": {},
                                                                            "ConditionEventDetails": null
                                                                            }
                                                                        }
                                                                        ]
                                                                    }
                                                                    """;

    public static readonly string JsonWithoutAValueTypeName = """
                                                                    {
                                                                        "RuleName": "RuleWithoutAValueTypeName",
                                                                        "TenantID": "All_Tenants",
                                                                        "CultureID": "en-GB",
                                                                        "IsDisabled": false,
                                                                        "FailureValue": {},
                                                                        "RuleEventDetails": null,
                                                                        "ConditionSets": [
                                                                        {
                                                                            "SetName": "SetOne",
                                                                            "SetValue": {},
                                                                            "BooleanConditions": {
                                                                            "Operator": null,
                                                                            "LeftOperand": null,
                                                                            "RightOperand": null,
                                                                            "ConditionName": "PersonName",
                                                                            "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Person",
                                                                            "ExpressionToEvaluate": "p =\u003E (p.FirstName == \u0022John\u0022)",
                                                                            "EvaluatorTypeName": "PredicateConditionEvaluator",
                                                                            "ConditionType": "LambdaPredicate",
                                                                            "AdditionalInfo": {},
                                                                            "ConditionEventDetails": null
                                                                            }
                                                                        }
                                                                        ]
                                                                    }
                                                                    """;


    public static readonly string JsonWithANullValueTypeName = """
                                                                    {
                                                                        "RuleName": "RuleWithoutAValueTypeName",
                                                                        "TenantID": "All_Tenants",
                                                                        "CultureID": "en-GB",
                                                                        "IsDisabled": false,
                                                                        "FailureValue": {},
                                                                        "ValueTypeName": null,
                                                                        "RuleEventDetails": null,
                                                                        "ConditionSets": [
                                                                        {
                                                                            "SetName": "SetOne",
                                                                            "SetValue": {},
                                                                            "BooleanConditions": {
                                                                            "Operator": null,
                                                                            "LeftOperand": null,
                                                                            "RightOperand": null,
                                                                            "ConditionName": "PersonName",
                                                                            "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Person",
                                                                            "ExpressionToEvaluate": "p =\u003E (p.FirstName == \u0022John\u0022)",
                                                                            "EvaluatorTypeName": "PredicateConditionEvaluator",
                                                                            "ConditionType": "LambdaPredicate",
                                                                            "AdditionalInfo": {},
                                                                            "ConditionEventDetails": null
                                                                            }
                                                                        }
                                                                        ]
                                                                    }
                                                                    """;


    public static readonly string JsonWithBadRuleEventWhenTypes = """
                                                                    {
                                                                        "RuleName": "RuleWithABadRuleEvent",
                                                                        "TenantID": "All_Tenants",
                                                                        "CultureID": "en-GB",
                                                                        "IsDisabled": false,
                                                                        "FailureValue": 10,
                                                                        "ValueTypeName": "Int32",
                                                                          "RuleEventDetails": {
                                                                            "EventTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Events.RuleEventInt",
                                                                            "EventWhenType": "IncorrectType"
                                                                          },
                                                                        "ConditionSets": [
                                                                        {
                                                                            "SetName": "SetOne",
                                                                            "SetValue": 20,
                                                                            "BooleanConditions": {
                                                                            "Operator": null,
                                                                            "LeftOperand": null,
                                                                            "RightOperand": null,
                                                                            "ConditionName": "PersonName",
                                                                            "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Customer",
                                                                            "ExpressionToEvaluate": "c =\u003E (c.CustomerName == \u0022CustomerOne\u0022)",
                                                                            "FailureMessage": "The customer name should be CustomerOne",
                                                                            "EvaluatorTypeName": "PredicateConditionEvaluator",
                                                                            "ConditionType": "LambdaPredicate",
                                                                            "AdditionalInfo": {},
                                                                              "ConditionEventDetails": {
                                                                                "EventTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Events.ConditionEventCustomer",
                                                                                "EventWhenType": "AnotherIncorrectType"
                                                                              }
                                                                            }
                                                                        }
                                                                        ]
                                                                    }
                                                                    """;




    public static readonly string JsonWithARuleNameOfNull = """
                                                                    {
                                                                        "RuleName": null,
                                                                        "TenantID": "All_Tenants",
                                                                        "CultureID": "en-GB",
                                                                        "IsDisabled": false,
                                                                        "FailureValue": {},
                                                                        "ValueTypeName": "None",
                                                                        "RuleEventDetails": null,
                                                                        "ConditionSets": [
                                                                        {
                                                                            "SetName": "SetOne",
                                                                            "SetValue": {},
                                                                            "BooleanConditions": {
                                                                            "Operator": null,
                                                                            "LeftOperand": null,
                                                                            "RightOperand": null,
                                                                            "ConditionName": "PersonName",
                                                                            "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Person",
                                                                            "ExpressionToEvaluate": "p =\u003E (p.FirstName == \u0022John\u0022)",
                                                                            "EvaluatorTypeName": "PredicateConditionEvaluator",
                                                                            "ConditionType": "LambdaPredicate",
                                                                            "AdditionalInfo": {},
                                                                            "ConditionEventDetails": null
                                                                            }
                                                                        }
                                                                        ]
                                                                    }
                                                                    """;
    public static readonly string JsonWithARuleNameProperty = """
                                                                    {
                                                                        "TenantID": "All_Tenants",
                                                                        "CultureID": "en-GB",
                                                                        "IsDisabled": false,
                                                                        "FailureValue": {},
                                                                        "ValueTypeName": "None",
                                                                        "RuleEventDetails": null,
                                                                        "ConditionSets": [
                                                                        {
                                                                            "SetName": "SetOne",
                                                                            "SetValue": {},
                                                                            "BooleanConditions": {
                                                                            "Operator": null,
                                                                            "LeftOperand": null,
                                                                            "RightOperand": null,
                                                                            "ConditionName": "PersonName",
                                                                            "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Person",
                                                                            "ExpressionToEvaluate": "p =\u003E (p.FirstName == \u0022John\u0022)",
                                                                            "EvaluatorTypeName": "PredicateConditionEvaluator",
                                                                            "ConditionType": "LambdaPredicate",
                                                                            "AdditionalInfo": {},
                                                                            "ConditionEventDetails": null
                                                                            }
                                                                        }
                                                                        ]
                                                                    }
                                                                    """;
public static readonly string JsonWithMissingRegexOptionsKey = """
                                                                {
                                                                  "RuleName": "RegexRule",
                                                                  "TenantID": "All_Tenants",
                                                                  "CultureID": "en-GB",
                                                                  "IsDisabled": false,
                                                                  "FailureValue": {},
                                                                  "ValueTypeName": "None",
                                                                  "RuleEventDetails": null,
                                                                  "ConditionSets": [
                                                                    {
                                                                      "SetName": "ConditionSetOne",
                                                                      "SetValue": {},
                                                                      "BooleanConditions": {
                                                                        "Operator": null,
                                                                        "LeftOperand": null,
                                                                        "RightOperand": null,
                                                                        "ConditionName": "ConditionOne",
                                                                        "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Customer",
                                                                        "ExpressionToEvaluate": "Address.AddressLine [IsMatch] ^[A-Z][a-z]{2,49}$",
                                                                        "FailureMessage": "The address line of: @{Address.AddressLine} did not match the required pattern",
                                                                        "EvaluatorTypeName": "RegexConditionEvaluator",
                                                                        "ConditionType": "CustomExpression",
                                                                        "AdditionalInfo": {"WrongKey":""},
                                                                        "ConditionEventDetails": null
                                                                      }
                                                                    }
                                                                  ]
                                                                }
                                                                """;

    public static readonly string JsonWithIncorrectRegexOptionValue = """
                                                                {
                                                                  "RuleName": "RegexRule",
                                                                  "TenantID": "All_Tenants",
                                                                  "CultureID": "en-GB",
                                                                  "IsDisabled": false,
                                                                  "FailureValue": {},
                                                                  "ValueTypeName": "None",
                                                                  "RuleEventDetails": null,
                                                                  "ConditionSets": [
                                                                    {
                                                                      "SetName": "ConditionSetOne",
                                                                      "SetValue": {},
                                                                      "BooleanConditions": {
                                                                        "Operator": null,
                                                                        "LeftOperand": null,
                                                                        "RightOperand": null,
                                                                        "ConditionName": "ConditionOne",
                                                                        "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Customer",
                                                                        "ExpressionToEvaluate": "Address.AddressLine [IsMatch] ^[A-Z][a-z]{2,49}$",
                                                                        "FailureMessage": "The address line of: @{Address.AddressLine} did not match the required pattern",
                                                                        "EvaluatorTypeName": "RegexConditionEvaluator",
                                                                        "ConditionType": "CustomExpression",
                                                                        "AdditionalInfo": {"RegexOptions":"IgnoreCaseWithExtraChars"},
                                                                        "ConditionEventDetails": null
                                                                      }
                                                                    }
                                                                  ]
                                                                }
                                                                """;

    public static readonly string JsonWithCorruptedExpressionString = """
                                                                {
                                                                  "RuleName": "RegexRule",
                                                                  "TenantID": "All_Tenants",
                                                                  "CultureID": "en-GB",
                                                                  "IsDisabled": false,
                                                                  "FailureValue": {},
                                                                  "ValueTypeName": "None",
                                                                  "RuleEventDetails": null,
                                                                  "ConditionSets": [
                                                                    {
                                                                      "SetName": "ConditionSetOne",
                                                                      "SetValue": {},
                                                                      "BooleanConditions": {
                                                                        "Operator": null,
                                                                        "LeftOperand": null,
                                                                        "RightOperand": null,
                                                                        "ConditionName": "ConditionOne",
                                                                        "ContextTypeName": "Conditionals.Core.Tests.SharedDataAndFixtures.Models.Customer",
                                                                        "ExpressionToEvaluate": "Address.AddressLine-wrong-format-^[A-Z][a-z]{2,49}$",
                                                                        "FailureMessage": "The address line of: @{Address.AddressLine} did not match the required pattern",
                                                                        "EvaluatorTypeName": "RegexConditionEvaluator",
                                                                        "ConditionType": "CustomExpression",
                                                                        "AdditionalInfo": {"RegexOptions":"IgnoreCaseWithExtraChars"},
                                                                        "ConditionEventDetails": null
                                                                      }
                                                                    }
                                                                  ]
                                                                }
                                                                """;
}
