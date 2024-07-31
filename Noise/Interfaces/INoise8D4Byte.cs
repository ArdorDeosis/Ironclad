namespace IronClad.Noise;

/// <summary>
/// A deterministic discrete 8-dimensional noise field with 4-byte values.
/// Values are retrieved as <tt>float</tt> in range [0,1) or as raw <tt>uint</tt> values.
/// </summary>
public interface INoise8D4Byte : INoise7D4Byte
{
  /// <summary>
  /// Gets the raw <tt>uint</tt> noise value of the specified 8-dimensional coordinates.
  /// </summary>
  uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7, uint d8);

  /// <summary>
  /// Gets the raw <tt>uint</tt> noise value of the specified 8-dimensional coordinates.
  /// </summary>
  uint Raw(int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8);

  /// <summary>
  /// Gets the noise value in [0,1) of the specified 8-dimensional coordinates.
  /// </summary>
  float this[uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7, uint d8] { get; }

  /// <summary>
  /// Gets the noise value in [0,1) of the specified 8-dimensional coordinates.
  /// </summary>
  float this[int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8] { get; }
}