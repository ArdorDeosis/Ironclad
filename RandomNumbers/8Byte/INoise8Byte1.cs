namespace Ironclad.RandomNumbers;

public interface INoise8Byte1
{
  /// <summary>
  /// Gets the raw ulong noise value of the specified 1-dimensional coordinates.
  /// </summary>
  ulong Raw(ulong d1);

  /// <summary>
  /// Gets the raw ulong noise value of the specified 1-dimensional coordinates.
  /// </summary>
  ulong Raw(long d1);

  /// <summary>
  /// Gets the noise value of the specified 1-dimensional coordinates.
  /// </summary>
  double this[ulong d1] { get; }

  /// <summary>
  /// Gets the noise value of the specified 1-dimensional coordinates.
  /// </summary>
  double this[long d1] { get; }
}