using System.Collections;
using System.Collections.Immutable;

namespace Playground;

public class ParentChildRelation<TParent, TChild>
{
  private readonly Dictionary<TParent, ImmutableHashSet<TChild>> parentToChildren = new();
  private readonly Dictionary<TChild, TParent> childToParent = new();

  public ISetHandle<TChild> this[TParent parent] => new ParentHandle(this, parent);

  public class ParentHandle : ISetHandle<TChild>
  {
    private readonly ParentChildRelation<TParent, TChild> relation;
    private readonly TParent parent;
    internal ParentHandle(ParentChildRelation<TParent, TChild> relation, TParent parent)
    {
      this.relation = relation;
      this.parent = parent;
    }

    public event Action<IReadOnlyCollection<TChild>>? OnValueChanged;

    public IEnumerator<TChild> GetEnumerator() =>
      relation.parentToChildren.TryGetValue(parent, out var children) 
        ? children.GetEnumerator() 
        : Enumerable.Empty<TChild>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Add(TChild child)
    {
      if (relation.childToParent.TryGetValue(child, out var oldParent))
      {
        relation.parentToChildren[oldParent] = relation.parentToChildren[oldParent].rem
      }
      
      relation.childToParent[child] = parent;

      if (!relation.parentToChildren.TryGetValue(parent, out var children))
      {
        children = new HashSet<TChild>();
        relation.parentToChildren[parent] = children;
      }

      bool added = children.Add(child);
      if (added)
      {
        OnValueChanged?.Invoke(children);
      }

      return added;
    }

    public bool Remove(TChild child)
    {
      if (relation.childToParent.TryGetValue(child, out var p) && Equals(p, parent))
      {
        relation.childToParent.Remove(child);

        if (relation.parentToChildren.TryGetValue(parent, out var children) && children.Remove(child))
        {
          OnValueChanged?.Invoke(children);
          return true;
        }
      }

      return false;
    }

    public bool Contains(TChild child)
    {
      return relation.childToParent.TryGetValue(child, out var p) && Equals(p, parent);
    }

    public void Clear()
    {
      if (relation.parentToChildren.TryGetValue(parent, out var children))
      {
        foreach (var child in children)
        {
          relation.childToParent.Remove(child);
        }

        children.Clear();
        OnValueChanged?.Invoke(children);
      }
    }
  }

  public class ChildHandle : IValueHandle<TParent>
  {
    private readonly ParentChildRelation<TParent, TChild> relation;
    private readonly TChild child;
    internal ChildHandle(ParentChildRelation<TParent, TChild> relation, TChild child)
    {
      this.relation = relation;
      this.child = child;
    }

    public TParent? Get() => throw new NotImplementedException();

    public void Set(TParent? value)
    {
      throw new NotImplementedException();
    }

    public void Unset()
    {
      throw new NotImplementedException();
    }

    public event Action<TParent?>? OnValueChanged;
  }
}