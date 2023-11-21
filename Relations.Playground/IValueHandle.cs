namespace Playground;

public interface IValueHandle<T>
{
  T? Get();
  void Set(T? value);
  void Unset();
  event Action<T?> OnValueChanged;
}