using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace MillimanSupport.Calendar
{
    public class CalendarItem
    {

        public string Subject { get; set; }
        public bool IsBold { get; set; }
        public bool IsStrikeOut { get; set; }
        public bool IsItalic { get; set; }
        public int FontSize { get; set; }
        public string FontName { get; set; }
        public Color FontColor { get; set; }
        public Color BackColor { get; set; }
        public string ToolTip { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public CalendarItem() { }

        public CalendarItem(string _Subject, bool _IsBold, bool _IsStrikeOut, bool _IsItalic, int _FontSize, string _FontName, Color _FontColor, Color _BackColor, string _Tooltip, DateTime _Start, DateTime _End)
        {
            Subject = _Subject;
            IsBold = _IsBold;
            IsStrikeOut = _IsStrikeOut;
            IsItalic = _IsItalic;
            FontSize = _FontSize;
            FontName = _FontName;
            FontColor = _FontColor;
            BackColor = _BackColor;
            ToolTip = _Tooltip;
            Start = _Start;
            End = _End;
        }
    }
}