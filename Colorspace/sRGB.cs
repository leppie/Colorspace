using System;

namespace Colorspace
{
  public static class sRGB
  {
    public static readonly xyY Red = new xyY { x = 0.6400, y = 0.3300, Y = 0.2126 };
    public static readonly xyY Green = new xyY { x = 0.3000, y = 0.6000, Y = 0.7153 };
    public static readonly xyY Blue = new xyY { x = 0.1500, y = 0.0600, Y = 0.0721 };

    public static readonly xyY WhitePoint = XYZ.D65_Whitepoint.ToxyY();

    public static RGB Compand(RGB c)
    {
      return new RGB
      {
        R = Compand(c.R),
        G = Compand(c.G),
        B = Compand(c.B)
      };
    }

    static double Compand(double c)
    {
      if (c <= 0.0031308)
      {
        return c * 12.92;
      }
      return 1.055 * Math.Pow(c, 1 / 2.4) - 0.055;
    }

    public static RGB InverseCompand(RGB c)
    {
      return new RGB
      {
        R = InverseCompand(c.R),
        G = InverseCompand(c.G),
        B = InverseCompand(c.B)
      };
    }

    static double InverseCompand(double c)
    {
      if (c <= 0.04045)
      {
        return c / 12.92;
      }
      return Math.Pow((c + 0.055) / 1.055, 2.4);
    }
  }
}
