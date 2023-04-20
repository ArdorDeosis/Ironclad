using System.Collections;
using System.Numerics;
using System.Text;

namespace Ironclad.Collections;

/// <summary>
/// A value type list of items.
/// </summary>
/// <typeparam name="T">The type of the items.</typeparam>
public readonly struct ValueList<T> : 
  IReadOnlyList<T>, 
  IEquatable<ValueList<T>>, 
  IEqualityOperators<ValueList<T>, ValueList<T>, bool>
{
  private readonly T[] items = Array.Empty<T>();

  /// <summary>
  /// An empty instance of <see cref="ValueList{T}"/>.
  /// </summary>
  public static ValueList<T> Empty { get; } = new();

  /// <summary>
  /// Creates a new instance of <see cref="ValueList{T}"/> containing the specified items.
  /// </summary>
  /// <remarks>Copies the items to an array and thus enumerates the enumerable.</remarks>
  public ValueList(IEnumerable<T> items)
  {
    this.items = items.ToArray();
  }

  /// <summary>
  /// Creates a new instance of <see cref="ValueList{T}"/> containing the specified items.
  /// </summary>
  public ValueList(params T[] items) : this(items.AsEnumerable()) { }

  /// <inheritdoc />
  public int Count => items.Length;
  
  /// <inheritdoc />
  public T this[int index] => items[index];

  /// <inheritdoc />
  public bool Equals(ValueList<T> other)
  {
    if (items.Length != other.items.Length)
      return false;

    for (var i = 0; i < items.Length; i++)
    {
      if (!EqualityComparer<T>.Default.Equals(items[i], other.items[i]))
        return false;
    }

    return true;
  }

  /// <inheritdoc />
  public override bool Equals(object? obj) => 
    obj is ValueList<T> other && Equals(other);
  
  public static bool operator ==(ValueList<T> left, ValueList<T> right) => left.Equals(right);

  public static bool operator !=(ValueList<T> left, ValueList<T> right) => !left.Equals(right);

  /// <inheritdoc />
  public override int GetHashCode()
  {
    var hash = new HashCode();
    foreach (var item in items) 
      hash.Add(item);
    return hash.ToHashCode();
  }

  /// <inheritdoc />
  public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)items).GetEnumerator();
  
  /// <inheritdoc />
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  /// <inheritdoc />
  public override string ToString()
  {
    var builder = new StringBuilder();
    builder.Append('[');
    for (var i = 0; i < items.Length; i++)
    {
      builder.Append(items[i]);
      if (i < items.Length - 1) 
        builder.Append(", ");
    }
    builder.Append(']');
    return builder.ToString();
  }
}