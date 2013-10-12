﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Colorspace.Sampling
{
  public static class Argyll
  {
#if DEBUG
    static Argyll()
    {
      BinPath = @"D:\dev\Argyll_V1.6.1\spectro\";
    }
#endif

    public static string BinPath { get; set;}

    public static Measure GetMeasure()
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
          Arguments = "-d2 -y2 -Yp -r",
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

      var rgb = xyz.ToRGB();

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

    // use with foreach, so it can be disposed
    // thanks @controlflow for this trick :D
    public static IEnumerable<XYZ> ContinuousRead()
    {
      var t = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          // this is a modified version of spotread to do continuous readings
          FileName = Path.Combine(BinPath, "contread.exe"),
          Arguments = "-y2", // screen type (CCFL LCD in this case) y5 for White LED
          CreateNoWindow = true,
          RedirectStandardOutput = true,
          RedirectStandardInput = true,
          UseShellExecute = false,
        }
      };

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