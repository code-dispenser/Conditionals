using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Common.Models;

namespace Conditionals.Core.Tests.SharedDataAndFixtures.Evaluators;

public class ExceptionInDisposeEvaluator<TContext> : IConditionEvaluator<TContext>, IDisposable
{
    private bool _disposedValue;
    private bool _supressException;

    public ExceptionInDisposeEvaluator(bool supressException = false)
    
        => _supressException = supressException;

    public async Task<EvaluationResult> Evaluate(Condition<TContext> condition, TContext data, CancellationToken cancellationToken, string tenantID = "All_Tenants")
    {

        return await Task.FromResult(new EvaluationResult(true));
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing && _supressException == false) throw new Exception("Bad stuff happened");

            _disposedValue = true;
        }
    }


    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}