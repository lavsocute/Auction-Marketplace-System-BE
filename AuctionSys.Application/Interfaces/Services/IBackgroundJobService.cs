namespace AuctionSys.Application.Interfaces.Services;

public interface IBackgroundJobService
{
    string Schedule<T>(System.Linq.Expressions.Expression<Action<T>> methodCall, TimeSpan delay);
    string Schedule<T>(System.Linq.Expressions.Expression<Func<T, Task>> methodCall, TimeSpan delay);
}
