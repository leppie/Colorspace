namespace Colorspace
{
  /// <summary>
  /// Represents an illuminant
  /// </summary>
  public class Illuminant
  {
    internal string Name;
    internal int start, end;
    internal double[] s;

    int Interval
    {
      get { return (end - start) / (s.Length - 1); }
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
