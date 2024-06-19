namespace Ironclad.RandomNumbers;

/// <summary>
/// A 4-byte noise generator with up to 7 dimensions of input.
/// </summary>
public interface INoise4Byte7 : INoise4Byte6
{
  /// <summary>
  /// Gets the raw uint noise value of the specified 7-dimensional coordinates.
  /// </summary>
  uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7);

  /// <summary>
  /// Gets the raw uint noise value of the specified 7-dimensional coordinates.
  /// </summary>
  uint Raw(int d1, int d2, int d3, int d4, int d5, int d6, int d7);

  /// <summary>
  /// Gets the noise value of the specified 7-dimensional coordinates.
  /// </summary>
  float this[uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7] { get; }

  /// <summary>
  /// Gets the noise value of the specified 7-dimensional coordinates.
  /// </summary>
  float this[int d1, int d2, int d3, int d4, int d5, int d6, int d7] { get; }
}