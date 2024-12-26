namespace Ironclad.Relations.Playground;

[AttributeUsage(AttributeTargets.Property)]
public class RelationAttribute<T> : Attribute
{
  public string PropertyName { get; }

  public RelationAttribute(string propertyName)
  {
    if (string.IsNullOrWhiteSpace(propertyName))
      throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));

    var property = typeof(T).GetProperty(propertyName);
    if (property == null || property.PropertyType != typeof(T)) // TODO: type check needs to go
      throw new ArgumentException($"'{propertyName}' is not a valid property of type '{typeof(T).Name}' or does not match the required type.", nameof(propertyName));

    PropertyName = propertyName;
  }
}

[AttributeUsage(AttributeTargets.Property)]
public class RelationAttribute : Attribute
{
  public string? PropertyName { get; }

  public RelationAttribute(string? propertyName = null)
  {
    PropertyName = propertyName;
  }
}