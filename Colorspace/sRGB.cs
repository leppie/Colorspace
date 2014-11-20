using System;

namespace Colorspace
{
  public static class sRGB
  {
    public static RGB Compand(RGB c)
    {
      return (Vector3)c * Compand;
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
      return (Vector3)c * InverseCompand;
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
