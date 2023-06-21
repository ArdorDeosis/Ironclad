using System.Linq.Expressions;
using System.Reflection;

namespace Ironclad.Relations.Playground;

public sealed partial class RelationsManager
{
	public class FromRelationBuilder<TSelf>
	{
		private readonly RelationsManager manager;
		private readonly PropertyInfo source;

		internal FromRelationBuilder(RelationsManager relationsManager, PropertyInfo source)
		{
			manager = relationsManager;
			this.source = source;
		}

		public SingleValueRelationHandle<TSelf, TSelf> ToSelf =>
			manager.GetSingleValueRelationHandle<TSelf>(source);

		public SingleValueRelationHandle<TSelf, TTarget> To<TTarget>(Expression<Func<TTarget, TSelf?>> targetMember)
		{
			if (targetMember.Body is not MemberExpression { Member: PropertyInfo target })
				throw new ArgumentException( /* TODO */);
			return manager.GetSingleValueRelationHandle<TSelf, TTarget>(source, target);
		}
	}
}