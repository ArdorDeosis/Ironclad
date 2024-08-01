using System.Drawing;

namespace Ironclad.Noise.Analysis;

internal static class RenderUtilities
{
  /// <summary>
  /// Creates a 1024x1024 bitmap with color values defined by the specified value function.
  /// </summary>
  public static Bitmap Visualize(Func<int, int, float> valueFunction) => 
    Visualize(valueFunction, 1024, 1024);
  
  /// <summary>
  /// Creates a bitmap of specified size with color values defined by the specified value function.
  /// </summary>
  public static Bitmap Visualize(Func<int, int, float> valueFunction, int width, int height)
  {
    var bitmap = new Bitmap(width, height);

    for (var x = 0; x < width; x++)
    {
      for (var y = 0; y < height; y++)
      {
        var grayValue = (int)(valueFunction(x, y) * 255);
        var color = Color.FromArgb(grayValue, grayValue, grayValue);
        bitmap.SetPixel(x, y, color);
      }
    }

    return bitmap;
  }
}