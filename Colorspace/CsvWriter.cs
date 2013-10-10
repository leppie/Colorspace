using System;

namespace Colorspace
{
  public static class CsvWriter
  {
    public static string ToCSVHeader(this Measure _)
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

    public static string ToCSV(this Measure m)
    {
      return string.Format("{0},{1:f2},{2:f0},{3:f0},{4:f1},{5:f2},{6:f4},{7:f4},{8:f4},{9:f6},{10:f6},{11:f6}{12}", 
        m.Seconds, 
        m.Gamma, 
        m.Temperature, 
        m.Contrast, 
        m.DeltaE, 
        m.Luminance, 
        m.Red, 
        m.Green, 
        m.Blue, 
        m.X, 
        m.Y, 
        m.Z,
        Environment.NewLine);
    }
  }
}