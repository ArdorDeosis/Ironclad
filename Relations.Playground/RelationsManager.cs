using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Ironclad.Relations.Playground.Comparer;
using Ironclad.Relations.Playground.Relations;

namespace Ironclad.Relations.Playground;

public sealed partial class RelationsManager
{
	private readonly BindingFlags RelationalPropertyBindingFlags =
		BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
	
	private readonly Dictionary<(PropertyInfo, PropertyInfo), object> asymmetricRelations =
		new(PropertyInfoTupleEqualityComparer.Default);

	private readonly Dictionary<PropertyInfo, object> symmetricRelations =
		new(PropertyInfoEqualityComparer.Default);

	public FromRelationBuilder<TSelf> From<TSelf>(Expression<Func<TSelf, object?>> propertyExpression) => new(this,
		propertyExpression.Body is MemberExpression { Member: PropertyInfo propertyInfo }
			? propertyInfo
			: throw new InvalidOperationException());

	public FromRelationBuilder<TSelf> FromThisProperty<TSelf>() => new(this,
		typeof(TSelf).GetProperty(GetCallingPropertyName(), RelationalPropertyBindingFlags)
		?? throw new InvalidOperationException());

	private SingleValueRelationHandle<T, T> GetSingleValueRelationHandle<T>(PropertyInfo propertyInfo)
	{
		var relation = GetOrCreateSymmetricRelation<T>(propertyInfo);
		return new SingleValueRelationHandle<T, T>(relation.Get, relation.Set);
	}

	private SingleValueRelationHandle<TSource, TTarget> GetSingleValueRelationHandle<TSource, TTarget>(
		PropertyInfo source, PropertyInfo target)
	{
		switch (PropertyInfoComparer.Default.Compare(source, target))
		{
			case 0:
			{
				// this cannot fail, since PropertyInfoComparer only returns 0 if TSource and TTarget are the same type
				return (GetSingleValueRelationHandle<TSource>(source) as SingleValueRelationHandle<TSource, TTarget>)!;
			}
			case < 0:
			{
				var relation = GetOrCreateAsymmetricRelation<TSource, TTarget>((source, target));
				return new SingleValueRelationHandle<TSource, TTarget>(relation.GetForward, relation.SetForward);
			}
			case > 0:
			{
				var relation = GetOrCreateAsymmetricRelation<TTarget, TSource>((target, source));
				return new SingleValueRelationHandle<TSource, TTarget>(relation.GetBackward, relation.SetBackward);
			}
		}
	}

	private AsymmetricRelation<TSource, TTarget> GetOrCreateAsymmetricRelation<TSource, TTarget>(
		(PropertyInfo, PropertyInfo) key)
	{
		if (asymmetricRelations.TryGetValue(key, out var untypedRelation) &&
		    untypedRelation is not AsymmetricRelation<TSource, TTarget>)
			throw new Exception("wrong type");
		if (!asymmetricRelations.ContainsKey(key))
			asymmetricRelations[key] = new AsymmetricRelation<TSource, TTarget>(key);
		return (AsymmetricRelation<TSource, TTarget>)asymmetricRelations[key];
	}

	private SymmetricRelation<T> GetOrCreateSymmetricRelation<T>(PropertyInfo property)
	{
		if (symmetricRelations.TryGetValue(property, out var untypedRelation) &&
		    untypedRelation is not SymmetricRelation<T>)
			throw new Exception("wrong type");
		if (!symmetricRelations.ContainsKey(property))
			symmetricRelations[property] = new SymmetricRelation<T>(property);
		return (SymmetricRelation<T>)symmetricRelations[property];
	}


	// TODO: make a TryGet method
	private static string GetCallingPropertyName()
	{
		var stackTrace = new StackTrace();
		var callingMethod = stackTrace.GetFrame(2).GetMethod();
		var callingPropertyName = callingMethod.Name.StartsWith("get_") || callingMethod.Name.StartsWith("set_")
			? callingMethod.Name.Substring(4)
			: null;
		return callingPropertyName;
	}
}