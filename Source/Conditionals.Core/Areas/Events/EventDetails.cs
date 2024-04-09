using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Common.Utilities;

namespace Conditionals.Core.Areas.Events;

/// <summary>
/// Holds information about an event such as its fully qualified type name and an enumerated value that indicates when the event should be raised.
/// </summary>
public class EventDetails
{
    /// <summary>
    /// Gets the fully qualified event type name.
    /// </summary>
    public string EventTypeName { get; }

    /// <summary>
    /// Gets the enumerated value that is used to define when the event is raised <see cref="Conditionals.Core.Common.Seeds.EventWhenType"/>.
    /// </summary>
    public EventWhenType EventWhenType  { get; }

    internal EventDetails(string eventTypeName, EventWhenType eventWhenType = EventWhenType.OnSuccessOrFailure)

        => (EventTypeName, EventWhenType)  = (eventTypeName, eventWhenType);

    /// <summary>
    /// Factory method used to create an instance of the <see cref="EventDetails" /> class.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to be raised. Events will be implementations of either the <see cref="RuleEventBase{T}" /> or <see cref="ConditionEventBase{T}" /> abstract classes.</typeparam>
    /// <param name="eventWhenType">The enumerated value that is used to define when the event is raised see the <see cref="Conditionals.Core.Common.Seeds.EventWhenType"/> enumeration.</param>
    /// <returns>An new instance of the <see cref="EventDetails" /> class.</returns>
    public static EventDetails Create<TEvent>(EventWhenType eventWhenType = EventWhenType.OnSuccessOrFailure) where TEvent : IEvent
    
        => new (typeof(TEvent).AssemblyQualifiedName!, eventWhenType);


    internal EventDetails DeepCloneEvent()
    
        => new (EventTypeName, EventWhenType);

    //internal static EventDetails? FromJsonRule(JsonEventDetails? eventDetails)
    //{
    //    if (eventDetails == null || eventDetails.EventTypeName == null) return null;

    //    string searchName = eventDetails.EventTypeName.Contains('.') == true ? eventDetails.EventTypeName : String.Concat(".", eventDetails.EventTypeName);

    //    string? assemblyQualifiedName = (GeneralUtils.EventTypeNames.Where(e => e.fullName.EndsWith(searchName)).SingleOrDefault()).assemblyQualifiedName;

    //    EventWhenType eventWhenType = Enum.TryParse<EventWhenType>(eventDetails.EventWhenType, out var eventWhenValue) == true ? eventWhenValue : EventWhenType.Never;

    //    return String.IsNullOrWhiteSpace(assemblyQualifiedName) == false ? new EventDetails(assemblyQualifiedName, eventWhenType) : null;
    //}

}