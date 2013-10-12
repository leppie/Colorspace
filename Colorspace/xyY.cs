using System;

namespace Colorspace
{
  /// <summary>
  /// Represent the xyY color
  /// </summary>
  public struct xyY
  {
    public double x, y, Y;

    public xyY(double x, double y, double Y = 1)
    {
      this.x = x;
      this.y = y;
      this.Y = Y;
    }

    public double z
    {
      get { return 1 - x - y; }
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