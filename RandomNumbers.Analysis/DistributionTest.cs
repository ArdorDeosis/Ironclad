using System.Numerics;

namespace Ironclad.RandomNumbers.Analysis;


internal abstract class DistributionTest<TValue> where TValue : struct, INumber<TValue>, IConvertible
{
  protected abstract string[] ParameterDescriptions { get; }
  protected abstract int BucketCount { get; }
  protected abstract int SampleCount { get; }
  
  public HistogramData Execute()
  {
    var buckets = new int[BucketCount];
    var samples = new TValue[SampleCount];
    for (var i = 0; i < SampleCount; i++)
    {
      samples[i] = GetNextSample();
      buckets[GetBucketNumber(samples[i])]++;
    }
    return new HistogramData
    {
      ParameterDescriptions = ParameterDescriptions,
      BucketValues = buckets,
      ChiSquare = CalculateChiSquareValue(buckets),
      KolmogorovSmirnov = CalculateKolmogorovSmirnovValue(samples),
    };
  }
  
  /// <returns> A sample in [0,1) </returns>
  protected abstract TValue GetNextSample();

  /// <remarks> Assumes a sample in [0,1) </remarks>
  private int GetBucketNumber(TValue value) => 
    (int)(BucketCount * Convert.ToDouble(value));

  private double CalculateChiSquareValue(int[] buckets)
  {
    var expectedFrequency = (double)SampleCount / BucketCount;
    var chiSquare = 0.0;

    foreach (var observedFrequency in buckets)
    {
      var difference = observedFrequency - expectedFrequency;
      chiSquare += (difference * difference) / expectedFrequency;
    }

    return chiSquare;
  }
  
  private double CalculateKolmogorovSmirnovValue(TValue[] samples)
  {
    var sortedSamples = samples.OrderBy(x => x).ToArray();

    var maxDifference = 0.0;

    for (var i = 0; i < SampleCount; i++)
    {
      var actualValue = Convert.ToDouble(sortedSamples[i]); // For uniform distribution on [0,1], CDF is the value itself
      var expectedValue = (i + 1) / (double)SampleCount;

      var difference = Math.Abs(expectedValue - actualValue);

      if (difference > maxDifference) 
        maxDifference = difference;
    }

    return maxDifference;
  }
}