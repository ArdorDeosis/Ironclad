namespace Ironclad.RandomNumbers;

public interface INoise8Byte5 : INoise8Byte4
{
  /// <summary>
  /// Gets the raw ulong noise value of the specified 5-dimensional coordinates.
  /// </summary>
  ulong Raw(ulong d1, ulong d2, ulong d3, ulong d4, ulong d5);

  /// <summary>
  /// Gets the raw ulong noise value of the specified 5-dimensional coordinates.
  /// </summary>
  ulong Raw(long d1, long d2, long d3, long d4, long d5);

  /// <summary>
  /// Gets the noise value of the specified 5-dimensional coordinates.
  /// </summary>
  double this[ulong d1, ulong d2, ulong d3, ulong d4, ulong d5] { get; }

  /// <summary>
  /// Gets the noise value of the specified 5-dimensional coordinates.
  /// </summary>
  double this[long d1, long d2, long d3, long d4, long d5] { get; }
}