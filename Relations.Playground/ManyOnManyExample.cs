using System.Collections;

namespace Playground;

public class Person
{
	public ICollection<Person> Friends { get; }
}

public class UndirectedMultiRelationshipManager<T> where T : class
{
	private readonly Dictionary<T, HashSet<T>> connections = new();
	
	public ICollection<T> Get(T self) => connections.TryGetValue(self, out var value) ? value : Array.Empty<T>();
	
#pragma warning disable CS0693 // type parameter has same name
	internal class CollectionHandle<T> : ICollection<T> where T : class
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

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(T item)
		{
			if (!manager.connections.ContainsKey(self)) 
				manager.connections.Add(self, new HashSet<T>());
			manager.connections[self].Add(item);
			if (!manager.connections.ContainsKey(item)) 
				manager.connections.Add(item, new HashSet<T>());
			manager.connections[self].Add(self);
		}

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