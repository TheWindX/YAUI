﻿using System;
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
        //invalid = 0xCDCDCDCD,
        //white = 0xffffffff,
        //silver = 0xffc0c0c0,
        //gray = 0xff808080,
        //black = 0xff000000,
        //red = 0xffff0000,
        //maroon = 0xff800000,
        //yellow = 0xffffff00,
        //olive = 0xff808000,
        //lime = 0xff00ff00,
        //green = 0xff008000,
        //aqua = 0xff00ffff,
        //teal = 0xff008080,
        //blue = 0xff0000ff,
        //navy = 0xff000080,
        //fuchsia = 0xffff00ff,
        //purple = 0xff800080,

        //pink = colors,
        pink = 0xffffc0cb,
        lightpink = 0xffffb6c1,
        hotpink = 0xffff69b4,
        deeppink = 0xffff1493,
        palevioletred = 0xffdb7093,
        mediumvioletred = 0xffc71585,
        //red = colors,
        lightsalmon = 0xffffa07a,
        salmon = 0xfffa8072,
        darksalmon = 0xffe9967a,
        lightcoral = 0xfff08080,
        indianred = 0xffcd5c5c,
        crimson = 0xffdc143c,
        firebrick = 0xffb22222,
        darkred = 0xff8b0000,
        red = 0xffff0000,
        //orange = colors,
        orangered = 0xffff4500,
        tomato = 0xffff6347,
        coral = 0xffff7f50,
        darkorange = 0xffff8c00,
        orange = 0xffffa500,
        //yellow = colors,
        yellow = 0xffffff00,
        lightyellow = 0xffffffe0,
        lemonchiffon = 0xfffffacd,
        lightgoldenrodyellow = 0xfffafad2,
        papayawhip = 0xffffefd5,
        moccasin = 0xffffe4b5,
        peachpuff = 0xffffdab9,
        palegoldenrod = 0xffeee8aa,
        khaki = 0xfff0e68c,
        darkkhaki = 0xffbdb76b,
        gold = 0xffffd700,
        //brown = colors,
        cornsilk = 0xfffff8dc,
        blanchedalmond = 0xffffebcd,
        bisque = 0xffffe4c4,
        navajowhite = 0xffffdead,
        wheat = 0xfff5deb3,
        burlywood = 0xffdeb887,
        tan = 0xffd2b48c,
        rosybrown = 0xffbc8f8f,
        sandybrown = 0xfff4a460,
        goldenrod = 0xffdaa520,
        darkgoldenrod = 0xffb8860b,
        peru = 0xffcd853f,
        chocolate = 0xffd2691e,
        saddlebrown = 0xff8b4513,
        sienna = 0xffa0522d,
        brown = 0xffa52a2a,
        maroon = 0xff800000,
        //green = colors,
        darkolivegreen = 0xff556b2f,
        olive = 0xff808000,
        olivedrab = 0xff6b8e23,
        yellowgreen = 0xff9acd32,
        limegreen = 0xff32cd32,
        lime = 0xff00ff00,
        lawngreen = 0xff7cfc00,
        chartreuse = 0xff7fff00,
        greenyellow = 0xffadff2f,
        springgreen = 0xff00ff7f,
        mediumspringgreen = 0xff00fa9a,
        lightgreen = 0xff90ee90,
        palegreen = 0xff98fb98,
        darkseagreen = 0xff8fbc8f,
        mediumseagreen = 0xff3cb371,
        seagreen = 0xff2e8b57,
        forestgreen = 0xff228b22,
        green = 0xff008000,
        darkgreen = 0xff006400,
        //cyan = colors,
        mediumaquamarine = 0xff66cdaa,
        aqua = 0xff00ffff,
        cyan = 0xff00ffff,
        lightcyan = 0xffe0ffff,
        paleturquoise = 0xffafeeee,
        aquamarine = 0xff7fffd4,
        turquoise = 0xff40e0d0,
        mediumturquoise = 0xff48d1cc,
        darkturquoise = 0xff00ced1,
        lightseagreen = 0xff20b2aa,
        cadetblue = 0xff5f9ea0,
        darkcyan = 0xff008b8b,
        teal = 0xff008080,
        //blue = colors,
        lightsteelblue = 0xffb0c4de,
        powderblue = 0xffb0e0e6,
        lightblue = 0xffadd8e6,
        skyblue = 0xff87ceeb,
        lightskyblue = 0xff87cefa,
        deepskyblue = 0xff00bfff,
        dodgerblue = 0xff1e90ff,
        cornflowerblue = 0xff6495ed,
        steelblue = 0xff4682b4,
        royalblue = 0xff4169e1,
        blue = 0xff0000ff,
        mediumblue = 0xff0000cd,
        darkblue = 0xff00008b,
        navy = 0xff000080,
        midnightblue = 0xff191970,
        //purple = colors,
        lavender = 0xffe6e6fa,
        thistle = 0xffd8bfd8,
        plum = 0xffdda0dd,
        violet = 0xffee82ee,
        orchid = 0xffda70d6,
        fuchsia = 0xffff00ff,
        magenta = 0xffff00ff,
        mediumorchid = 0xffba55d3,
        mediumpurple = 0xff9370db,
        blueviolet = 0xff8a2be2,
        darkviolet = 0xff9400d3,
        darkorchid = 0xff9932cc,
        darkmagenta = 0xff8b008b,
        purple = 0xff800080,
        indigo = 0xff4b0082,
        darkslateblue = 0xff483d8b,
        slateblue = 0xff6a5acd,
        mediumslateblue = 0xff7b68ee,
        //white = colors,
        white = 0xffffffff,
        snow = 0xfffffafa,
        honeydew = 0xfff0fff0,
        mintcream = 0xfff5fffa,
        azure = 0xfff0ffff,
        aliceblue = 0xfff0f8ff,
        ghostwhite = 0xfff8f8ff,
        whitesmoke = 0xfff5f5f5,
        seashell = 0xfffff5ee,
        beige = 0xfff5f5dc,
        oldlace = 0xfffdf5e6,
        floralwhite = 0xfffffaf0,
        ivory = 0xfffffff0,
        antiquewhite = 0xfffaebd7,
        linen = 0xfffaf0e6,
        lavenderblush = 0xfffff0f5,
        mistyrose = 0xffffe4e1,
        //gray/black = colors,
        gainsboro = 0xffdcdcdc,
        lightgray = 0xffd3d3d3,
        silver = 0xffc0c0c0,
        darkgray = 0xffa9a9a9,
        gray = 0xff808080,
        dimgray = 0xff696969,
        lightslategray = 0xff778899,
        slategray = 0xff708090,
        darkslategray = 0xff2f4f4f,
        black = 0xff000000,
    }
}
