// See https://aka.ms/new-console-template for more information

using System.Drawing.Imaging;
using IronClad.Noise;
using IronClad.Noise.Analysis;


uint[] seeds = [0, 0xC0FFEE, 0xBEEF, 0xBAD_C0DE_FU];

foreach (var seed in seeds)
{
  for (var x = 0; x < 8; x++)
  {
    for (var y = x + 1; y < 9; y++)
    {
      var noise = new Noise4Byte(seed);
      var function = NoiseFunctionGeneration.GenerateFunction(noise, x, y);
      var bitmap = RenderUtilities.Visualize(function);
      bitmap.Save($"../../../output/{seed}-{x}-{y}.png", ImageFormat.Png);
    }
  }
}
