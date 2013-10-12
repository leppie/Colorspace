namespace Colorspace
{
  /// <summary>
  /// Represents the Lab color
  /// </summary>
  public struct Lab
  {
    // L    [0,   100]
    // a,b  [-128,127]
    public double L, a, b;

    /// <summary>
    /// The Lab white point
    /// </summary>
    public static readonly Lab WhitePoint = new Lab {L = 100 };

    public override string ToString()
    {
#if DEBUG
      return string.Format("L={0:f6}, a={1:f6}, b={2:f6}", L, a, b);
#else
      return string.Format("L={0:f4}, a={1:f4}, b={2:f4}", L, a, b);
#endif
    }

  }
}