using System.Reflection;

namespace Ironclad.Relations.Playground.Relations;


internal sealed class AsymmetricRelation<TA, TB>
{
	private readonly PropertyInfo sourceProperty;
	private readonly PropertyInfo targetProperty;
		
	public AsymmetricRelation((PropertyInfo source, PropertyInfo target) properties)
	{
		sourceProperty = properties.source;
		targetProperty = properties.target;
	}
		
	public AsymmetricRelation(PropertyInfo sourceProperty, PropertyInfo targetProperty)
	{
		this.sourceProperty = sourceProperty;
		this.targetProperty = targetProperty;
	}

	public TB GetForward(TA instance) => (TB)sourceProperty.GetValue(instance);
	public void SetForward(TA instance, TB? value) => sourceProperty.SetValue(instance, value);
	public TA GetBackward(TB instance) => (TA)targetProperty.GetValue(instance);
	public void SetBackward(TB instance, TA? value) => targetProperty.SetValue(instance, value);
}