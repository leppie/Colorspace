using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colorspace.Sampling
{
  /// <summary>
  /// Display settings for ColorMunki Display
  /// </summary>
  public enum Display
  {
    NonRefresh = 'n',
    Refresh = 'r',
    CRT = '1',
    LCD_CCFL = '2',
    LCD_CCFL_WideGamut,
    LCD_RGB_LED,
    LCD_White_LED = '5',
    LCD_OLED,
    Plasma,
    Projector,
    LCD_RGPhosphor_LED = '9',
    LCD = 'l',
  }
}
