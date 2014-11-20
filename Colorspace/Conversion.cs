using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Colorspace
{
  public static class Conversion
  {
    const double E = 216.0 / 24389;
    const double K = 24389.0 / 27;
    const double KE = 216.0 / 27;

    static readonly XYZ LAB_DEFAULT_WP = XYZ.D50_Whitepoint;

    /// <summary>
    /// Converts xyY to sRGB
    /// </summary>
    /// <param name="c">the color</param>
    /// <param name="clip">true if clipping should be applied</param>
    /// <returns>the converted color</returns>
    public static RGB TosRGB(this xyY c, bool clip = false)
    {
      return c.ToXYZ().TosRGB(clip);
    }

    /// <summary>
    /// Converts RGB to xyY
    /// </summary>
    /// <param name="c">the color</param>
    /// <returns>the converted color</returns>
    public static xyY ToxyY(this RGB c)
    {
      return c.ToXYZ().ToxyY();
    }
    
    /// <summary>
    /// Normalizes Lab
    /// </summary>
    /// <param name="c">the color</param>
    /// <returns>the normalized color</returns>
    public static Lab Normalize(this Lab c)
    {
      return new Lab
      {
        L = 100,
        a = c.a * 100 / c.L,
        b = c.b * 100 / c.L
      };
    }

    /// <summary>
    /// Converts XYZ to Lab using D50 whitepoint
    /// </summary>
    /// <param name="c">the color</param>
    /// <returns>the converted color</returns>
    public static Lab ToLab(this XYZ c)
    {
      return ToLab(c, LAB_DEFAULT_WP);
    }

    static double ToLabHelper(double x)
    {
      return x > E ? Math.Pow(x, 1/3.0) : (K*x + 16) / 116;
    }

    /// <summary>
    /// Converts Lab to XYZ using D50 whitepoint
    /// </summary>
    /// <param name="c">the color</param>
    /// <returns>the converted color</returns>
    public static XYZ ToXYZ(this Lab c)
    {
      return ToXYZ(c, LAB_DEFAULT_WP);
    }

    /// <summary>
    /// Converts Lab to XYZ using a given whitepoint
    /// </summary>
    /// <param name="c">the color</param>
    /// <param name="wp">the whitepoint</param>
    /// <returns>the converted color</returns>
    public static XYZ ToXYZ(this Lab c, XYZ wp)
    {
      http://www.brucelindbloom.com/Eqn_Lab_to_XYZ.html

      double fy = (c.L + 16) / 116;
      double fz = fy - c.b / 200;
      double fx = c.a / 500 + fy;

      double fx3 = Math.Pow(fx, 3);
      double fz3 = Math.Pow(fz, 3);

      double x = fx3 > E ? fx3 : (116 * fx - 16) / K;
      double z = fz3 > E ? fz3 : (116 * fz - 16) / K;
      double y = c.L > KE ? Math.Pow((c.L + 16) / 116, 3) : c.L / K;

      return new XYZ
      {
        X = x * wp.X,
        Y = y * wp.Y,
        Z = z * wp.Z,
      };
    }

    /// <summary>
    /// Converts XYZ to Lab using a given whitepoint
    /// </summary>
    /// <param name="C">the color</param>
    /// <param name="wp">the whitepoint</param>
    /// <returns>the converted color</returns>
    public static Lab ToLab(this XYZ C, XYZ wp)
    {
      http://www.brucelindbloom.com/Eqn_XYZ_to_Lab.html

      var c = new XYZ
      {
        X = C.X/wp.X,
        Y = C.Y/wp.Y,
        Z = C.Z/wp.Z
      };

      double fx = ToLabHelper(c.X);
      double fy = ToLabHelper(c.Y);
      double fz = ToLabHelper(c.Z);

      return new Lab
      {
        L = 116 * fy - 16,
        a = 500 * (fx - fy),
        b = 200 * (fy - fz)
      };
    }

    /// <summary>
    /// Converts xyY to XYZ
    /// </summary>
    /// <param name="c">the color</param>
    /// <returns>the converted color</returns>
    public static XYZ ToXYZ(this xyY c)
    {
      if (c.Y == 0)
      {
        return new XYZ();
      }
      return new XYZ
      {
        Y = c.Y,
        X = c.x * c.Y / c.y,
        Z = (1 - c.x - c.y) * c.Y / c.y
      };
    }

    /// <summary>
    /// Converts XYZ to xyY
    /// </summary>
    /// <param name="c">the color</param>
    /// <returns>the converted color</returns>
    public static xyY ToxyY(this XYZ c)
    {
      var d = c.X + c.Y + c.Z;
      if (d < 1e-9)
      {
        return new xyY();
      }
      return new xyY
      {
        x = c.X / d,
        y = c.Y / d,
        Y = c.Y
      };
    }

    public static RGB TosRGB(this XYZ c, bool clip = false)
    {
      return c.TosRGB(XYZ.D65_Whitepoint, clip);
    }

    /// <summary>
    /// Converts XYZ to sRGB
    /// </summary>
    /// <param name="c">the color</param>
    /// <param name="clip">true if clipping to the output should be applied</param>
    /// <returns>the converted color</returns>
    public static RGB TosRGB(this XYZ c, XYZ wp, bool clip = false)
    {
      http://www.brucelindbloom.com/Eqn_XYZ_to_RGB.html
      
      c = c.Normalize();

      if (c.Equals(default(XYZ))) // completely black
      {
        return new RGB();
      }

      var M = CreateMatrix(Coordinates.sRGB, wp, true);

      RGB rgbs = c * M; 

      rgbs = sRGB.Compand(rgbs);

      if (clip)
      {
        rgbs = Clip(rgbs);
      }

      return rgbs;
    }

    static Matrix3x3 CreateMatrix(Coordinates c, XYZ wp, bool invert)
    {
      var r = c.R.ToXYZ();
      var g = c.G.ToXYZ();
      var b = c.B.ToXYZ();

      Matrix3x3 M = new double[,]
      {
        {r.X, g.X, b.X },
        {r.Y, g.Y, b.Y },
        {r.Z, g.Z, b.Z },
      };

      M = M.Invert();

      RGB S = M * wp;

      Matrix3x3 result = new double[,]
      {
        {S.R * r.X, S.G * g.X, S.B * b.X},
        {S.R * r.Y, S.G * g.Y, S.B * b.Y},
        {S.R * r.Z, S.G * g.Z, S.B * b.Z},
      };

      if (invert)
      {
        result = result.Invert();
      }

      return result;
    }

    public static XYZ ToXYZ(this RGB c)
    {
      return c.ToXYZ(XYZ.D65_Whitepoint);
    }

    /// <summary>
    /// Convertes sRGB to XYZ
    /// </summary>
    /// <param name="c">the color</param>
    /// <param name="wp">the white point</param>
    /// <returns>the converted color</returns>
    public static XYZ ToXYZ(this RGB c, XYZ wp)
    {
      http://www.brucelindbloom.com/Eqn_RGB_to_XYZ.html

      if (c.Equals(default(RGB)))
      {
        return new XYZ();
      }

      var M = CreateMatrix(Coordinates.sRGB, wp, false);

      c = sRGB.InverseCompand(c);

      XYZ xyz = c * M;

      return xyz;
    }

    
    static double Clip(double d)
    {
      return Math.Max(0, Math.Min(1, d));
    }

    static RGB Clip(RGB c)
    {
      return new RGB
      {
        R = Clip(c.R),
        G = Clip(c.G),
        B = Clip(c.B)
      };
    }

    /// <summary>
    /// Normalizes XYZ
    /// </summary>
    /// <param name="c">the color</param>
    /// <returns>the normalized color</returns>
    public static XYZ Normalize(this XYZ c)
    {
      return new XYZ
      {
        X = c.X / c.Y,
        Y = 1,
        Z = c.Z / c.Y,
      };
    }
  }
}