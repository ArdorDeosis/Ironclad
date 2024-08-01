using FluentAssertions;
using IronClad.Noise;

namespace Ironclad.Noise.Tests;

public class Noise8ByteTests
{
  /// <summary>
  /// The first 20 numbers for the given seeds. These are used with tests below to ensure that the random number
  /// generation algorithm is not changed by accident and new versions are backwards compatible.
  /// </summary>
  private static readonly Dictionary<ulong, ulong[]> KnownValues = new()
  {
    [0] =
    [
      6764628895707808592, 5882133317350848853, 11951894994304149035, 13294171988278088418, 2330935729388435525,
      16023515165809802723, 14312819691223602580, 12565994838564215313, 7480587549707826127, 9237311760937106109,
      283902309744445277, 10025801383852320220, 5371544290219147953, 15486313094954134632, 9512776467283037159,
      6491847876011033409, 2028951921919330738, 17854222535870376942, 5347139711870392830, 5655392625374497007,
    ],
    [0xFFFFFFFFFFFFFFFF] =
    [
      7698844343410475856, 3636865164005918265, 3068383573920484936, 13635413618604643340, 17399819699489776805,
      3855796898201212953, 10731574310959563597, 370868971189443537, 7110255600758028138, 10893792155319938068,
      11224004791048366626, 14398182455476846082, 18235803915919968737, 15719271570327609926, 7852152626410008115,
      906586329287835855, 8300638569281985979, 4945996794928621354, 12920588587177808569, 13875914200308195598,
    ],
    [0x5555555555555555] =
    [
      10102077694566056784, 6782355366613907071, 6958772521374915260, 6516728238408728221, 13989062934355062338,
      12189769191287069674, 15880395311838987497, 269265513180472898, 4633784641253257484, 7633060483843386467,
      12110509568358825376, 7530057685910906543, 3253744947228602820, 9121880270701577864, 103968472445151659,
      1856477985802509898, 1244393307866157333, 14401714639273128920, 15977216166099123119, 16771042954170632133,
    ],
    [0xAAAAAAAAAAAAAAAA] =
    [
      8919038367451169616, 14916508721643887995, 18403816404652440338, 12102203940297486196, 15773438790064441148,
      985000937066344696, 2857713471658923966, 12081179254300688261, 13333583045820166866, 6877356219458007377,
      16185830702618318132, 9616019481866111035, 5281882315061365889, 10789767769665379896, 2488609200464071065,
      6908860372862578453, 2543530788597243238, 3101598867320514483, 1503902695624625579, 5768040219063340385,
    ],
    [0x0F0F0F0F0F0F0F0F] =
    [
      13404905071288894288, 17234438589145509413, 6863473916898115197, 14961840294548923457, 2955667448731238260,
      14183162782790652381, 15921317645191358244, 8802174016439241755, 9145643176177361527, 14111683146102309561,
      2677502650958846329, 4646943078412690023, 11893011041499337166, 1603669903263737868, 9386443279781094192,
      14570107419453682250, 7640903026933504398, 11676520945597878798, 8395151076409985669, 12097135514581358649,
    ],
    [0xF0F0F0F0F0F0F0F0] =
    [
      5616210990728332112, 15349808872856530121, 9899558632211064764, 17908927916494497850, 17509111774923022170,
      14465488736714363995, 2724910217564735655, 5540364481154462812, 6062104131899445602, 11326255775166793511,
      18024768399099911976, 3246235909584727520, 1409646004567359968, 3774172251395062895, 6703603296218858961,
      1126603016737510549, 9831609220395635918, 9105527187736953981, 16580640951927859980, 13586312661341310259,
    ],
  };

  private static IEnumerable<(object seedObject, ulong expectedSeed)> SeedObjectsAndSeeds =>
    new[]
    {
      "some string",
      true,
      new object(),
      new { Name = "Horst", Number = 1337 },
      (int[]) [1, 2, 3, 4, 5],
    }.Select(obj => (obj, unchecked((ulong)obj.GetHashCode())));

  [Test]
  public void DefaultSeed_IsZero()
  {
    // ARRANGE
    var noise = new Noise8Byte();

    // ASSERT
    noise.Seed.Should().Be(0);
  }

  [Test]
  public void NullSeed_SeedIsZero()
  {
    // ARRANGE
    var noise = new Noise8Byte(null);

    // ASSERT
    noise.Seed.Should().Be(0);
  }

  [TestCase(-1, ulong.MaxValue)]
  [TestCase(long.MinValue, 0x8000000000000000)]
  public void NegativeIntSeed_IsExpectedULongSeed(long seed, ulong expectedSeed)
  {
    // ARRANGE
    var noise = new Noise8Byte(seed);

    // ASSERT
    noise.Seed.Should().Be(expectedSeed);
  }

  [TestCaseSource(nameof(SeedObjectsAndSeeds))]
  public void ObjectSeed_IsObjectHashCode((object seedObject, ulong expectedSeed) obj)
  {
    // ARRANGE
    var noise = new Noise8Byte(obj.seedObject);

    // ASSERT
    noise.Seed.Should().Be(obj.expectedSeed);
  }

  [TestCaseSource(nameof(KnownValues))]
  public void SampleValuesAreAsExpected(KeyValuePair<ulong, ulong[]> expected)
  {
    // ARRANGE
    var noise = new Noise8Byte(expected.Key);

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
    var noise = new Noise8Byte();
    var expectedValue = noise[c];

    // ACT
    double[] values =
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
    var noise = new Noise8Byte();

    // ACT
    var value1 = noise.Raw(1, 2, 3, 4, 5, 6, 7, 8, 9);
    var value2 = noise.Raw(42, 69, 1337, 0xBEEF, 0xDEAD, 0xF00D, 0xC0FFEE, int.MinValue, int.MaxValue);

    // ASSERT
    Assert.Multiple(() =>
    {
      value1.Should().Be(14355685760848589803);
      value2.Should().Be(1850073071597428768);
    });
  }
}