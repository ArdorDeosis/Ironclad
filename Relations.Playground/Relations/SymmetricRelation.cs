using System.Reflection;

namespace Ironclad.Relations.Playground.Relations;

internal sealed class SymmetricRelation<T>
{
	private readonly PropertyInfo propertyInfo;
		
	public SymmetricRelation(PropertyInfo propertyInfo)
	{
		this.propertyInfo = propertyInfo;
	}
		
	public T Get(T instance) => (T)propertyInfo.GetValue(instance);
	public void Set(T instance, T? value) => propertyInfo.SetValue(instance, value);
}