namespace Ironclad.RandomNumbers;

/// <summary>
/// A deterministic discrete 7-dimensional noise field with 4-byte values.
/// Values are retrieved as <tt>float</tt> in range [0,1) or as raw <tt>uint</tt> values.
/// </summary>
public interface INoise7D4Byte : INoise6D4Byte
{
  /// <summary>
  /// Gets the raw <tt>uint</tt> noise value of the specified 7-dimensional coordinates.
  /// </summary>
  uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7);

  /// <summary>
  /// Gets the raw <tt>uint</tt> noise value of the specified 7-dimensional coordinates.
  /// </summary>
  uint Raw(int d1, int d2, int d3, int d4, int d5, int d6, int d7);

  /// <summary>
  /// Gets the noise value of the specified 7-dimensional coordinates. The value is in [0,1).
  /// </summary>
  float this[uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7] { get; }

  /// <summary>
  /// Gets the noise value of the specified 7-dimensional coordinates. The value is in [0,1).
  /// </summary>
  float this[int d1, int d2, int d3, int d4, int d5, int d6, int d7] { get; }
}