using System.Collections;

namespace Ironclad.Collections;

// TODO: analyzers to warn about using mutable indices
public sealed class AutoIndexedCollection<TIndex, TValue> : IIndexedCollection<TIndex, TValue> where TIndex : notnull
{
	private readonly Dictionary<TIndex, TValue> dictionary;
	private readonly Func<TValue, TIndex> getIndex;

	public AutoIndexedCollection(Func<TValue, TIndex> getIndex, IEqualityComparer<TIndex>? indexEqualityComparer = null)
	{
		this.getIndex = getIndex;
		dictionary = new Dictionary<TIndex, TValue>(indexEqualityComparer);
	}

	public IEnumerator<TValue> GetEnumerator() => dictionary.Values.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public TValue this[TIndex index] => dictionary[index];

	public IReadOnlyCollection<TValue> Values => dictionary.Values;
	
	public IReadOnlyCollection<TIndex> Indices => dictionary.Keys;

	public int Count => dictionary.Count;

	public bool Contains(TValue item) => dictionary.ContainsValue(item);

	public bool ContainsIndex(TIndex index) => dictionary.ContainsKey(index);

	public void Add(TValue item) => dictionary.Add(getIndex(item), item);

	public bool TryAdd(TValue item) => dictionary.TryAdd(getIndex(item), item);

	public bool Remove(TIndex index) => dictionary.Remove(index);

	public void Clear() => dictionary.Clear();
}