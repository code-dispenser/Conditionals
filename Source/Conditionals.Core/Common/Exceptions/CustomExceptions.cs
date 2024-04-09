using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;

namespace Conditionals.Core.Common.Exceptions;


/// <summary>
/// Represents an exception that is thrown when there no data <see cref="DataContext"/> for a condition. 
/// Initialises a new instance of the <see cref="MissingEvaluatorResolverException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public class MissingConditionDataException(string message) : Exception(message) { }


/// <summary>
/// Represents an exception that is thrown when there is no implementation of a <see cref="ConditionEvaluatorResolver"/>. 
/// Initialises a new instance of the <see cref="MissingEvaluatorResolverException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public class MissingEvaluatorResolverException(string message) : Exception(message) { }

/// <summary>
/// Represents an exception that is thrown when there is no implementation of a <see cref="IConditionEvaluator{TContext}"/>. 
/// Initialises a new instance of the <see cref="MissingEvaluatorException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public class MissingEvaluatorException(string message) : Exception(message) { }

/// <summary>
/// Represents an exception that is thrown when there is no <see cref="ConditionData"/>. 
/// Initialises a new instance of the <see cref="MissingAllConditionDataException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public class MissingAllConditionDataException(string message) : Exception(message) { }

/// <summary>
/// Represents an exception that is thrown when trying to deserialize an object in a Json formatted string, but the string is null. 
/// Initialises a new instance of the <see cref="DeserializationToNullException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public class DeserializationToNullException(string message) : Exception(message) { }

/// <summary>
/// Represents an exception that may be thrown whilst creating and raising an event associated with either a rule or condition. 
/// Initialises a new instance of the <see cref="RaiseEventException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
public class RaiseEventException(string message, Exception? innerException = null) : Exception(message,innerException) { }

/// <summary>
/// Represents an exception that may be thrown during the disposal of an object that implements the <see cref="IDisposable"/> interface after it has been removed from internal cache. 
/// Initialises a new instance of the <see cref="DisposingRemovedItemException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
public class DisposingRemovedItemException(string message, Exception? innerException = null) : Exception(message, innerException) {}

/// <summary>
/// Represents an exception that is thrown when a condition is not an implementation of either <see cref="BooleanConditionBase"/> or <see cref="ICondition"/>.
/// Initialises a new instance of the <see cref="InvalidBooleanConditionTypeException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
public class InvalidBooleanConditionTypeException(string message, Exception? innerException = null) : Exception(message, innerException) { }

/// <summary>
/// Represents an exception that is thrown when an rule is not found in the condition engines cache of rules.
/// Initialises a new instance of the <see cref="RuleNotFoundException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
public class RuleNotFoundException(string message, Exception? innerException = null) : Exception(message, innerException) {}

/// <summary>
/// Represents an exception that is thrown when a <see cref="PredicateConditionEvaluator{TContext}"/> encounters a condition that does not have a 
/// <see cref="Conditionals.Core.Common.Seeds.ConditionType" /> value of LambdaPredicate.
/// Initialises a new instance of the <see cref="PredicateConditionCompilationException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
public class PredicateConditionCompilationException(string message, Exception? innerException = null) : Exception(message, innerException) {}

/// <summary>
/// Represents an exception that is thrown when an implementation of <see cref="IConditionEvaluator{TContext}"/> is missing.
/// Initialises a new instance of the <see cref="MissingConditionEvaluatorException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
public class MissingConditionEvaluatorException(string message, Exception? innerException = null) : Exception(message, innerException) {}

/// <summary>
/// Represents an exception that is thrown when the data type expected by <see cref="Rule{T}" /> is not found in the list of <see cref="AppDomain" /> assemblies.
/// Initialises a new instance of the <see cref="InvalidSystemDataTypeException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public class InvalidSystemDataTypeException(string message) : Exception(message) { }

/// <summary>
/// Represents an exception that is thrown when an unexpected exception occurs whilst converting a Json rule to a <see cref="Rule{T}" />.
/// Initialises a new instance of the <see cref="RuleFromJsonException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
public class RuleFromJsonException(string message, Exception? innerException = null) : Exception(message, innerException) {}


/// <summary>
/// Represents an exception that is thrown when the ExpressionToEvaluate property of condition is missing.
/// Initialises a new instance of the <see cref="MissingExpressionToEvaluateException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
public class MissingExpressionToEvaluateException(string message, Exception? innerException = null) : Exception(message, innerException) {}

/// <summary>
/// Represents an exception that is thrown when the ContextType of a condition is not found in the list of <see cref="AppDomain" /> assemblies when trying to create
/// a condition from a Json rule.
/// Initialises a new instance of the <see cref="ContextTypeAssemblyNotFoundException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
public class ContextTypeAssemblyNotFoundException(string message, Exception? innerException = null) : Exception(message, innerException) {}

/// <summary>
/// Represents an exception that is thrown when an event is not found.
/// Initialises a new instance of the <see cref="EventNotFoundException"/> class with a specified error message.
/// </summary>
/// <param name="message">The message that describes the error.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
public class EventNotFoundException(string message, Exception? innerException = null) : Exception(message, innerException) {}