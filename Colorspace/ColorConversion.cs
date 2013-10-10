using System;

namespace Colorspace
{

  public static class ColorConversion
  {
    const double E = 216.0 / 24389;
    const double K = 24389.0 / 27;
    const double KE = 216.0 / 27;

    static readonly XYZ LAB_DEFAULT_WP = XYZ.D50;

    public static RGB ToRGB(this xyY c)
    {
      return c.ToXYZ().ToRGB();
    }

    public static xyY ToxyY(this RGB c)
    {
      return c.ToXYZ().ToxyY();
    }

    public static Lab ToUCS(this XYZ c)
    {
      var d = c.X + 15.0 * c.Y + 3.0 * c.Z;
      return new Lab
      {
        L = c.Y,
        a = (4.0 * c.X) / d,
        b = (6.0 * c.Y) / d
      };
	  }
    
    public static Lab Normalize(this Lab c)
    {
      return new Lab
      {
        L = 100,
        a = c.a * 100 / c.L,
        b = c.b * 100 / c.L
      };
    }

    public static Lab ToLab(this XYZ c)
    {
      return ToLab(c, LAB_DEFAULT_WP);
    }

    static double ToLabHelper(double x)
    {
      return x > E ? Math.Pow(x, 1/3.0) : (K*x + 16) / 116;
    }

    public static XYZ ToXYZ(this Lab c)
    {
      return ToXYZ(c, LAB_DEFAULT_WP);
    }

    public static XYZ ToXYZ(this Lab c, XYZ wp)
    {
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

    public static xyY ToxyY(this XYZ c)
    {
      var d = c.X + c.Y + c.Z;
      if (d < 1e-9)
      {
        return new xyY
        {
          x = 0,
          y = 0,
          Y = 0
        };
      }
      return new xyY
      {
        x = c.X / d,
        y = c.Y / d,
        Y = c.Y
      };
    }

    public static RGB ToRGB(this XYZ c)
    {
      http://www.brucelindbloom.com/Eqn_XYZ_to_RGB.html

      var M = new double[,]
      {
        {3.2404542, -1.5371385, -0.4985314},
        {-0.9692660, 1.8760108, 0.0415560},
        {0.0556434, -0.2040259, 1.0572252}
      };

      RGB rgbs = (Vector3)c * M; 

      // no clipping
      rgbs = sRGB.Compand(rgbs);

      return rgbs;
    }

    public static XYZ ToXYZ(this RGB c)
    {
      http://www.brucelindbloom.com/Eqn_RGB_to_XYZ.html

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