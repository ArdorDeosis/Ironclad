﻿namespace Ironclad.Noise;

/// <summary>
/// A deterministic discrete 5-dimensional noise field with 8-byte values.
/// Values are retrieved as <tt>double</tt> in range [0,1) or as raw <tt>ulong</tt> values.
/// </summary>
public interface INoise5D8Byte : INoise4D8Byte
{
  /// <summary>
  /// Gets the raw <tt>ulong</tt> noise value of the specified 5-dimensional coordinates.
  /// </summary>
  ulong Raw(ulong d1, ulong d2, ulong d3, ulong d4, ulong d5);

  /// <summary>
  /// Gets the raw <tt>ulong</tt> noise value of the specified 5-dimensional coordinates.
  /// </summary>
  ulong Raw(long d1, long d2, long d3, long d4, long d5);

  /// <summary>
  /// Gets the noise value of the specified 5-dimensional coordinates. The value is in [0,1).
  /// </summary>
  double this[ulong d1, ulong d2, ulong d3, ulong d4, ulong d5] { get; }

  /// <summary>
  /// Gets the noise value of the specified 5-dimensional coordinates. The value is in [0,1).
  /// </summary>
  double this[long d1, long d2, long d3, long d4, long d5] { get; }
}