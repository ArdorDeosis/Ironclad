using System.Collections;

namespace Ironclad.Collections;

// TODO: add equality comparers
public sealed class SetDictionary<TKey, TValue> where TKey : notnull
{
	private uint version = 0;

	private readonly Dictionary<TKey, HashSet<TValue>> dictionary = new();

	public IReadOnlyCollection<TValue> this[TKey key] =>
		dictionary.TryGetValue(key, out var set)
			? new ValueSet(set)
			: ValueSet.Empty;

	public bool Add(TKey key, TValue value)
	{
		if (!dictionary.ContainsKey(key))
			dictionary.Add(key, new HashSet<TValue>());
		var result = dictionary[key].Add(value);
		unchecked
		{
			version++;
		}

		return result;
	}

	public bool Remove(TKey key)
	{
		if (dictionary.TryGetValue(key, out var set))
			set.Clear();
		if (dictionary.Remove(key))
		{
			unchecked
			{
				version++;
			}

			return true;
		}

		return false;
	}

	public bool RemoveFrom(TKey key, TValue value)
	{
		if (dictionary.TryGetValue(key, out var set) && set.Remove(value))
		{
			if (set.Count is 0)
				dictionary.Remove(key);
			unchecked
			{
				version++;
			}

			return true;
		}

		return false;
	}

	public void Clear()
	{
		dictionary.Clear();
		unchecked
		{
			version++;
		}
	}

	public readonly struct ValueSet : IReadOnlyCollection<TValue>
	{
		private readonly TValue[] data;

		public static ValueSet Empty { get; } = new(Array.Empty<TValue>());

		internal ValueSet(IEnumerable<TValue> data)
		{
			this.data = data.ToArray();
			Count = this.data.Length;
		}

		public IEnumerator<TValue> GetEnumerator() => data.Cast<TValue>().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => data.GetEnumerator();

		public int Count { get; }
	}
}

public readonly struct KeySetPair<TKey, TValue> { }