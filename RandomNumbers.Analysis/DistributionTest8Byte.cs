namespace Ironclad.RandomNumbers.Analysis;

internal sealed class DistributionTest8Byte : DistributionTest<double>
{
  private readonly RandomNumberGenerator8Byte rng;
  
  protected override string[] ParameterDescriptions { get; }

  protected override int BucketCount => 1000;
  protected override int SampleCount => 1_000_000;

  public DistributionTest8Byte(ulong seed)
  {
    rng = new RandomNumberGenerator8Byte(seed);
    ParameterDescriptions = [
      "8 Byte Random Number Generator",
      $"seed {seed}",
      "1,000,000 samples in 1,000 buckets",
    ];
  }
  
  protected override double GetNextSample() => rng.NextDouble();
}