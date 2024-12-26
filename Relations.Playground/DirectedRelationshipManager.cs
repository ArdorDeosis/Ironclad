namespace Ironclad.Relations.Playground;

public class DirectedRelationshipManager<A, B> where A : class where B : class
{
  private readonly Dictionary<A, B> dictionaryAB = new();
  private readonly Dictionary<B, A> dictionaryBA = new();

  public B? GetB(A self) => dictionaryAB.GetValueOrDefault(self);
  public A? GetA(B self) => dictionaryBA.GetValueOrDefault(self);

  public void SetA(B self, A? value)
  {
    if (value is null)
      UnsetA(self);
    else
      Set(value, self);
  }
  
  public void SetB(A self, B? value)
  {
    if (value is null)
      UnsetB(self);
    else
      Set(self, value);
  }

  private void Set(A a, B b)
  {
    if (dictionaryAB.ContainsKey(a))
      throw new InvalidOperationException();
    if (dictionaryBA.ContainsKey(b))
      throw new InvalidOperationException();
    dictionaryAB.Add(a, b);
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
    if (dictionaryAB.TryGetValue(self, out var value))
      dictionaryBA.Remove(value);
    dictionaryAB.Remove(self);
  }
}