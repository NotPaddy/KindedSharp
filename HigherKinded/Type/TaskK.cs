using HigherKinded.Typeclass;

namespace HigherKinded.Type;

public record TaskK<TElement>(Task<TElement> Task) : IKind<TaskK.W, TElement>
{
    public static implicit operator Task<TElement>(TaskK<TElement> task) => task.Task;
    public static implicit operator TaskK<TElement>(Task<TElement> task) => new(task);
}

public static class TaskK
{
    public readonly struct W
    {
    }

    public static TaskK<TElement> K<TElement>(this Task<TElement> target) => target;
    public static TaskK<TElement> Fix<TElement>(this IKind<W, TElement> target) => (TaskK<TElement>) target;

    public static readonly IMonad<W> Monad = new TaskKMonad();

    private class TaskKMonad : IMonad<W>
    {
        public IKind<W, TElement> Pure<TElement>(TElement element) =>
            new TaskK<TElement>(Task.FromResult(element));

        public IKind<W, TResult> SelectMany<TSource, TResult>(IKind<W, TSource> source,
            Func<TSource, IKind<W, TResult>> selector) =>
            new TaskK<TResult>(SelectTask(source, selector));

        private static async Task<TResult> SelectTask<TSource, TResult>(IKind<W, TSource> source,
            Func<TSource, IKind<W, TResult>> selector)
        {
            var from = await source.Fix().Task;
            return await selector(from).Fix().Task;
        }
    }

    #region LINQ Extensions

    public static IKind<W, TResult> Select<TSource, TResult>(
        this IKind<W, TSource> source,
        Func<TSource, TResult> selector
    ) => Monad.Select(source, selector);

    public static IKind<W, TResult> SelectMany<TSource, TResult>(
        this IKind<W, TSource> source,
        Func<TSource, IKind<W, TResult>> selector
    ) => Monad.SelectMany(source, selector);

    public static IKind<W, TResult> SelectMany<TSource, TCollection, TResult>(
        this IKind<W, TSource> source,
        Func<TSource, IKind<W, TCollection>> collectionSelector,
        Func<TSource, TCollection, TResult> resultSelector
    ) => source.SelectMany(a => collectionSelector(a).Select(b => resultSelector(a, b)));

    #endregion
}