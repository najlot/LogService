using Najlot.Map;
using System.Collections.Generic;
using System.Linq;

namespace LogService.Service.Test.Helpers;

/// <summary>
/// Helper classes for mocking Najlot.Map interfaces in unit tests
/// </summary>
public class MockMapExpression<T> : IMapExpression<T>
{
    private readonly T _value;

    public MockMapExpression(T value)
    {
        _value = value;
    }

    public T To<TDest>() => (T)(object)_value;
}

public class MockArrayMapExpression<T> : IArrayMapExpression<T>
{
    private readonly T[] _value;

    public MockArrayMapExpression(T[] value)
    {
        _value = value;
    }

    public T[] ToArray<TDest>() => _value;
}

public class MockListMapExpression<T> : IListMapExpression<T>
{
    private readonly List<T> _value;

    public MockListMapExpression(List<T> value)
    {
        _value = value;
    }

    public List<T> ToList<TDest>() => _value;
}

/// <summary>
/// Extension methods to support async enumerable testing
/// </summary>
public static class TestExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            yield return item;
        }
    }
}