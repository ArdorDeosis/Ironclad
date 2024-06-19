namespace Ironclad.RandomNumbers;

public interface INoise4Byte9 : INoise4Byte8
{
  /// <summary>
  /// Gets the raw uint noise value of the specified 9-dimensional coordinates.
  /// </summary>
  uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7, uint d8, uint d9);

  /// <summary>
  /// Gets the raw uint noise value of the specified 9-dimensional coordinates.
  /// </summary>
  uint Raw(int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8, int d9);

  /// <summary>
  /// Gets the noise value of the specified 9-dimensional coordinates.
  /// </summary>
  float this[uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7, uint d8, uint d9] { get; }

  /// <summary>
  /// Gets the noise value of the specified 9-dimensional coordinates.
  /// </summary>
  float this[int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8, int d9] { get; }
}