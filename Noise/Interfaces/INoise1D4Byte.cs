namespace Ironclad.Noise;

/// <summary>
/// A deterministic discrete 1-dimensional noise field with 4-byte values.
/// Values are retrieved as <tt>float</tt> in range [0,1) or as raw <tt>uint</tt> values.
/// </summary>
public interface INoise1D4Byte
{
  /// <summary>
  /// Gets the raw <tt>uint</tt> noise value of the specified 1-dimensional coordinate.
  /// </summary>
  uint Raw(uint d1);

  /// <summary>
  /// Gets the raw <tt>uint</tt> noise value of the specified 1-dimensional coordinate.
  /// </summary>
  uint Raw(int d1);

  /// <summary>
  /// Gets the noise value of the specified 1-dimensional coordinate.
  /// </summary>
  float this[uint d1] { get; }

  /// <summary>
  /// Gets the noise value of the specified 1-dimensional coordinate.
  /// </summary>
  float this[int d1] { get; }
}