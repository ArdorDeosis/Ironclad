﻿namespace Ironclad.RandomNumbers;

public interface INoise8Byte6 : INoise8Byte5
{
  /// <summary>
  /// Gets the raw ulong noise value of the specified 6-dimensional coordinates.
  /// </summary>
  ulong Raw(ulong d1, ulong d2, ulong d3, ulong d4, ulong d5, ulong d6);

  /// <summary>
  /// Gets the raw ulong noise value of the specified 6-dimensional coordinates.
  /// </summary>
  ulong Raw(long d1, long d2, long d3, long d4, long d5, long d6);

  /// <summary>
  /// Gets the noise value of the specified 6-dimensional coordinates.
  /// </summary>
  double this[ulong d1, ulong d2, ulong d3, ulong d4, ulong d5, ulong d6] { get; }

  /// <summary>
  /// Gets the noise value of the specified 6-dimensional coordinates.
  /// </summary>
  double this[long d1, long d2, long d3, long d4, long d5, long d6] { get; }
}