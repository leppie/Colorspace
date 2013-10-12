using Cureos.Numerics;
using System;

namespace Colorspace
{
  /// <summary>
  /// Represents an observer
  /// </summary>
  public class Observer
  {
    internal string Name;
    internal int start, end;
    internal double[] x, y, z;

    int Interval
    {
      get { return (end - start) / (x.Length - 1);}
    }

    public override string ToString()
    {
      return Name;
    }
  }

  /// <summary>
  /// Represents an illuminant
  /// </summary>
  public class Illuminant
  {
    internal string Name;
    internal int start, end;
    internal double[] s;

    int Interval
    {
      get { return (end - start) / (s.Length - 1); }
    }

    public override string ToString()
    {
      return Name;
    }
  }

  /// <summary>
  /// The whitepoint locus
  /// </summary>
  public enum Locus
  {
    Daylight,
    Planckian
  }

  public  static partial class Spectral
  {
    static readonly double[] s0_1nm = InterpolateLinear(s0_5nm, 4);
    static readonly double[] s1_1nm = InterpolateLinear(s1_5nm, 4);
    static readonly double[] s2_1nm = InterpolateLinear(s2_5nm, 4);

    /// <summary>
    /// The D65 illuminant at 1nm intervals
    /// </summary>
    public static readonly Illuminant D65_1nm = new Illuminant
    {
      Name = "D65",
      start = 300,
      end = 830,
      s = InterpolateLinear(d65_10nm, 9),
    };

    /// <summary>
    /// The CIE1931_2deg observer at 1nm intervals
    /// </summary>
    public static readonly Observer CIE1931_2deg_1nm = new Observer
    {
      Name = "CIE 1931 2deg",
      start = 360,
      end = 830,
      x = cie1931_2deg_1nm_x,
      y = cie1931_2deg_1nm_y,
      z = cie1931_2deg_1nm_z,
    };

    static double[] InterpolateLinear(double[] a, int fill, Func<double, double> transform = null)
    {
      int l = (fill + 1);
      int rl = l * (a.Length - 1) + 1;
      double[] r = new double[rl];
      
      int ai = 0;

      for (; ai < a.Length - 1; ai++)
      {
        double start = a[ai], end = a[ai + 1];

        r[ai * l] = start;

        for (int i = 1; i < l; i++)
        {
          var result = (end - start) * i / l + start;
          if (transform != null)
          {
            result = transform(result);
          }
          r[ai * l + i] = result;
        }
      }

      r[rl - 1] = a[ai];

      return r;
    }
    
    /// <summary>
    /// Gives the daylight locus with a CIE1931_2deg observer at 10K intervals.
    /// </summary>
    public static readonly Observer DaylightLocus = CalculateDaylightLocus(CIE1931_2deg_1nm);

    /// <summary>
    /// Gives the planckian locus with a CIE1931_2deg observer at 10K intervals.
    /// </summary>
    public static readonly Observer PlanckianLocus = CalculatePlanckianLocus(CIE1931_2deg_1nm);

    static Observer CalculateDaylightLocus(Observer ob, int interval = 10, int start = 4000, int end = 10000)
    {
      int size = (end - start) / interval + 1;

      double[] x = new double[size];
      double[] y = new double[size];
      double[] z = new double[size];

      var locus = new Observer
      {
        Name = "Daylight locus",
        start = start,
        end = end,
        x = x,
        y = y,
        z = z
      };

      for (int i = 0; i < size; i++)
      {
        var m = (i * interval) + locus.start;

        var dwp = CalculateWhitePoint(Daylight(m), ob);

        x[i] = dwp.X;
        y[i] = dwp.Y;
        z[i] = dwp.Z;
      }

      return locus;
    }

    static Observer CalculatePlanckianLocus(Observer ob, int interval = 10, int start = 2000, int end = 10000)
    {
      int size = (end - start) / interval + 1;

      double[] x = new double[size];
      double[] y = new double[size];
      double[] z = new double[size];

      var locus = new Observer
      {
        Name = "Planckian locus",
        start = start,
        end = end,
        x = x,
        y = y,
        z = z
      };

      for (int i = 0; i < size; i++)
      {
        var m = (i * interval) + locus.start;

        var dwp = CalculateWhitePoint(Planckian(m), ob);

        x[i] = dwp.X;
        y[i] = dwp.Y;
        z[i] = dwp.Z;
      }

      return locus;
    }

