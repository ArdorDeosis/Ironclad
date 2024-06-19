namespace Ironclad.RandomNumbers;

public interface INoise8Byte2 : INoise8Byte1
{
  /// <summary>
  /// Gets the raw ulong noise value of the specified 2-dimensional coordinates.
  /// </summary>
  ulong Raw(ulong d1, ulong d2);

  /// <summary>
  /// Gets the raw ulong noise value of the specified 2-dimensional coordinates.
  /// </summary>
  ulong Raw(long d1, long d2);

  /// <summary>
  /// Gets the noise value of the specified 2-dimensional coordinates.
  /// </summary>
  double this[ulong d1, ulong d2] { get; }

  /// <summary>
  /// Gets the noise value of the specified 2-dimensional coordinates.
  /// </summary>
  double this[long d1, long d2] { get; }
}