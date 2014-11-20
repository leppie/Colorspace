using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Colorspace.Sampling
{
  /// <summary>
  /// Interaction with Argyll, mostly taylored for ColorMunki DisplayType
  /// </summary>
  public static class Argyll
  {
#if DEBUG
    static Argyll()
    {
      BinPath = @"D:\dev\Argyll_V1.6.1\bin\";
    }
#endif

    /// <summary>
    /// Gets or sets the path of the Argyll binaries
    /// </summary>
    /// <value>
    /// The path to Argyll binaries
    /// </value>
    public static string BinPath { get; set;}

    /// <summary>
    /// Get the measure from dispcal
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">BinPath;Please set Argyll.BinPath</exception>
    public static Measure GetMeasure(int displaynr = 1, DisplayType sc = DisplayType.LCD_CCFL)
    {
      if (BinPath == null)
      {
        throw new ArgumentNullException("BinPath", "Please set Argyll.BinPath");
      }

      var start = DateTime.Now;

      var p = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = Path.Combine(BinPath, "dispcal.exe"),
          Arguments = string.Format( "-d{0} -y{1} -Yp -r", displaynr, (char) sc) ,
          CreateNoWindow = true,
          RedirectStandardOutput = true,
          UseShellExecute = false
        },
      };

      p.Start();
      p.WaitForExit();

      var time = DateTime.Now;

      // takes 11.5 seconds
      //Console.WriteLine("{0:f1}", (time - start).TotalSeconds);

      var lines = p.StandardOutput.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      var whitelevel = Regex.Match(lines[3], @"\d+\.\d+").Value;
      var gamma = Regex.Match(lines[4], @"\d+\.\d+").Value;
      var contrast = Regex.Match(lines[5], @"\d+").Value;
      var m = Regex.Matches(lines[6], @"\d+\.\d+");

      var x = m[0].Value;
      var y = m[1].Value;

      var temp = Regex.Match(lines[10], @"\d+K").Value.TrimEnd('K');

      var de = Regex.Match(lines[10], @"\d+\.\d+").Value;

      var c = new xyY { x = double.Parse(x), y = double.Parse(y), Y = double.Parse(whitelevel)};

      var xyz = c.ToXYZ();

      var rgb = xyz.TosRGB();

      return new Measure
      {
        Time = time,
        Contrast = double.Parse(contrast),
        DeltaE = double.Parse(de),
        Gamma = double.Parse(gamma),
        Luminance = double.Parse(whitelevel),
        Temperature = double.Parse(temp),
        Blue = rgb.B,
        Red = rgb.R,
        Green = rgb.G,
        X = xyz.X,
        Y = xyz.Y,
        Z = xyz.Z,
        RGB = rgb,
        XYZ = xyz
      };
    }

    /// <summary>
    /// Continuously sample XYZ values
    /// </summary>
    /// <returns>the XYZ value</returns>
    /// <exception cref="System.ArgumentNullException">BinPath;Please set Argyll.BinPath</exception>
    /// <remarks>Make sure to dispose the enumerator if not using foreach</remarks>
    // thanks @controlflow for this trick :D
    public static IEnumerable<XYZ> ContinuousRead(DisplayType sc = DisplayType.LCD_CCFL, string inttime = "0.2")
    {
      if (BinPath == null)
      {
        throw new ArgumentNullException("BinPath", "Please set Argyll.BinPath");
      }

      var t = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          // this is a modified version of spotread to do continuous readings
          FileName = Path.Combine(BinPath, "contread.exe"),
          Arguments = string.Format("-y{0}", (char)sc),
          CreateNoWindow = true,
          RedirectStandardOutput = true,
          RedirectStandardInput = true,
          UseShellExecute = false,
        }
      };

      t.StartInfo.EnvironmentVariables["I1D3_DEF_INT_TIME"] = inttime;

      try
      {
        t.Start();

        Debug.Print("contread process started");

        while (true)
        {
          var v = t.StandardOutput.ReadLine();

          if (string.IsNullOrEmpty(v))
          {
            yield break;
          }

          XYZ current;

          try
          {
            var values = v.Split(' ').Select(double.Parse).ToArray();

            current = new XYZ
            {
              X = values[0],
              Y = values[1],
              Z = values[2]
            };

            Debug.Print("got sample: {0}", current);
          }
          catch
          {
            Debug.Print("unparseable data received: {0}", v);
            yield break;
          }

          yield return current;
        }
      }
      finally
      {
        if (!t.HasExited)
        {
          t.Kill();
          Debug.Print("contread process killed");
        }
        t.Dispose();
      }
    }
  }
}