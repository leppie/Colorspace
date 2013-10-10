using System;
using Colorspace;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
  [TestClass]
  public class ColorTests
  {

    [TestMethod]
    public void TestMethod1()
    {
      // not really a unit test, but a test runner..
      // I am not planning on testing floating point yet...

      //foreach (var mstr in Argyll.Spotread())
      //{
      //  Console.WriteLine(mstr);
      //}

      var wpr = Spectral.CalculateWhitePointD65();

      var awpr = new XYZ { X = 0.950470558654, Y = 1.000000000000, Z = 1.088828736396 };

      var diff = wpr.ScaleToD50().DifferenceToWhitePoint();
      var adiff = awpr.ScaleToD50().DifferenceToWhitePoint();


      var rrgb = awpr.ToRGB();

      var wpc = Spectral.CalculateWhitePoint(Spectral.Daylight(6502.9));
      var wpp = Spectral.CalculateWhitePoint(Spectral.Planckian(6502.9));

      var wpr2 = wpr.ToxyY();
      var wpc2 = wpc.ToxyY();

      var de2kwp = wpc.Difference(wpr);

      //var cr = xyY.FromWhitePoint(0.315314, 0.326862).ToXYZ();
      var cr = new XYZ { X = 135.656077255691, Y = 142.799826643091, Z = 155.244108611248 };
      //var cr = new XYZ { X = 52.445185084327, Y = 94.317863938736, Z = 57.157700415863 };
      //var cr = new XYZ { X = 60.547113, Y = 99.168354, Z = 100.586863};

      cr = wpr;

      var cct = cr.ToCorrelatedColorTemperature();
      var dct = cr.ToClosestDaylightColorTemperature();
      var pct = cr.ToClosestPlanckianColorTemperature();

      var twpp = Spectral.CalculateWhitePoint(Spectral.Planckian(pct));
      var twpc = Spectral.CalculateWhitePoint(Spectral.Daylight(dct));

      var pt = twpp.ToCorrelatedColorTemperature();
      var dt = twpc.ToCorrelatedColorTemperature();



      //6894K/6983K
      var c = xyY.FromWhitePoint(0.3073, 0.3209);
      var XYZ = c.ToXYZ();
      var rgb = c.ToRGB();
      var t = c.ToCorrelatedColorTemperature();
      var XYZ1 = rgb.ToXYZ();

      //6504K
      c = xyY.FromWhitePoint(0.312713, 0.329016);
      XYZ = c.ToXYZ();
      rgb = c.ToRGB();
      t = c.ToCorrelatedColorTemperature();
      XYZ1 = rgb.ToXYZ();

      XYZ = new RGB { R = 1, G = 1, B = 1 }.ToXYZ();
      c = XYZ.ToxyY();

      //6504K
      c = xyY.FromWhitePoint(0.31271, 0.32902);
      c = new xyY { x = 0.3127,              y = 0.3290,              Y = 1.000000 }; // close
      c = new xyY { x = 0.31273,             y = 0.32902,             Y = 1.000000 }; // close
      c = new xyY { x = 0.312727,            y = 0.329023,            Y = 1.000000 }; // close
      c = new xyY { x = 0.3127266,           y = 0.3290231,           Y = 1.000000 }; // ??
      c = new xyY { x = 0.31272661,          y = 0.32902314,          Y = 1.000000 }; // ??
      c = new xyY { x = 0.31272661468101209, y = 0.329023130326062,   Y = 1.000000 }; // closer (seems closer to XYZ)
      c = new xyY { x = 0.31272660439158345, y = 0.32902315240275221, Y = 1.000000 }; // closest

      XYZ = c.ToXYZ();
      rgb = c.ToRGB();
      t = c.ToCorrelatedColorTemperature();
      XYZ1 = rgb.ToXYZ();

      //6504K
      c = xyY.FromWhitePoint(0.3127, 0.3290);

      c = new XYZ {X = 0.950543, Y = 1.0, Z = 1.089303}.ToxyY();
      XYZ = c.ToXYZ();
      rgb = c.ToRGB();
      t = c.ToCorrelatedColorTemperature();
      XYZ1 = rgb.ToXYZ();

      //6504K
      c = xyY.FromWhitePoint(0.312699, 0.329001);

      c = new XYZ {X = 0.9504700, Y = 1, Z = 1.0888300}.ToxyY();

      XYZ = c.ToXYZ();
      rgb = c.ToRGB();
      t = c.ToCorrelatedColorTemperature();
      XYZ1 = rgb.ToXYZ();

      //6429K/6382K
      c = xyY.FromWhitePoint(0.3137, 0.3318);
      XYZ = c.ToXYZ();
      rgb = c.ToRGB();
      t = c.ToCorrelatedColorTemperature();
      XYZ1 = rgb.ToXYZ();

      XYZ = new XYZ { X = 133.82226318612311, Y = 140.71370657483300, Z = 153.52332866473165 };

      t = XYZ.ToCorrelatedColorTemperature();
      var il = t.ToIlumninant();
      var wp = il.ToXYZ();

      var ewp = new XYZ { X=0.96878319920108191, Y=1.0000000000000000, Z=1.1212169933925180 };
      //{X=0.96878319920108191 Y=1.0000000000000000 Z=1.1212169933925180 }


      var nXYZ = XYZ.Normalize();

      var Lab = nXYZ.ToLab(wp);

      // should be 2.869714
      double de2000 = Lab.DifferenceToWhitePoint(DeltaE.CIE1976);
      double de2000kt = Lab.Difference(Lab.WhitePoint, DeltaE.CIE1994);
      double de2000k = Lab.DifferenceToWhitePoint();
      

      XYZ = new XYZ { X = 133.164316, Y= 139.938897, Z = 154.836290 };

      Lab = XYZ.ToLab(XYZ.D50 * 100); // output from argyll sportread

      XYZ = XYZ.Normalize();
      Lab = XYZ.ToLab();


      //var Labr = XYZ.D50.ToLab();
      
      il.Y = XYZ.Y;
      var Labr = il.ToXYZ().ToLab();

      var de = Lab.Difference(Labr, DeltaE.CIE1994);

      XYZ = Lab.ToXYZ();

      var Lab2 = new Lab {L= 113.865594, a= -3.623637, b= -19.662553};
      Lab2 = Lab2.Normalize();

      c = XYZ.ToxyY();
      XYZ = XYZ.Normalize();
      XYZ = XYZ.ScaleToD65();

      XYZ = new XYZ { X = 132.683388, Y = 140.419845, Z = 148.432154 };
      XYZ = XYZ.Normalize();
      c = new xyY { x = 0.314762, y = 0.333115, Y = 140.419845 };
      c = XYZ.ToxyY();

      XYZ = XYZ.ScaleToD65();
      XYZ = XYZ.Normalize();

      Console.WriteLine("done");

    }
  }
}
