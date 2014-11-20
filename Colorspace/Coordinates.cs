
namespace Colorspace
{
  public class Coordinates
  {
    Coordinates() { }

    public xyY R { get; private set; }
    public xyY G { get; private set; }
    public xyY B { get; private set; }
    public xyY Whitepoint { get; private set; }

    public static readonly Coordinates sRGB = new Coordinates
    {
      R = new xyY { x = 0.6400, y = 0.3300, Y = 0.2126 },
      G = new xyY { x = 0.3000, y = 0.6000, Y = 0.7153 },
      B = new xyY { x = 0.1500, y = 0.0600, Y = 0.0721 },
      Whitepoint = xyY.D65_WhitePoint
    };
  }
}
