using System.Diagnostics.CodeAnalysis;

namespace Ironclad.Collections;

/// <summary>
/// Represents an indexed collection that allows accessing elements by index.
/// </summary>
/// <typeparam name="TIndex">The type of index used for accessing elements.</typeparam>
/// <typeparam name="TValue">The type of elements stored in the collection.</typeparam>
public interface IIndexedCollection<TIndex, TValue> : IEnumerable<TValue>
{
    /// <summary>
    /// Gets the value associated with the specified index.
    /// </summary>
    /// <param name="index">The index to retrieve the value for.</param>
    /// <returns>The value associated with the specified index.</returns>
    TValue this[TIndex index] { get; }

    /// <summary>
    /// Gets a read-only collection of all values in the indexed collection.
    /// </summary>
    IReadOnlyCollection<TValue> Values { get; }

    /// <summary>
    /// Gets a read-only collection of all indices in the indexed collection.
    /// </summary>
    IReadOnlyCollection<TIndex> Indices { get; }

    /// <summary>
    /// Gets the number of elements in the indexed collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines whether the indexed collection contains a specific value.
    /// </summary>
    /// <param name="item">The value to check for in the collection.</param>
    /// <returns>true if the value is found; otherwise, false.</returns>
    bool Contains(TValue item);

    /// <summary>
    /// Determines whether the indexed collection contains an element with the specified index.
    /// </summary>
    /// <param name="index">The index to check for in the collection.</param>
    /// <returns>true if the index is found; otherwise, false.</returns>
    bool ContainsIndex(TIndex index);
    
    /// <summary>
    /// Tries to retrieve the value associated with the specified index.
    /// </summary>
    /// <param name="index">The index to retrieve the value for.</param>
    /// <param name="value">When this method returns, contains the value associated with the index if the index is found;
    /// otherwise, the default value for the value type.</param>
    /// <returns>true if the index is found and the value is successfully retrieved; otherwise, false.</returns>
    bool TryGet(TIndex index, [NotNullWhen(true)] out TValue? value);

    /// <summary>
    /// Removes the element with the specified index from the collection.
    /// </summary>
    /// <param name="index">The index of the element to remove.</param>
    /// <returns>true if the element was removed; otherwise, false.</returns>
    bool Remove(TIndex index);

    /// <summary>
    /// Removes all elements from the collection.
    /// </summary>
    void Clear();
}
