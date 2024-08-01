using System.Drawing;

namespace IronClad.Noise.Analysis;

internal static class RenderUtilities
{
  public static Bitmap Visualize(Func<int, int, float> pixelValueFunc) => 
    Visualize(pixelValueFunc, 1024, 1024);
  
  public static Bitmap Visualize(Func<int, int, float> pixelValueFunc, int width, int height)
  {
    var bitmap = new Bitmap(width, height);

    for (var x = 0; x < width; x++)
    {
      for (var y = 0; y < height; y++)
      {
        var grayValue = (int)(pixelValueFunc(x, y) * 255);
        var color = Color.FromArgb(grayValue, grayValue, grayValue);
        bitmap.SetPixel(x, y, color);
      }
    }

    return bitmap;
  }
}