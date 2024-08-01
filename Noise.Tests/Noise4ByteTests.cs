using FluentAssertions;
using Ironclad.Noise;

namespace Ironclad.Noise.Tests;

public class Noise4ByteTests
{
  /// <summary>
  /// The first 20 numbers for the given seeds. These are used with tests below to ensure that the random number
  /// generation algorithm is not changed by accident and new versions are backwards compatible.
  /// </summary>
  private static readonly Dictionary<uint, uint[]> KnownValues = new()
  {
    [0u] =
    [
      436901570, 725778245, 61355516, 3809932532, 2923951477, 4286858955, 357163824, 2278085025, 1639418064, 2704673657,
      2964920448, 3882064412, 3534803571, 3677408160, 515893116, 3959362836, 536219454, 3552393794, 3714984180,
      2518424606,
    ],
    [0x55555555] =
    [
      624268994, 3177249880, 1038578418, 746777272, 3122647973, 1310234274, 267170437, 2117211699, 1739200436,
      2198848355, 3705128008, 4003412366, 1507409328, 2921268404, 685204254, 2197018858, 1906475369, 3487740453,
      317716728, 1219248106,
    ],
    [0xAAAAAAAA] =
    [
      4041250498, 1407392230, 2362940753, 4085822404, 774560897, 3902325315, 940757327, 3830311456, 3359796652,
      2340603619, 2585685121, 3649612891, 2705337513, 1652737227, 2527959340, 1970621616, 2310482581, 3099046077,
      3450551884, 1834062995,
    ],
    [0x0F0F0F0F] =
    [
      1264293570, 750528665, 1625890409, 354544926, 181241912, 342836939, 1184702759, 1658925800, 798001405, 3902473613,
      3600663788, 1334849522, 3186153472, 788565187, 798592501, 3573387615, 2704847033, 3413575842, 782178773,
      793207490,
    ],
    [0xF0F0F0F0] =
    [
      3403323074, 659140949, 1416850183, 1721519800, 2999507409, 2849024803, 3059218991, 4196415668, 384210736,
      2641892975, 3085964101, 2732170751, 3395594093, 2373041944, 4008268663, 643115956, 1481578144, 3405584008,
      4207015768, 2627383851,
    ],
    [0xFFFFFFFF] =
    [
      3148584642, 3119785218, 2434695049, 1906871640, 2113444784, 1910996276, 436683917, 3263135636, 2323115538,
      3721971004, 1961785239, 1506169888, 101780089, 1313392807, 127837549, 2021698431, 1212945328, 2560110544,
      886999511, 1531407875,
    ],
  };

  private static IEnumerable<(object seedObject, uint expectedSeed)> SeedObjectsAndSeeds =>
    new[]
    {
      "some string",
      true,
      new object(),
      new { Name = "Horst", Number = 1337 },
      (int[]) [1, 2, 3, 4, 5],
    }.Select(obj => (obj, unchecked((uint)obj.GetHashCode())));

  [Test]
  public void DefaultSeed_IsZero()
  {
    // ARRANGE
    var noise = new Noise4Byte();

    // ASSERT
    noise.Seed.Should().Be(0);
  }

  [Test]
  public void NullSeed_SeedIsZero()
  {
    // ARRANGE
    var noise = new Noise4Byte(null);

    // ASSERT
    noise.Seed.Should().Be(0);
  }

  [TestCase(-1, uint.MaxValue)]
  [TestCase(int.MinValue, 0x80000000)]
  public void NegativeIntSeed_IsExpectedUIntSeed(int seed, uint expectedSeed)
  {
    // ARRANGE
    var noise = new Noise4Byte(seed);

    // ASSERT
    noise.Seed.Should().Be(expectedSeed);
  }

  [TestCaseSource(nameof(SeedObjectsAndSeeds))]
  public void ObjectSeed_IsObjectHashCode((object seedObject, uint expectedSeed) obj)
  {
    // ARRANGE
    var noise = new Noise4Byte(obj.seedObject);

    // ASSERT
    noise.Seed.Should().Be(obj.expectedSeed);
  }

  [TestCaseSource(nameof(KnownValues))]
  public void SampleValuesAreAsExpected(KeyValuePair<uint, uint[]> expected)
  {
    // ARRANGE
    var noise = new Noise4Byte(expected.Key);

    // ACT
    var values = Enumerable.Range(0, 20).Select(n => noise.Raw(n)).ToArray();

    // ASSERT
    values.Should().BeEquivalentTo(expected.Value);
  }

  [Test]
  public void UnusedDimensionsZero_ValuesAreEqual()
  {
    // ARRANGE
    const uint c = 0xC0FFEE;
    var noise = new Noise4Byte();
    var expectedValue = noise[c];

    // ACT
    float[] values =
    [
      noise[c, 0],
      noise[c, 0, 0],
      noise[c, 0, 0, 0],
      noise[c, 0, 0, 0, 0],
      noise[c, 0, 0, 0, 0, 0],
      noise[c, 0, 0, 0, 0, 0, 0],
      noise[c, 0, 0, 0, 0, 0, 0, 0],
      noise[c, 0, 0, 0, 0, 0, 0, 0, 0],
    ];
    
    // ASSERT
    values.Should().AllBeEquivalentTo(expectedValue);
  }

  /// <summary>
  /// This checks sample-wise that the way coordinates are combined is not changed by accident and stays consistent. 
  /// </summary>
  [Test]
  public void MultiDimensionalSampleValuesAreAsExpected()
  {
    // ARRANGE
    var noise = new Noise4Byte();

    // ACT
    var value1 = noise.Raw(1, 2, 3, 4, 5, 6, 7, 8, 9);
    var value2 = noise.Raw(42, 69, 1337, 0xBEEF, 0xDEAD, 0xF00D, 0xC0FFEE, int.MinValue, int.MaxValue);

    // ASSERT
    Assert.Multiple(() =>
    {
      value1.Should().Be(3722053045);
      value2.Should().Be(167249036);
    });
  }
}