using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Ironclad.Collections;

// TODO: analyzers to warn about using mutable indices
public sealed class AutoIndexedCollection<TIndex, TValue> : IIndexedCollection<TIndex, TValue> //, IParameterlessAddOperators<TValue> where TIndex : notnull
{
	private readonly Dictionary<TIndex, TValue> dictionary;
	private readonly Func<TValue, TIndex> getIndex;

	public AutoIndexedCollection(Func<TValue, TIndex> getIndex, IEqualityComparer<TIndex>? indexEqualityComparer = null)
	{
		this.getIndex = getIndex;
		dictionary = new Dictionary<TIndex, TValue>(indexEqualityComparer);
	}

	/// <inheritdoc />
	public IEnumerator<TValue> GetEnumerator() => dictionary.Values.GetEnumerator();

	/// <inheritdoc />
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <inheritdoc />
	public TValue this[TIndex index] => dictionary[index];

	/// <inheritdoc />
	public IReadOnlyCollection<TValue> Values => dictionary.Values;
	
	/// <inheritdoc />
	public IReadOnlyCollection<TIndex> Indices => dictionary.Keys;

	/// <inheritdoc />
	public int Count => dictionary.Count;

	/// <inheritdoc />
	public bool Contains(TValue item) => dictionary.ContainsValue(item);

	/// <inheritdoc />
	public bool ContainsIndex(TIndex index) => dictionary.ContainsKey(index);
	
	/// <inheritdoc />
	public bool TryGet(TIndex index, [NotNullWhen(true)] out TValue? value) => dictionary.TryGetValue(index, out value);

	public void Add(TValue item) => dictionary.Add(getIndex(item), item);

	public bool TryAdd(TValue item) => dictionary.TryAdd(getIndex(item), item);

	/// <inheritdoc />
	public bool Remove(TIndex index) => dictionary.Remove(index);

	/// <inheritdoc />
	public void Clear() => dictionary.Clear();
}