    /// <summary>
    /// Gets the closest visual daylight color temperature to a color
    /// </summary>
    /// <param name="xyz">The color in XYZ</param>
    /// <returns>the color temperature</returns>
    public static double ToClosestDaylightColorTemperature(this XYZ xyz)
    {
      return ToClosestColorTemperature(xyz, Locus.Daylight);
    }

    /// <summary>
    /// Gets the closest visual planckian color temperature to a color
    /// </summary>
    /// <param name="xyz">The color in XYZ</param>
    /// <returns>the color temperature</returns>
    public static double ToClosestPlanckianColorTemperature(this XYZ xyz)
    {
      return ToClosestColorTemperature(xyz, Locus.Planckian);
    }

    /// <summary>
    /// Gets the closest color temperature to a color
    /// </summary>
    /// <param name="xyz">The color in XYZ</param>
    /// <param name="loc">The locus, either daylight or planckian</param>
    /// <param name="calc">The color difference calculation to use, either CIE2000 or else it is based on UCS1960 using CIE1976</param>
    /// <returns>the color temperature</returns>
    public static double ToClosestColorTemperature(this XYZ xyz, Locus loc = Locus.Daylight, DeltaE calc = ColorDifference.CALC_DEFAULT)
    {
      double de;
      return ToClosestColorTemperature(xyz, out de, loc, calc);
    }

    /// <summary>
    /// Gets the closest color temperature to a color
    /// </summary>
    /// <param name="xyz">The color in XYZ</param>
    /// <param name="de">returns the Delta E error</param>
    /// <param name="loc">The locus, either daylight or planckian</param>
    /// <param name="calc">The color difference calculation to use, either CIE2000 or else it is based on UCS1960 using CIE1976</param>
    /// <returns>the color temperature</returns>
    public static double ToClosestColorTemperature(this XYZ xyz, out double de, Locus loc = Locus.Daylight, DeltaE calc = ColorDifference.CALC_DEFAULT)
    {
      XYZ locuswp;
      return ToClosestColorTemperature(xyz, out de, out locuswp, loc, calc);
    }

    /// <summary>
    /// Gets the closest color temperature to a color
    /// </summary>
    /// <param name="xyz">The color in XYZ</param>
    /// <param name="de">returns the Delta E error</param>
    /// <param name="locuswp">return the white point of the closest color temperature</param>
    /// <param name="loc">The locus, either daylight or planckian</param>
    /// <param name="calc">The color difference calculation to use, either CIE2000 or else it is based on UCS1960 using CIE1976</param>
    /// <returns>the color temperature</returns>
    public static double ToClosestColorTemperature(this XYZ xyz, out double de, out XYZ locuswp, Locus loc = Locus.Daylight, DeltaE calc = ColorDifference.CALC_DEFAULT)
    {
      xyz = xyz.Normalize();

      double ber = 1e9;

      double[] x = { 7000, 0 }; // arb
      
      XYZ guess = new XYZ();

      var wp = xyz;

      Bobyqa.FindMinimum((i, d) =>
      {
        var tt = d[0];
        var v = CalculateWhitePoint(loc == Locus.Daylight ? Daylight(tt) : Planckian(tt));
        var err = calc == DeltaE.CIE2000 ? xyz.Difference(v, wp, calc) : xyz.ToUCS().Difference(v.ToUCS(), calc);
        if (err < ber)
        {
          ber = err;
          guess = v;
        }
        return err;
      },
        2,
        x
        ,rhobeg: 500
        ,rhoend: 0.01
        );

      de = ber;
      locuswp = guess;

      return x[0];
    }

    [Obsolete("should be private/internal")]
    public static XYZ CalculateWhitePointD65()
    {
      return CalculateWhitePoint(D65_1nm, CIE1931_2deg_1nm);
    }

    /// <summary>
    /// Calculates the whitepoint of an illuminant with CIE1931 2 deg observer.
    /// </summary>
    /// <param name="il">the illuminant</param>
    /// <returns>the whitepoint</returns>
    public static XYZ CalculateWhitePoint(Illuminant il)
    {
      return CalculateWhitePoint(il, CIE1931_2deg_1nm);
    }

