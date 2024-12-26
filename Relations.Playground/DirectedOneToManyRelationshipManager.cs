using System.Collections.Immutable;

namespace Ironclad.Relations.Playground;

public class DirectedOneToManyRelationshipManager<A, B> where A : class where B : class
{
  private readonly Dictionary<A, ImmutableHashSet<B>> dictionaryAB = new();
  private readonly Dictionary<B, A> dictionaryBA = new();

  public IReadOnlySet<B> GetB(A self) => dictionaryAB.GetValueOrDefault(self, []);
  public A? GetA(B self) => dictionaryBA.GetValueOrDefault(self);

  public void SetA(B self, A? value)
  {
    if (value is null)
      UnsetA(self);
    else
      Add(value, self);
  }
  
  public void SetB(A self, B? value)
  {
    if (value is null)
      UnsetB(self);
    else
      Add(self, value);
  }

  private void Add(A a, B b)
  {
    if (dictionaryBA.ContainsKey(b))
      throw new InvalidOperationException();
    if (!dictionaryAB.ContainsKey(a))
      dictionaryAB[a] = ImmutableHashSet<B>.Empty;
    dictionaryAB[a] = dictionaryAB[a].Add(b);
    dictionaryBA.Add(b, a);
  }

  private void UnsetA(B self)
  {
    if (dictionaryBA.TryGetValue(self, out var value))
      dictionaryAB.Remove(value);
    dictionaryBA.Remove(self);
  }
  
  private void UnsetB(A self)
  {
    foreach (var element in dictionaryAB.GetValueOrDefault(self, [])) 
      dictionaryBA.Remove(element);
    dictionaryAB.Remove(self);
  }
}