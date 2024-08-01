namespace Ironclad.RandomNumbers.Analysis;

internal sealed class ScatterTest4Byte : ScatterTest<float>
{
  private readonly RandomNumberGenerator rng;
  protected override string[] ParameterDescriptions { get; set; }
  
  public ScatterTest4Byte(uint seed)
  {
    rng = new RandomNumberGenerator(seed);
    ParameterDescriptions = [
      "4 Byte Random Number Generator",
      $"seed {seed}",
      "10,080 samples; 5,040 scatter plot points",
    ];
  }
  protected override float GetNextSample() => rng.NextFloat();
}