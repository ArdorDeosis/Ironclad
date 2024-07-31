namespace IronClad.Noise;

/// <summary>
/// A deterministic discrete 3-dimensional noise field with 4-byte values.
/// Values are retrieved as <tt>float</tt> in range [0,1) or as raw <tt>uint</tt> values.
/// </summary>
public interface INoise3D4Byte : INoise2D4Byte
{
  /// <summary>
  /// Gets the raw <tt>uint</tt> noise value of the specified 3-dimensional coordinates.
  /// </summary>
  uint Raw(uint d1, uint d2, uint d3);

  /// <summary>
  /// Gets the raw <tt>uint</tt> noise value of the specified 3-dimensional coordinates.
  /// </summary>
  uint Raw(int d1, int d2, int d3);

  /// <summary>
  /// Gets the noise value of the specified 3-dimensional coordinates. The value is in [0,1).
  /// </summary>
  float this[uint d1, uint d2, uint d3] { get; }

  /// <summary>
  /// Gets the noise value of the specified 3-dimensional coordinates. The value is in [0,1).
  /// </summary>
  float this[int d1, int d2, int d3] { get; }
}