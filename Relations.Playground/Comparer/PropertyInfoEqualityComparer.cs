using System.Reflection;

namespace Ironclad.Relations.Playground.Comparer;

internal class PropertyInfoEqualityComparer : IEqualityComparer<PropertyInfo>
{
	public static PropertyInfoEqualityComparer Default { get; } = new();
	
	public bool Equals(PropertyInfo x, PropertyInfo y) =>
		ReferenceEquals(x, y) || 
		x.DeclaringType == y.DeclaringType && 
		x.Name == y.Name;

	public int GetHashCode(PropertyInfo propertyInfo) => 
		propertyInfo.DeclaringType?.GetHashCode() ?? 0 ^ propertyInfo.Name.GetHashCode();
}