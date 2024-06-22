namespace Ironclad.RandomNumbers;

/// <summary>
/// A deterministic discrete 2-dimensional noise field with 8-byte values.
/// Values are retrieved as <tt>double</tt> in range [0,1) or as raw <tt>ulong</tt> values.
/// </summary>
public interface INoise2D8Byte : INoise1D8Byte
{
  /// <summary>
  /// Gets the raw <tt>ulong</tt> noise value of the specified 2-dimensional coordinates.
  /// </summary>
  ulong Raw(ulong d1, ulong d2);

  /// <summary>
  /// Gets the raw <tt>ulong</tt> noise value of the specified 2-dimensional coordinates.
  /// </summary>
  ulong Raw(long d1, long d2);

  /// <summary>
  /// Gets the noise value of the specified 2-dimensional coordinates. The value is in [0,1).
  /// </summary>
  double this[ulong d1, ulong d2] { get; }

  /// <summary>
  /// Gets the noise value of the specified 2-dimensional coordinates. The value is in [0,1).
  /// </summary>
  double this[long d1, long d2] { get; }
}