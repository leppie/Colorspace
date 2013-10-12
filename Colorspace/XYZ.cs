namespace Colorspace
{
  public struct XYZ
  {
    public double X, Y, Z;

    public static readonly XYZ D50_Whitepoint = new XYZ { X = 0.96422054826086956, Y = 1.0, Z = 0.825208953327554 }; // derived
    public static readonly XYZ D55_Whitepoint = new XYZ { X = 0.95682, Y = 1.0, Z = 0.92149 };
    public static readonly XYZ D65_Whitepoint = new XYZ { X = 0.950470558654283, Y = 1.0, Z = 1.08882873639588 }; // calculated from tables
    public static readonly XYZ D75_Whitepoint = new XYZ { X = 0.94972, Y = 1.0, Z = 1.22638 };

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

    public static implicit operator xyY(XYZ c)
    {
      return (Vector3)c;
    }
  }
}