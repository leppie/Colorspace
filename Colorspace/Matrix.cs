using System;

namespace Colorspace
{
  public class Matrix3x3
  {
    readonly double[,] data;

    Matrix3x3(double[,] data)
    {
      this.data = data;
    }

    public double this[int x, int y]
    {
      get
      {
        return data[x, y];
      }
    }

    public static implicit operator Matrix3x3(double[,] m)
    {
      if (m.GetLength(0) != 3 || m.GetLength(1) != 3)
      {
        throw new ArgumentException("not a 3 x 3 array", "m");
      }

      return new Matrix3x3(m);
    }
  }

  public class Vector3
  {
    readonly double[] data;

    Vector3(double[] data)
    {
      this.data = data;
    }

    public double this[int x]
    {
      get
      {
        return data[x];
      }
    }

    public static implicit operator Vector3(double[] m)
    {
      if (m.Length != 3)
      {
        throw new ArgumentException("not a 3 x 1 array", "m");
      }

      return new Vector3(m);
    }

    public static implicit operator Vector3(xyY c)
    {
      return new double[] { c.x, c.y, c.Y };
    }

    public static implicit operator xyY(Vector3 v)
    {
      return new xyY { x = v[0], y = v[1], Y = v[2] };
    }

    public static implicit operator Vector3(XYZ c)
    {
      return new double[] { c.X, c.Y, c.Z };
    }

    public static implicit operator XYZ(Vector3 v)
    {
      return new XYZ { X = v[0], Y = v[1], Z = v[2] };
    }

    public static implicit operator Vector3(RGB c)
    {
      return new double[] { c.R, c.G, c.B };
    }

    public static implicit operator RGB(Vector3 v)
    {
      return new RGB { R = v[0], G = v[1], B = v[2] };
    }

    public static implicit operator Vector3(Lab c)
    {
      return new double[] { c.L, c.a, c.b };
    }

    public static implicit operator Lab(Vector3 v)
    {
      return new Lab { L = v[0], a = v[1], b = v[2] };
    }

    
    public static Vector3 operator *(Matrix3x3 M, Vector3 V)
    {
      return V * M;
    }

    public static Vector3 operator *(Vector3 V, Matrix3x3 M)
    {
      return new double[]
      {
        V[0] * M[0, 0] + V[1] * M[0, 1] + V[2] * M[0, 2],
        V[0] * M[1, 0] + V[1] * M[1, 1] + V[2] * M[1, 2],
        V[0] * M[2, 0] + V[1] * M[2, 1] + V[2] * M[2, 2]
      };
    }
  }
}
