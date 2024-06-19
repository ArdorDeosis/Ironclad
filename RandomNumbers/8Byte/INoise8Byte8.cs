namespace Ironclad.RandomNumbers;

public interface INoise8Byte8 : INoise8Byte7
{
  /// <summary>
  /// Gets the raw ulong noise value of the specified 8-dimensional coordinates.
  /// </summary>
  ulong Raw(ulong d1, ulong d2, ulong d3, ulong d4, ulong d5, ulong d6, ulong d7, ulong d8);

  /// <summary>
  /// Gets the raw ulong noise value of the specified 8-dimensional coordinates.
  /// </summary>
  ulong Raw(long d1, long d2, long d3, long d4, long d5, long d6, long d7, long d8);

  /// <summary>
  /// Gets the noise value of the specified 8-dimensional coordinates.
  /// </summary>
  double this[ulong d1, ulong d2, ulong d3, ulong d4, ulong d5, ulong d6, ulong d7, ulong d8] { get; }

  /// <summary>
  /// Gets the noise value of the specified 8-dimensional coordinates.
  /// </summary>
  double this[long d1, long d2, long d3, long d4, long d5, long d6, long d7, long d8] { get; }
}