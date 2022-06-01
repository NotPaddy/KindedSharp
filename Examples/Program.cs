using HigherKinded.Type;

var result =
    from a in Task.FromResult(123).K()
    from err in Task.FromCanceled<double>(new CancellationToken()).K()
    from b in Task.FromResult($"abc{a * 2}").K()
    select $"{a} and {b}";
    
Console.Write(await result.Fix().Task);