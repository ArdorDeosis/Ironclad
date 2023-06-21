using System.Reflection;

namespace Ironclad.Relations.Playground.Comparer;

internal class PropertyInfoComparer : IComparer<PropertyInfo>
{
	public static PropertyInfoComparer Default { get; } = new();
	
	public int Compare(PropertyInfo x, PropertyInfo y)
	{
		var comparer = PropertyInfoEqualityComparer.Default;
		if (comparer.Equals(x, y)) return 0;
		
		var comparison = comparer.GetHashCode(x) - comparer.GetHashCode(y);
		if (comparison != 0) return comparison;
		
		comparison = x.DeclaringType!.GetHashCode() - y.DeclaringType!.GetHashCode();
		if (comparison != 0) return comparison;
		
		comparison = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
		if (comparison != 0) return 0;

		comparison = string.Compare(
			x.DeclaringType!.AssemblyQualifiedName, 
			y.DeclaringType!.AssemblyQualifiedName, 
			StringComparison.Ordinal);
		if (comparison != 0) return comparison;
		
		throw new Exception("x and y are not equal, but all compared properties yield equality");
	}
}