namespace Colorspace
{
  public struct XYZ
  {
    public double X, Y, Z;

    public static readonly XYZ D50 = new XYZ { X = 0.96422, Y = 1.0, Z = 0.82521 };
    public static readonly XYZ D55 = new XYZ { X = 0.95682, Y = 1.0, Z = 0.92149 };
    public static readonly XYZ D65 = new XYZ { X = 0.950470558654, Y = 1.000000000000, Z = 1.088828736396 };
    public static readonly XYZ D75 = new XYZ { X = 0.94972, Y = 1.0, Z = 1.22638 };

    public override string ToString()
    {
      return string.Format("X={0:f6}, Y={1:f6}, Z={2:f6}", X, Y, Z);
    }

    public static XYZ operator *(XYZ c, double m)
    {
      return new XYZ
      {
        X = c.X * m,
        Y = c.Y * m,
        Z = c.Z * m,
      };
    }
  }
}