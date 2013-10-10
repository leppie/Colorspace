using System;

namespace Colorspace
{
  public class Measure
  {
    public int Seconds {get; set; }

    public DateTime Time { get; set; }

    public double Gamma { get; set; }
    public double Temperature { get; set; }

    public double Contrast { get; set; }

    public double DeltaE { get; set; }

    public double Luminance { get; set; }

    public double Red { get; set; }
    public double Green { get; set; }
    public double Blue { get; set; }

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public RGB RGB {get;set;}
    public XYZ XYZ { get; set; }

  }
}
