using System.Collections;

namespace Playground;

public class Person
{
	public ICollection<Person> Friends { get; }
}

public class UndirectedMultiRelationshipManager<T> where T : class
{
	private readonly Dictionary<T, HashSet<T>> connections = new();

	private HashSet<T> this[T item] => connections.ContainsKey(item) ? connections[item] : new HashSet<T>();

	public ICollection<T> Get(T self) => new CollectionHandle<T>(this, self);
	
#pragma warning disable CS0693 // type parameter has same name
	public class CollectionHandle<T> : ICollection<T> where T : class
#pragma warning restore CS0693
	{
		private readonly UndirectedMultiRelationshipManager<T> manager;
		private readonly T self;

		public CollectionHandle(UndirectedMultiRelationshipManager<T> manager, T self)
		{
			this.manager = manager;
			this.self = self;
		}

		public IEnumerator<T> GetEnumerator() => manager.connections.ContainsKey(self) 
			? manager.connections[self].GetEnumerator()
			: new HashSet<T>.Enumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Add(T item)
		{
			if (manager.connections.TryGetValue(self, out var connections) && connections.Contains(item))
				throw new InvalidOperationException("self and item are already connected");
			if (!manager.connections.ContainsKey(self)) 
				manager.connections.Add(self, new HashSet<T>());
			if (!manager.connections.ContainsKey(item)) 
				manager.connections.Add(item, new HashSet<T>());
			manager.connections[self].Add(item);
			manager.connections[item].Add(self);
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(T item) => manager.connections.ContainsKey(item);

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (manager.connections.TryGetValue(self, out var connections))
				connections.CopyTo(array, arrayIndex);
			else
				Array.Empty<T>().CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		public int Count { get; }
		public bool IsReadOnly { get; }
	}
}