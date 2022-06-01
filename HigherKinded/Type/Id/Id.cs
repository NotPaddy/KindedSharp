using HigherKinded.Typeclass;

namespace HigherKinded.Type.Id;

public readonly struct Id<TElement> : IKind<Id.W, TElement>
{
    public readonly TElement Value;

    public Id(TElement value) => Value = value;

    public static implicit operator TElement(Id<TElement> id) => id.Value;
    public static implicit operator Id<TElement>(TElement value) => new(value);
}

public static class Id
{
    public readonly struct W
    {
    }

    public static Id<TElement> K<TElement>(this TElement target) => target;
    public static Id<TElement> Fix<TElement>(this IKind<W, TElement> target) => (Id<TElement>) target;

    public static readonly IMonad<W> Monad = new IdMonad();

    private class IdMonad : IMonad<W>
    {
        public IKind<W, TElement> Pure<TElement>(TElement element) =>
            new Id<TElement>(element);

        public IKind<W, TResult> SelectMany<TSource, TResult>(
            IKind<W, TSource> source,
            Func<TSource, IKind<W, TResult>> selector
        ) => selector(source.Fix());
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