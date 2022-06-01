namespace HigherKinded.Typeclass;

public interface IFunctor<TWitness>
{
    IKind<TWitness, TB> Select<TA, TB>(IKind<TWitness, TA> source, Func<TA, TB> selector);
}