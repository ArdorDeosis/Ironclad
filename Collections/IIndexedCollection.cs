namespace Ironclad.Collections;

public interface IIndexedCollection<TIndex, TValue> : IEnumerable<TValue>
{
	TValue this[TIndex index] { get; }

	IReadOnlyCollection<TValue> Values { get; }
	
	IReadOnlyCollection<TIndex> Indices { get; }
	
	public int Count { get; }

	public bool Contains(TValue item);
	
	public bool ContainsIndex(TIndex index);
	
	public void Add(TValue item);
	
	public bool TryAdd(TValue item);

	public bool Remove(TIndex index);
	
	public void Clear();
}