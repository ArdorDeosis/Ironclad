using System.Linq.Expressions;

namespace IronClad.Noise.Analysis;

internal static class NoiseFunctionGeneration
{
  /// <summary>
  /// Dynamically generates a lambda <c>(x, y) => noise[..., x, ..., y, ...]</c> where x and y are the positions
  /// specified in <paramref name="dx"/> and <paramref name="dy"/>.
  /// <para>
  /// Example: <c>GenerateFunction(myNoise, 2, 6)</c> will result in lambda
  /// <c>(x, y) => myNoise[0, 0, x, 0, 0, 0, y, 0, 0]</c>.
  /// </para> 
  /// </summary>
  public static Func<int, int, float> GenerateFunction(Noise4Byte noise, int dx, int dy)
  {
    var noiseIndexProperty = noise.GetType().GetProperty("Item", Enumerable.Repeat(typeof(int), 9).ToArray());

    var xParam = Expression.Parameter(typeof(int), "x");
    var yParam = Expression.Parameter(typeof(int), "y");

    var noiseParams = new Expression[9];
    for (var i = 0; i < 9; i++)
    {
      if (i == dx)
        noiseParams[i] = xParam;
      else if (i == dy)
        noiseParams[i] = yParam;
      else
        noiseParams[i] = Expression.Constant(0);
    }

    var noiseAccess = Expression.MakeIndex(
      Expression.Constant(noise),
      noiseIndexProperty,
      noiseParams);

    var lambda = Expression.Lambda<Func<int, int, float>>(noiseAccess, xParam, yParam);

    return lambda.Compile();
  }
}