    /// <summary>
    /// Calculates the whitepoint of an illuminant with an observer.
    /// </summary>
    /// <param name="il">the illuminant</param>
    /// <param name="ob">the observer</param>    
    /// <returns>the whitepoint</returns>
    public static XYZ CalculateWhitePoint(Illuminant il, Observer ob)
    {
      double x = 0, y = 0, z = 0;

      int ilstart = il.start, obstart = ob.start;

      int start = Math.Max(ilstart, obstart);
      int end = Math.Max(il.end, ob.end);
      int length = end - start + 1;

      for (int i = 0; i < length; i++)
      {
        var s = il.s[start - ilstart + i];
        var ox = ob.x[start - obstart + i];
        var oy = ob.y[start - obstart + i];
        var oz = ob.z[start - obstart + i];

        x += s * ox;
        y += s * oy;
        z += s * oz;
      }

      var xyz = new XYZ { X = x, Y = y, Z = z };

      return xyz.Normalize();
    }

    /// <summary>
    /// Gets the daylight illuminant for a given temperature at 1nm intervals
    /// </summary>
    /// <param name="ct">color temperature</param>
    /// <returns>the illuminant</returns>
    public static Illuminant Daylight(double ct)
    {
#pragma warning disable 618
      var il = ct.ToIluminant();
#pragma warning restore 618
      var sl = s0_1nm.Length;
      
      double xd = il.x;
      double yd = il.y;

      double m = 0.0241 + 0.2562 * xd - 0.7341 * yd;

      double m1 = (-1.3515 - 1.7703 * xd + 5.9114 * yd) / m;
      double m2 = (0.03 - 31.4424 * xd + 30.0717 * yd) /m;

      double[] spec = new double[sl];

      /* Compute spectral values */
      for (int i = 0; i < sl; i++)
      {
        spec[i] = s0_1nm[i] + m1 * s1_1nm[i] + m2 * s2_1nm[i];
      }

      return new Illuminant { Name = string.Format("D({0:f1}K)", ct), s = spec, start = 300, end = 830 };
    }

    /// <summary>
    /// Gets the plankian illuminant for a given temperature at 1nm intervals
    /// </summary>
    /// <param name="ct">color temperature</param>
    /// <returns>the illuminant</returns>
    public static Illuminant Planckian(double ct)
    {
      if (ct < 1.0 || ct > 1e6)	/* set some arbitrary limits */
        return null;

      const int len = 531;

      double[] s = new double[len];

      var il = new Illuminant 
      {
        Name = string.Format("P({0:f1}K)", ct) ,
        start = 300,
        end = 830,
        s = s
      };
      
      /* Compute spectral values using Plank's radiation law: */
      /* Normalise numbers by energy at 560 nm */
      double wl = 1e-9 * 560;
      double norm = 0.01 * (3.74183e-16 * Math.Pow(wl, -5.0)) / (Math.Exp(1.4388e-2 / (wl * ct)) - 1.0);

      for (int i = 0; i <len; i++)
      {
        wl = 1e-9 * (300 + i);			/* Wavelength in meters */
        s[i] = (3.74183e-16 * Math.Pow(wl, -5.0)) / (Math.Exp(1.4388e-2 / (wl * ct)) - 1.0);
        s[i] /= norm;
      }

      return il;
    }

    /// <summary>
    /// Calculates the closest correlated color temperature to a color
    /// </summary>
    /// <param name="c">the color</param>
    /// <returns>the color temperature</returns>
    public static double ToClosestCorrelatedColorTemperature(this XYZ c)
    {
      return c.ToClosestColorTemperature(Locus.Planckian, DeltaE.CIE1976);
    }

    /// <summary>
    /// Calculates the closest correlated color temperature to a color
    /// </summary>
    /// <param name="c">the color</param>
    /// <returns>the color temperature</returns>
    public static double ToClosestCorrelatedColorTemperature(this xyY c)
    {
      return c.ToXYZ().ToClosestCorrelatedColorTemperature();
    }

    [Obsolete("Should be private/internal")]
    public static xyY ToIluminant(this double cct)
    {
      http://www.brucelindbloom.com/Eqn_T_to_xy.html

      double cct2 = cct * cct;
      double cct3 = cct2 * cct;

      double x;

      if (cct > 7000)
      {
        x = -2.0064e9 / cct3 + 1.9018e6 / cct2 + 0.24748e3 / cct + 0.237040;
      }
      else
      {
        // wont work below 4000k, but all we got \o/
        x = -4.6070e9 / cct3 + 2.9678e6 / cct2 + 0.09911e3 / cct + 0.244063;
      }

      double x2 = x * x;
      double y = -3 * x2 + 2.87 * x - 0.275;

      return xyY.FromWhitePoint(x, y);
    }
  }
}

