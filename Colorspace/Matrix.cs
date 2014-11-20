using System;
using System.Diagnostics;

namespace Colorspace
{
  class Matrix3x3
  {
    readonly double[,] data;

    Matrix3x3(double[,] data)
    {
      this.data = data;
    }

    public static implicit operator Matrix3x3(double[,] m)
    {
      if (m.GetLength(0) != 3 || m.GetLength(1) != 3)
      {
        throw new ArgumentException("not a 3 x 3 array", "m");
      }

      return new Matrix3x3(m);
    }

    public Matrix3x3 Invert()
    {
      var m = data;
      // computes the inverse of a matrix m
      double det = m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) -
                   m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]) +
                   m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);

      double invdet = 1 / det;

      var minv = new double[3, 3]; // inverse of matrix m
      minv[0, 0] = (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) * invdet;
      minv[0, 1] = (m[0, 2] * m[2, 1] - m[0, 1] * m[2, 2]) * invdet;
      minv[0, 2] = (m[0, 1] * m[1, 2] - m[0, 2] * m[1, 1]) * invdet;
      minv[1, 0] = (m[1, 2] * m[2, 0] - m[1, 0] * m[2, 2]) * invdet;
      minv[1, 1] = (m[0, 0] * m[2, 2] - m[0, 2] * m[2, 0]) * invdet;
      minv[1, 2] = (m[1, 0] * m[0, 2] - m[0, 0] * m[1, 2]) * invdet;
      minv[2, 0] = (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]) * invdet;
      minv[2, 1] = (m[2, 0] * m[0, 1] - m[0, 0] * m[2, 1]) * invdet;
      minv[2, 2] = (m[0, 0] * m[1, 1] - m[1, 0] * m[0, 1]) * invdet;

      return new Matrix3x3(minv);
    }

    public static Vector3 operator *(Matrix3x3 M, Vector3 V)
    {
      return V * M;
    }

    public static Vector3 operator *(Vector3 v, Matrix3x3 m)
    {
      var M = m.data;
      var V = v.data;
      return new double[]
      {
        V[0] * M[0, 0] + V[1] * M[0, 1] + V[2] * M[0, 2],
        V[0] * M[1, 0] + V[1] * M[1, 1] + V[2] * M[1, 2],
        V[0] * M[2, 0] + V[1] * M[2, 1] + V[2] * M[2, 2]
      };
    }

    public static Matrix3x3 operator *(Matrix3x3 m1, Matrix3x3 m2)
    {
      var M1 = m1.data;
      var M2 = m2.data;

      var R = new double[3,3];

      for (int j = 0; j < 3; j++)
      {
        for (int i = 0; i < 3; i++)
        {
          double tt = 0.0;
          
          for (int k = 0; k < 3; k++)
          {
            tt += M1[j, k] * M2[k, i];
          }

          R[j, i] = tt;
        }
      }

      return new Matrix3x3(R);
    }
  }

  [DebuggerDisplay("{data[0]} {data[1]} {data[2]}")]
  class Vector3
  {
    internal readonly double[] data;

    Vector3(double[] data)
    {
      this.data = data;
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

    public static implicit operator xyY(Vector3 V)
    {
      var v = V.data;
      return new xyY { x = v[0], y = v[1], Y = v[2] };
    }

    public static implicit operator Vector3(XYZ c)
    {
      c = c.Normalize();
      return new double[] { c.X, c.Y, c.Z };
    }

    public static implicit operator XYZ(Vector3 V)
    {
      var v = V.data;
      return new XYZ { X = v[0], Y = v[1], Z = v[2] };
    }

    public static implicit operator Vector3(RGB c)
    {
      return new double[] { c.R, c.G, c.B };
    }

    public static implicit operator RGB(Vector3 V)
    {
      var v = V.data;
      return new RGB { R = v[0], G = v[1], B = v[2] };
    }

    public static implicit operator Vector3(Lab c)
    {
      c = c.Normalize();
      return new double[] { c.L, c.a, c.b };
    }

    public static implicit operator Lab(Vector3 V)
    {
      var v = V.data;
      return new Lab { L = v[0], a = v[1], b = v[2] };
    }

    public static Vector3 operator *(Vector3 V, Func<double, double> t)
    {
      var v = V.data;

      return new double[]
      {
        t(v[0]), t(v[1]), t(v[2])
      };
    }

  }
}
