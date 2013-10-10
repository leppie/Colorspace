namespace Colorspace
{
  public struct RGB
  {
    public double R, G, B;

    public override string ToString()
    {
#if DEBUG
      return string.Format("R={0:f6}, G={1:f6}, B={2:f6}", R, G, B);
#else
      return string.Format("R={0:f4}, G={1:f4}, B={2:f4}", R, G, B);
#endif
    }
  }
}