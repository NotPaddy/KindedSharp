using HigherKinded.Typeclass;

namespace HigherKinded.Type;

public record ListK<TA>(List<TA> List) : IKind<ListK.W, TA>
{
    public static implicit operator List<TA>(ListK<TA> task) => task.List;
    public static implicit operator ListK<TA>(List<TA> task) => new(task);
}

public static class ListK
{
    public readonly struct W
    {
    }
    
    public static ListK<TElement> K<TElement>(this List<TElement> target) => target;
    public static ListK<TA> Fix<TA>(this IKind<W, TA> target) => (ListK<TA>) target;

    public static readonly IMonad<W> Monad = new ListKMonad();

    private class ListKMonad : IMonad<W>
    {
        public IKind<W, TElement> Pure<TElement>(TElement element) =>
            new ListK<TElement>(new List<TElement> {element});

        public IKind<W, TResult> SelectMany<TSource, TResult>(IKind<W, TSource> source,
            Func<TSource, IKind<W, TResult>> selector) =>
            new ListK<TResult>(
                source
                    .Fix().List
                    .SelectMany(a => selector(a)
                        .Fix().List
                    ).ToList()
            );
    }
}