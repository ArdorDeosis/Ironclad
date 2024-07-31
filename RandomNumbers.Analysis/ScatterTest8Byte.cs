namespace Ironclad.RandomNumbers.Analysis;

internal sealed class ScatterTest8Byte : ScatterTest<double>
{
  private readonly RandomNumberGenerator8Byte rng;
  protected override string[] ParameterDescriptions { get; set; }
  
  public ScatterTest8Byte(ulong seed)
  {
    rng = new RandomNumberGenerator8Byte(seed);
    ParameterDescriptions = [
      "8 Byte Random Number Generator",
      $"seed {seed}",
      "10,080 samples; 5,040 scatter plot points",
    ];
  }
  protected override double GetNextSample() => rng.NextDouble();
}