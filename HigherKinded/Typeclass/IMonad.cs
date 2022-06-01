namespace HigherKinded.Typeclass;

public interface IMonad<TWitness> : IFunctor<TWitness>
{
    IKind<TWitness, TElement> Pure<TElement>(TElement element);

    IKind<TWitness, TResult> SelectMany<TSource, TResult>(IKind<TWitness, TSource> source,
        Func<TSource, IKind<TWitness, TResult>> selector);

    IKind<TWitness, TResult> IFunctor<TWitness>.Select<TSource, TResult>(
        IKind<TWitness, TSource> source,
        Func<TSource, TResult> selector
    ) => SelectMany(source, a => Pure(selector(a)));
}