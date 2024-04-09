namespace Conditionals.Core.Tests.SharedDataAndFixtures.Events;

public class BadEventHandler
{
    public Task Handle(RuleEventInt theEvent, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
