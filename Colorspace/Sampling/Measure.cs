using System;

namespace Colorspace.Sampling
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

    public static string ToCSVHeader()
    {
      return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}{12}",
        "Seconds",
        "Gamma",
        "Temperature",
        "Contrast",
        "DeltaE",
        "Luminance",
        "Red",
        "Green",
        "Blue",
        "X",
        "Y",
        "Z",
        Environment.NewLine);
    }

    public string ToCSV()
    {
      return string.Format("{0},{1:f2},{2:f0},{3:f0},{4:f1},{5:f2},{6:f4},{7:f4},{8:f4},{9:f6},{10:f6},{11:f6}{12}",
        Seconds,
        Gamma,
        Temperature,
        Contrast,
        DeltaE,
        Luminance,
        Red,
        Green,
        Blue,
        X,
        Y,
        Z,
        Environment.NewLine);
    }
  }
}
