namespace Ironclad.RandomNumbers;

/// <inheritdoc cref="INoise4Byte9"/>
public class Noise4Byte : INoise4Byte9
{
  private readonly uint seed;

  private const uint Prime1 = 0xBD4BCB5u;
  private const uint Prime2 = 0x63D68Du;
  private const uint Prime3 = 0xBD7C9u;
  private const uint Prime4 = 0xAE23u;
  private const uint Prime5 = 0x5B5E1A3u;
  private const uint Prime6 = 0xE1A363u;
  private const uint Prime7 = 0xA5AB5u;
  private const uint Prime8 = 0x95E3u;

  /// <summary>
  /// Initializes a new instance with seed 0.
  /// </summary>
  public Noise4Byte() { }

  /// <summary>
  /// Initializes a new instance with the specified seed.
  /// </summary>
  public Noise4Byte(uint seed) => this.seed = seed;

  /// <summary>
  /// Initializes a new instance with the specified seed.
  /// </summary>
  public Noise4Byte(int seed) => this.seed = unchecked((uint)seed);

  /// <summary>
  /// Initializes a new instance with a seed derived from the hash code of the specified object.
  /// </summary>
  public Noise4Byte(object seed) => this.seed = unchecked((uint)seed.GetHashCode());

  /// <inheritdoc />
  public uint Raw(uint d1) => Squirrel3(d1);

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2) => Squirrel3(unchecked(d1 + d2 * Prime1));

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2, uint d3) => Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2));

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2, uint d3, uint d4) =>
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3));

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5) =>
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4));

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5, uint d6) =>
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5));

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7) => 
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6));

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7, uint d8) => 
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6 ^ d8 * Prime7));

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7, uint d8, uint d9) => 
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6 ^ d8 * Prime7 ^ d9 * Prime8));
  
  /// <inheritdoc />
  public uint Raw(int d1) => Raw(unchecked((uint)d1));

  /// <inheritdoc />
  public uint Raw(int d1, int d2) => Raw(unchecked((uint)d1), unchecked((uint)d2));

  /// <inheritdoc />
  public uint Raw(int d1, int d2, int d3) => Raw(unchecked((uint)d1), unchecked((uint)d2), unchecked((uint)d3));

  /// <inheritdoc />
  public uint Raw(int d1, int d2, int d3, int d4) => 
    Raw(unchecked((uint)d1), unchecked((uint)d2), unchecked((uint)d3), unchecked((uint)d4));

  /// <inheritdoc />
  public uint Raw(int d1, int d2, int d3, int d4, int d5) => 
    Raw(unchecked((uint)d1), unchecked((uint)d2), unchecked((uint)d3), unchecked((uint)d4), unchecked((uint)d5));

  /// <inheritdoc />
  public uint Raw(int d1, int d2, int d3, int d4, int d5, int d6) => 
    Raw(unchecked((uint)d1), unchecked((uint)d2), unchecked((uint)d3), unchecked((uint)d4), unchecked((uint)d5), unchecked((uint)d6));

  /// <inheritdoc />
  public uint Raw(int d1, int d2, int d3, int d4, int d5, int d6, int d7) => 
    Raw(unchecked((uint)d1), unchecked((uint)d2), unchecked((uint)d3), unchecked((uint)d4), unchecked((uint)d5), unchecked((uint)d6), unchecked((uint)d7));

  /// <inheritdoc />
  public uint Raw(int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8) => 
    Raw(unchecked((uint)d1), unchecked((uint)d2), unchecked((uint)d3), unchecked((uint)d4), unchecked((uint)d5), unchecked((uint)d6), unchecked((uint)d7), unchecked((uint)d8));

  /// <inheritdoc />
  public uint Raw(int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8, int d9) => 
    Raw(unchecked((uint)d1), unchecked((uint)d2), unchecked((uint)d3), unchecked((uint)d4), unchecked((uint)d5), unchecked((uint)d6), unchecked((uint)d7), unchecked((uint)d8), unchecked((uint)d9));

  /// <inheritdoc />
  public float this[uint d1] => Raw(d1) / (float)uint.MaxValue;

  /// <inheritdoc />
  public float this[uint d1, uint d2] => Raw(unchecked(d1 + d2 * Prime1)) / (float)uint.MaxValue;

  /// <inheritdoc />
  public float this[uint d1, uint d2, uint d3] => Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2)) / (float)uint.MaxValue;

  /// <inheritdoc />
  public float this[uint d1, uint d2, uint d3, uint d4] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3)) / (float)uint.MaxValue;

  /// <inheritdoc />
  public float this[uint d1, uint d2, uint d3, uint d4, uint d5] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4)) / (float)uint.MaxValue;

  /// <inheritdoc />
  public float this[uint d1, uint d2, uint d3, uint d4, uint d5, uint d6] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5)) / (float)uint.MaxValue;

  /// <inheritdoc />
  public float this[uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6)) /
    (float)uint.MaxValue;

  /// <inheritdoc />
  public float this[uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7, uint d8] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6 ^
                  d8 * Prime7)) / (float)uint.MaxValue;

  /// <inheritdoc />
  public float this[uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7, uint d8, uint d9] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6 ^ d8 * Prime7 ^
                  d9 * Prime8)) / (float)uint.MaxValue;

  /// <inheritdoc />
  public float this[int d1] => this[unchecked((uint)d1)];

  /// <inheritdoc />
  public float this[int d1, int d2] => this[unchecked((uint)d1), unchecked((uint)d2)];

  /// <inheritdoc />
  public float this[int d1, int d2, int d3] => this[unchecked((uint)d1), unchecked((uint)d2), unchecked((uint)d3)];

  /// <inheritdoc />
  public float this[int d1, int d2, int d3, int d4] => this[unchecked((uint)d1), unchecked((uint)d2),
    unchecked((uint)d3), unchecked((uint)d4)];

  /// <inheritdoc />
  public float this[int d1, int d2, int d3, int d4, int d5] => this[unchecked((uint)d1), unchecked((uint)d2),
    unchecked((uint)d3), unchecked((uint)d4), unchecked((uint)d5)];

  /// <inheritdoc />
  public float this[int d1, int d2, int d3, int d4, int d5, int d6] => this[unchecked((uint)d1), unchecked((uint)d2),
    unchecked((uint)d3), unchecked((uint)d4), unchecked((uint)d5), unchecked((uint)d6)];

  /// <inheritdoc />
  public float this[int d1, int d2, int d3, int d4, int d5, int d6, int d7] => this[unchecked((uint)d1),
    unchecked((uint)d2), unchecked((uint)d3), unchecked((uint)d4), unchecked((uint)d5), unchecked((uint)d6),
    unchecked((uint)d7)];

  /// <inheritdoc />
  public float this[int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8] => this[unchecked((uint)d1),
    unchecked((uint)d2), unchecked((uint)d3), unchecked((uint)d4), unchecked((uint)d5), unchecked((uint)d6),
    unchecked((uint)d7), unchecked((uint)d8)];

  /// <inheritdoc />
  public float this[int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8, int d9] => this[unchecked((uint)d1),
    unchecked((uint)d2), unchecked((uint)d3), unchecked((uint)d4), unchecked((uint)d5), unchecked((uint)d6),
    unchecked((uint)d7), unchecked((uint)d8), unchecked((uint)d9)];

  private uint Squirrel3(uint x)
  {
    const uint bitNoise1 = 0xB5297A4D;
    const uint bitNoise2 = 0x68E31DA4;
    const uint bitNoise3 = 0x1B56C4E9;

    unchecked
    {
      var mangled = x;
      mangled *= bitNoise1;
      mangled += seed;
      mangled ^= mangled >> 8;
      mangled += bitNoise2;
      mangled ^= mangled << 8;
      mangled *= bitNoise3;
      mangled ^= mangled >> 8;
      return mangled;
    }
  }
}