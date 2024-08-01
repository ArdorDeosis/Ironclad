namespace Ironclad.Noise;

/// <summary>
/// A deterministic discrete 8-dimensional noise field with 8-byte values.
/// Values are retrieved as <tt>double</tt> in range [0,1) or as raw <tt>ulong</tt> values.
/// </summary>
public interface INoise8D8Byte : INoise7D8Byte
{
  /// <summary>
  /// Gets the raw <tt>ulong</tt> noise value of the specified 8-dimensional coordinates.
  /// </summary>
  ulong Raw(ulong d1, ulong d2, ulong d3, ulong d4, ulong d5, ulong d6, ulong d7, ulong d8);

  /// <summary>
  /// Gets the raw <tt>ulong</tt> noise value of the specified 8-dimensional coordinates.
  /// </summary>
  ulong Raw(long d1, long d2, long d3, long d4, long d5, long d6, long d7, long d8);

  /// <summary>
  /// Gets the noise value of the specified 8-dimensional coordinates. The value is in [0,1).
  /// </summary>
  double this[ulong d1, ulong d2, ulong d3, ulong d4, ulong d5, ulong d6, ulong d7, ulong d8] { get; }

  /// <summary>
  /// Gets the noise value of the specified 8-dimensional coordinates. The value is in [0,1).
  /// </summary>
  double this[long d1, long d2, long d3, long d4, long d5, long d6, long d7, long d8] { get; }
}