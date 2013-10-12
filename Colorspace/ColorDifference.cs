using System;

namespace Colorspace
{
  /// <summary>
  /// The Delta E calculation
  /// </summary>
  public enum DeltaE
  {
    CIE1976,
    CIE1994,
    CIE2000,
  }

  public static class ColorDifference
  {
    internal const DeltaE CALC_DEFAULT = DeltaE.CIE2000;

    const double 
      pow25_7 = 25.0 * 25 * 25 * 25 * 25 * 25 * 25, 
      K1 = 0.045, 
      K2 = 0.015,
      d2r = Math.PI / 180;

    const double
      deg180 = 180 * d2r,
      deg360 = 360 * d2r,
      deg275 = 275 * d2r,
      deg30 = 30 * d2r,
      deg6 = 6 * d2r,
      deg63 = 63 * d2r;

    static double Square(double d)
    {
      return d * d;
    }

    public static double DifferenceToWhitePoint(this XYZ c, DeltaE calc = CALC_DEFAULT)
    {
      return DifferenceToWhitePoint(c.ToLab());
    }

    public static double DifferenceToWhitePoint(this Lab c, DeltaE calc = CALC_DEFAULT)
    {
      // same as Difference(c, Lab.WhitePoint)
      switch (calc)
      {
        case DeltaE.CIE1976:
        case DeltaE.CIE1994: // same for the white point
          {
            http://www.brucelindbloom.com/Eqn_DeltaE_CIE76.html

            return Math.Sqrt(Square(-c.a) + Square(-c.b));
          }
        case DeltaE.CIE2000:
          {
            http://www.brucelindbloom.com/Eqn_DeltaE_CIE2000.html

            double a2 = c.a;
            double b2 = c.b;

            double Ct = Math.Sqrt(Square(a2) + Square(b2)) / 2;
            double Ct7 = Math.Pow(Ct, 7);

            double G = (1 - Math.Sqrt(Ct7 / (Ct7 + pow25_7))) / 2;
            double ad2 = a2 * (1 + G);

            double dCd = Math.Sqrt(Square(ad2) + Square(b2));
            double Ctd = dCd / 2;

            double Sc = 1 + K1 * Ctd;
            double SDE = Square(dCd / Sc);

            return Math.Sqrt(SDE);
          }
      }
      throw new ArgumentOutOfRangeException("calc", "Not a valid value for DeltaE");
    }

    public static double Difference(this XYZ c, XYZ r, DeltaE calc = CALC_DEFAULT)
    {
      return Difference(c.ToLab(), r.ToLab(), calc);
    }

    public static double Difference(this XYZ c, XYZ r, XYZ wp, DeltaE calc = CALC_DEFAULT)
    {
      return Difference(c.ToLab(wp), r.ToLab(wp), calc);
    }

    public static double Difference(this Lab c, Lab r, DeltaE calc = CALC_DEFAULT)
    {
      c = c.Normalize();
      r = r.Normalize();

      switch (calc)
      {
        case DeltaE.CIE1976:
        {
          http://www.brucelindbloom.com/Eqn_DeltaE_CIE76.html
          
          return Math.Sqrt(Square(r.L - c.L) + Square(r.a - c.a) + Square(r.b - c.b));
        }
        case DeltaE.CIE1994:
        {
          http://www.brucelindbloom.com/Eqn_DeltaE_CIE94.html

          double Da = r.a - c.a;
          double Db = r.b - c.b;
          double C1 = Math.Sqrt(Square(r.a) + Square(r.b));
          double C2 = Math.Sqrt(Square(c.a) + Square(c.b));
          double Sc = 1 + K1 * C1;
          double Sh = 1 + K2 * C1;
          double Dl = r.L - c.L;
          double Dc = C1 - C2;
          double Dh2 = Square(Da) + Square(Db) - Square(Dc);

          return Math.Sqrt(Square(Dl) + Square(Dc / Sc) + Dh2 / Square(Sh));
        }
        case DeltaE.CIE2000:
        {
          http://www.brucelindbloom.com/Eqn_DeltaE_CIE2000.html

          double L1 = r.L;
          double L2 = c.L;
          double a1 = r.a;
          double a2 = c.a;
          double b1 = r.b;
          double b2 = c.b;

          double Ltd = (L1 + L2) / 2;
          double C1 = Math.Sqrt(Square(a1) + Square(b1));
          double C2 = Math.Sqrt(Square(a2) + Square(b2));
          double Ct = (C1 + C2) / 2;

          double Ct7 = Math.Pow(Ct, 7);
          double G = (1 - Math.Sqrt(Ct7 / (Ct7 + pow25_7))) / 2;

          double ad1 = a1 * (1 + G);
          double ad2 = a2 * (1 + G);

          double Cd1 = Math.Sqrt(Square(ad1) + Square(b1));
          double Cd2 = Math.Sqrt(Square(ad2) + Square(b2));
          double Ctd = (Cd1 + Cd2) / 2;

          double atanhd1 = Math.Atan2(b1, ad1);
          double hd1 = atanhd1 + (atanhd1 >= 0 ? 0 : deg360);
          double atanhd2 = Math.Atan2(b2, ad2);
          double hd2 = atanhd2 + (atanhd2 >= 0 ? 0 : deg360);

          double sdh = hd1 + hd2;
          double dh1 = hd1 - hd2;
          double adh1 = Math.Abs(dh1);
          double Htd = (sdh + (adh1 > deg180 ? deg360 : 0)) / 2;

          double T = 1 - 0.17 * Math.Cos(Htd - deg30) + 0.24 * Math.Cos(2 * Htd) + 0.32 * Math.Cos(2 * Htd + deg6) - 0.20 * Math.Cos(4 * Htd - deg63);

          double dh2 = hd2 - hd1;
          double adh2 = Math.Abs(dh2);
          double dhd = adh2 <= deg180 ? dh2 : (hd2 <= hd1 ? (dh2 + deg360) : (dh2 - deg360));

          double dLd = L2 - L1;
          double dCd = Cd2 - Cd1;
          double dHd = 2 * Math.Sqrt(Cd1 * Cd2) * Math.Sin(dhd / 2);

          double Ltd2 = Square(Ltd - 50);
          double Sl = 1 + (K2 * Ltd2) / Math.Sqrt(20 + Ltd2);
          double Sc = 1 + K1 * Ctd;
          double Sh = 1 + K2 * Ctd * T;

          double ctd7 = Math.Pow(Ctd, 7);
          double dO = 30 * Math.Exp(- Square((Htd - deg275)/25));

          double Rc = 2 * Math.Sqrt(ctd7 / (ctd7 + pow25_7));
          double Rt = -Rc * Math.Sin(2 * dO);

          double SDE = Square(dLd / Sl) + Square(dCd / Sc) + Square(dHd / Sh) + Rt * (dCd / Sc) * (dHd / Sh);

          return Math.Sqrt(SDE);
        }
      }

      throw new ArgumentOutOfRangeException("calc", "Not a valid value for DeltaE");
    }
  }
}