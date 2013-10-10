﻿namespace Colorspace
{
  public struct xyY
  {
    public double x, y, Y;

    public static xyY FromWhitePoint(double x, double y)
    {
      return new xyY
      {
        x = x,
        y = y,
        Y = 1 - x - y
      };
    }

    public override string ToString()
    {
#if DEBUG
      return string.Format("x={0:f6}, y={1:f6}, Y={2:f6}", x, y, Y);
#else
      return string.Format("x={0:f4}, y={1:f4}, Y={2:f4}", x, y, Y);
#endif
    }
  }
}