using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YAUIUser
{
    using ns_YAUI;

    class testLayout3 : Singleton<testLayout3>
    {
        const string XMLPAGE = @"
<rect clip='true' shrink='true' color='DarkGoldenrod' layout='vertical' padding='5' dragAble='true' rotateAble='true' scaleAble='true'>
    <resizer name='client' length='512' layout='vertical' layoutFilled='true'>
        <blank length='32' layout='horizon' layoutFilled='true' layoutInverse='true' expandX='true'>
            <lable text='x'></lable>
            <blank layout='horizon' layoutFilled='true'>
                <lable text='template' size='12'></lable>
                <blank name='tabs' expandY='true' layout='horizon'>
                    <rect width='*128' marginX='*1' expandY='*true'>
                        <lable derived='false' text='tools' align='center' debugName='lable'></lable>
                    </rect>
                    <rect>
                        <lable derived='false' text='tools2' debugName='lable2' align='center'></lable>
                    </rect>
                </blank>
            </blank>
        </blank>
        <rect expandX='true' height='128'></rect>
        <rect></rect>
    </resizer>
</rect>
";
        const string XMLPAGE2 = @"
<rect clip='true' shrink='true' color='DarkGoldenrod' layout='vertical' padding='5' dragAble='true' rotateAble='true' scaleAble='true'>
    <resizer name='client' length='512' layout='vertical' >
        <rect paddingX='13' length='32' layout='horizon' layoutInverse='true' expandX='true'>
            <lable text='x'></lable>
            <rect expand='true' layout='horizon'>
                <lable text='template' size='12'></lable>
                <blank name='tabs' expandY='true' height='48' layout='horizon'>
                    <rect width='*128' marginX='*1' expandY='*true'>
                        <lable derived='false' text='tools' align='center' debugName='lable'></lable>
                    </rect>
                    <rect>
                        <lable derived='false' text='tools2' debugName='lable2' align='center'></lable>
                    </rect>
                </blank>
            </rect>
        </rect>
        
    </resizer>
</rect>
";
        public testLayout3()
        {
            UIRoot.Instance.root.appendFromXML(XMLPAGE);

        }
    }
}
