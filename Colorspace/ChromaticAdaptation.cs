namespace Colorspace
{
  static class ChromaticAdaptation
  {
    static readonly Matrix3x3 Bradford = new double[,] 
    {
      { 0.8951,  0.2664, -0.1614},
      {-0.7502,  1.7135,  0.0367},
      { 0.0389, -0.0685,  1.0296}
    };

    public static XYZ Scale(this XYZ c, XYZ srcwp, XYZ dstwp)
    {
      http://www.brucelindbloom.com/Eqn_ChromAdapt.html

      var MA = Bradford;
      var MA_1 = MA.Invert();

      var d = (MA * dstwp).data;
      var s = (MA * srcwp).data;

      var PYB = new double[,] 
      {
        {d[0]/s[0], 0, 0},
        {0, d[1]/s[1], 0},
        {0, 0, d[2]/s[2]}
      };

      var M = MA_1 * PYB * MA;

      return c * M;
    }
  }
}
