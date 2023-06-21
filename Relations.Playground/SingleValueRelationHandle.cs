namespace Ironclad.Relations.Playground;

public sealed class SingleValueRelationHandle<TA, TB>
{
	private readonly Func<TA, TB?> get;
	private readonly Action<TA, TB?> set;

	public SingleValueRelationHandle(Func<TA, TB> get, Action<TA, TB?> set)
	{
		this.get = get;
		this.set = set;
	}

	public TB? Get(TA instance) => get(instance);
	public void Set(TA instance, TB? value) => set(instance, value);
}