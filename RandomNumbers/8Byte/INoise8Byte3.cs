namespace Ironclad.RandomNumbers;

public interface INoise8Byte3 : INoise8Byte2
{
  /// <summary>
  /// Gets the raw ulong noise value of the specified 3-dimensional coordinates.
  /// </summary>
  ulong Raw(ulong d1, ulong d2, ulong d3);

  /// <summary>
  /// Gets the raw ulong noise value of the specified 3-dimensional coordinates.
  /// </summary>
  ulong Raw(long d1, long d2, long d3);

  /// <summary>
  /// Gets the noise value of the specified 3-dimensional coordinates.
  /// </summary>
  double this[ulong d1, ulong d2, ulong d3] { get; }

  /// <summary>
  /// Gets the noise value of the specified 3-dimensional coordinates.
  /// </summary>
  double this[long d1, long d2, long d3] { get; }
}