using System.Reflection;

namespace Ironclad.Relations.Playground.Comparer;

internal class PropertyInfoTupleEqualityComparer : IEqualityComparer<(PropertyInfo, PropertyInfo)>
{
	public static PropertyInfoTupleEqualityComparer Default { get; } = new();

	public bool Equals((PropertyInfo, PropertyInfo) x, (PropertyInfo, PropertyInfo) y) =>
		PropertyInfoEqualityComparer.Default.Equals(x.Item1, y.Item1) &&
		PropertyInfoEqualityComparer.Default.Equals(x.Item2, y.Item2);

	public int GetHashCode((PropertyInfo, PropertyInfo) tuple) =>
		PropertyInfoEqualityComparer.Default.GetHashCode(tuple.Item1) ^
		PropertyInfoEqualityComparer.Default.GetHashCode(tuple.Item2);
}