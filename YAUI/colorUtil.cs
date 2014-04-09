using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YAUI
{
    //http://en.wikipedia.org/wiki/Web_colors
    //CSS 1–2.0 / HTML 3.2–4 / VGA color names
    enum EColorUtil : uint
    {
        invalid = 0xCDCDCDCD,
        white = 0xffffffff,
        silver = 0xffc0c0c0,
        gray = 0xff808080,
        black = 0xff000000,
        red = 0xffff0000,
        maroon = 0xff800000,
        yellow = 0xffffff00,
        olive = 0xff808000,
        lime = 0xff00ff00,
        green = 0xff008000,
        aqua = 0xff00ffff,
        teal = 0xff008080,
        blue = 0xff0000ff,
        navy = 0xff000080,
        fuchsia = 0xffff00ff,
        purple = 0xff800080,
    }
}
