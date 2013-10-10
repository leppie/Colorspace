using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cureos.Numerics;

namespace Colorspace
{
  public class Observer
  {
    public string Name;
    public int start, end;
    public double[] x, y, z;

    public int Interval
    {
      get { return (end - start) / (x.Length - 1);}
    }

    public int Length
    {
      get { return (end - start)/Interval + 1; }
    }

    public XYZ this[int i]
    {
      get
      {
        var ri = (i - start)/Interval;
        return new XYZ
        {
          X = x[ri],
          Y = y[ri],
          Z = z[ri],
        };
      }
    }
  }

  public class Illuminant
  {
    public string Name;
    public int start, end;
    public double[] s;
    
    public int Interval
    {
      get { return (end - start) / (s.Length - 1); }
    }

    public int Length
    {
      get { return (end - start) / Interval + 1; }
    }

    public double this[int i]
    {
      get
      {
        return s[(i - start) / Interval];
      }
    }

  }

  public enum Locus
  {
    Daylight,
    Planckian
  }

  public  static partial class Spectral
  {
    static readonly double[] s0_1nm = InterpolateLinear(s0_5nm, 4, x => Math.Round(x, 5));
    static readonly double[] s1_1nm = InterpolateLinear(s1_5nm, 4, x => Math.Round(x, 5));
    static readonly double[] s2_1nm = InterpolateLinear(s2_5nm, 4, x => Math.Round(x, 5));

    public static readonly Illuminant D65_1nm = new Illuminant
    {
      Name = "D65",
      start = 300,
      end = 830,
      s = InterpolateLinear(d65_10nm, 9),
    };

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
    
    public static readonly Observer DaylightLocus = CalculateDaylightLocus(CIE1931_2deg_1nm);
    public static readonly Observer PlanckianLocus = CalculatePlanckianLocus(CIE1931_2deg_1nm);

    static readonly Observer[] locii = 
    { 
      DaylightLocus, 
      PlanckianLocus 
    };

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

    public static double ToClosestDaylightColorTemperature(this XYZ xyz)
    {
      return ToClosestColorTemperature(xyz, Locus.Daylight);
    }

    public static double ToClosestPlanckianColorTemperature(this XYZ xyz)
    {
      return ToClosestColorTemperature(xyz, Locus.Planckian);
    }
    
    public static double ToClosestColorTemperature(this XYZ xyz, Locus loc = Locus.Daylight, DeltaE calc = ColorDifference.CALC_DEFAULT)
    {
      double de;
      return ToClosestColorTemperature(xyz, out de, loc, calc);
    }

    public static double ToClosestColorTemperature(this XYZ xyz, out double de, Locus loc = Locus.Daylight, DeltaE calc = ColorDifference.CALC_DEFAULT)
    {
      XYZ locuswp;
      return ToClosestColorTemperature(xyz, out de, out locuswp, loc, calc);
    }

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

    public static XYZ SpectrumValue(this Observer ob, double wl)
    {
      var n = (ob.end - ob.start) / ob.Interval;
      var f = (wl - ob.start) / ob.Interval;
      int i = (int) Math.Floor(f);

      if (i < 0) i = 0;
      if (i > n - 1) i = n - 1;

      var w = f - i;

      return new XYZ
      {
        X = (1 - w) * ob.x[i] + w * ob.x[i + 1],
        Y = (1 - w) * ob.y[i] + w * ob.y[i + 1],
        Z = (1 - w) * ob.z[i] + w * ob.z[i + 1],
      };
    }

    public static XYZ CalculateWhitePointD65()
    {
      return CalculateWhitePoint(D65_1nm, CIE1931_2deg_1nm);
    }

    public static XYZ CalculateWhitePoint(Illuminant il)
    {
      return CalculateWhitePoint(il, CIE1931_2deg_1nm);
    }

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

    public static Illuminant Daylight(double ct)
    {
      var il = ct.ToIlumninant();
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
  }
}

