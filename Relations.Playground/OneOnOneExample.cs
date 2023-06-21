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

public sealed class UndirectedRelationshipManager<T> where T : class
{
	private readonly Dictionary<T, T> dictionary = new();
	
	public T? Get(T self) => dictionary.TryGetValue(self, out var value) ? value : null;
	
	public void Set(T self, T? value)
	{
		RemoveConnection(self);
		if (value is null)
			return;
		RemoveConnection(value);
		dictionary.Add(self, value);
		dictionary.Add(value, self);
	}

	private void RemoveConnection(T self)
	{
		if (!dictionary.TryGetValue(self, out var other)) 
			return;
		if (!dictionary.TryGetValue(other, out var shouldBeSelf))
			throw new InvalidStateException();
		if (shouldBeSelf != self)
			throw new InvalidStateException();
		dictionary.Remove(other);
		dictionary.Remove(self);
	}
}