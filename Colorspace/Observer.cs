namespace Colorspace
{
  /// <summary>
  /// Represents an observer
  /// </summary>
  public class Observer
  {
    internal string Name;
    internal int start, end;
    internal double[] x, y, z;

    int Interval
    {
      get { return (end - start) / (x.Length - 1); }
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
