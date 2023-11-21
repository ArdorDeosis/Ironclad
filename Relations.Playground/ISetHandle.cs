namespace Playground;

public interface ISetHandle<TChild> : IEnumerable<TChild>
{
  bool Add(TChild child);
  bool Remove(TChild child);
  bool Contains(TChild child);
  void Clear();

  event Action<IReadOnlyCollection<TChild>> OnValueChanged;
}