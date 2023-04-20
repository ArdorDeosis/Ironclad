namespace Ironclad.Collections;

/// <summary>
/// Extension methods for <see cref="ValueList{T}"/>.
/// </summary>
public static class ValueListExtensions
{
  /// <summary>
  /// Converts an enumerable to a <see cref="ValueList{T}"/>.
  /// </summary>
  public static ValueList<T> AsValueList<T>(this IEnumerable<T> enumerable) => new(enumerable.ToArray());
}