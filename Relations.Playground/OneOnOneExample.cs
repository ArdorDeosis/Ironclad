namespace Ironclad.Relations.Playground;

public class MonogamousPerson
{
	// instance needs to be injected
	private readonly UndirectedRelationshipManager<MonogamousPerson> relationshipManager;

	public MonogamousPerson? Partner
	{
		get => relationshipManager.Get(this);
		set => relationshipManager.Set(this, value);
	}
}

public class UndirectedRelationshipManager<T> where T : class
{
	private readonly Dictionary<T, T> dictionary = new();
	
	public T? Get(T self) => dictionary.TryGetValue(self, out var value) ? value : null;
	
	public void Set(T self, T? value)
	{
		if (dictionary.ContainsKey(self))
			throw new InvalidOperationException();
		if (value is null)
		{
			dictionary.Remove(dictionary[self]);
			dictionary.Remove(self);
			return;
		}
		if (dictionary.ContainsKey(value))
			throw new InvalidOperationException();
		dictionary.Add(self, value);
		dictionary.Add(value, self);
	}
}