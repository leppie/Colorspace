namespace Colorspace
{
  public static class Scaling
  {
    //http://www.brucelindbloom.com/Eqn_RGB_XYZ_Matrix.html

    static readonly Matrix3x3 D65_D50_Bradford = new double[,]
        {
          {1.0478112, 0.0228866, -0.0501270},
          {0.0295424, 0.9904844, -0.0170491},
          {-0.0092345, 0.0150436, 0.7521316}
        };

    static readonly Matrix3x3 D50_D65_Bradford = new double[,]
        {
          {0.9555766, -0.0230393, 0.0631636},
          {-0.0282895, 1.0099416, 0.0210077},
          {0.0122982, -0.0204830, 1.3299098}
        };

    static readonly Matrix3x3 D65_D50_Linear = new double[,]
        {
          {1.0144665, 0.0000000, 0.0000000},
          {0.0000000, 1.0000000, 0.0000000},
          {0.0000000, 0.0000000, 0.7578869}
        };

    static readonly Matrix3x3 D50_D65_Linear = new double[,]
        {
          {0.9857398, 0.0000000, 0.0000000},
          {0.0000000, 1.0000000, 0.0000000},
          {0.0000000, 0.0000000, 1.3194581}
        };

    public static XYZ ScaleToD50(this XYZ c, bool bradford = true)
    {
      return c * (bradford ? D65_D50_Bradford : D65_D50_Linear);
    }

    public static XYZ ScaleToD65(this XYZ c, bool bradford = true)
    {
      return c * (bradford ? D50_D65_Bradford : D50_D65_Linear);
    }
  }
}