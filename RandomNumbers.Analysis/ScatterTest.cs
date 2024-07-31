using System.Numerics;

namespace Ironclad.RandomNumbers.Analysis;

internal abstract class ScatterTest<TValue> where TValue : struct, INumber<TValue>, IConvertible
{
  protected abstract string[] ParameterDescriptions { get; set; }
  private const int SampleCount = 10_080;

  public ScatterData<TValue> Execute()
  {
    var samples = Enumerable.Range(0, SampleCount).Select(i => GetNextSample()).ToArray();
    return new ScatterData<TValue>(samples) { ParameterDescriptions = ParameterDescriptions };
  }

  /// <returns> A sample in [0,1) </returns>
  protected abstract TValue GetNextSample();
}