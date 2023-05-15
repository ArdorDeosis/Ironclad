using System.Collections;
using System.Collections.Immutable;

namespace Ironclad.Collections;

// TODO: add equality comparers
public sealed class MultiValueDictionary<TKey, TValue> where TKey : notnull
{
	private readonly Dictionary<TKey, HashSet<TValue>> dictionary = new();

	public IReadOnlyCollection<TValue> this[TKey key] => dictionary.TryGetValue(key, out var set)
		? set
		: Array.Empty<TValue>();

	public bool Add(TKey key, TValue value)
	{
		if (!dictionary.ContainsKey(key))
			dictionary.Add(key, new HashSet<TValue>());
		return dictionary[key].Add(value);
	}

	public void Clear() => dictionary.Clear();

	public sealed class ValueSet<TKey, TValue> : IReadOnlyCollection<TValue> where TKey : notnull
	{
		private readonly MultiValueDictionary<TKey, TValue> parent;
		private readonly TKey key;

		internal ValueSet(MultiValueDictionary<TKey, TValue> parent, TKey key)
		{
			this.parent = parent;
			this.key = key;
		}

		public IEnumerator<TValue> GetEnumerator() => parent.dictionary.ContainsKey(key)
			? parent[key].GetEnumerator()
			: new EmptyEnumerator<TValue>();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public bool Add(TValue item) => set.Add(item);

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(T item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		public int Count { get; }
		public bool IsReadOnly { get; }
	}
}