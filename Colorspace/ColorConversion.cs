using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Colorspace
{
  public static class ColorConversion
  {
    const double E = 216.0 / 24389;
    const double K = 24389.0 / 27;
    const double KE = 216.0 / 27;

    static readonly XYZ LAB_DEFAULT_WP = XYZ.D50_Whitepoint;

    /// <summary>
    /// Converts xyY to RGB
    /// </summary>
    /// <param name="c">the color</param>
    /// <param name="clip">true if clipping should be applied</param>
    /// <returns>the converted color</returns>
    public static RGB ToRGB(this xyY c, bool clip = false)
    {
      return c.ToXYZ().ToRGB(clip);
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

    internal static Lab ToUCS(this XYZ c)
    {
      var d = c.X + 15 * c.Y + 3 * c.Z;
      return new Lab
      {
        L = c.Y,
        a = (4 * c.X) / d,
        b = (6 * c.Y) / d
      };
	  }
    
    /// <summary>
    /// Normalizes Lab
    /// </summary>
    /// <param name="c">the color</param>
    /// <returns>the normalized color</returns>
#if DEBUG
    public static Lab Normalize(this Lab c, [CallerMemberName] string caller = "nowhere")
#else
    public static Lab Normalize(this Lab c)
#endif
    {
#if DEBUG
      Debug.Print("normalizing: {0} called from: {1}", c, caller);
#endif
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

    /// <summary>
    /// Converts XYZ to RGB
    /// </summary>
    /// <param name="c">the color</param>
    /// <param name="clip">true if clipping to the output should be applied</param>
    /// <returns>the converted color</returns>
    public static RGB ToRGB(this XYZ c, bool clip = false)
    {
      http://www.brucelindbloom.com/Eqn_XYZ_to_RGB.html

      if (c.Equals(default(XYZ))) // completely black
      {
        return new RGB();
      }

      var M = new double[,]
      {
        {3.2404542, -1.5371385, -0.4985314},
        {-0.9692660, 1.8760108, 0.0415560},
        {0.0556434, -0.2040259, 1.0572252}
      };

      RGB rgbs = (Vector3)c * M; 

      rgbs = sRGB.Compand(rgbs);

      if (clip)
      {
        rgbs = Clip(rgbs);
      }

      return rgbs;
    }

    /// <summary>
    /// Convertes RGB to XYZ
    /// </summary>
    /// <param name="c">the color</param>
    /// <returns>the converted color</returns>
    public static XYZ ToXYZ(this RGB c)
    {
      http://www.brucelindbloom.com/Eqn_RGB_to_XYZ.html

      if (c.Equals(default(RGB)))
      {
        return new XYZ();
      }

      var M = new double[,]
      {
        {0.4124564,  0.3575761,  0.1804375},
        {0.2126729,  0.7151522,  0.0721750},
        {0.0193339,  0.1191920,  0.9503041}
      };

      c = sRGB.InverseCompand(c);

      XYZ xyz = (Vector3)c * M;

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
#if DEBUG
    public static XYZ Normalize(this XYZ c, [CallerMemberName] string caller = "")
#else
    public static XYZ Normalize(this XYZ c)
#endif
    {
#if DEBUG // completely needless
      Debug.Print("normalizing: {0} called from: {1}", c, caller);
#endif 
      return new XYZ
      {
        X = c.X / c.Y,
        Y = 1,
        Z = c.Z / c.Y,
      };
    }
  }
}