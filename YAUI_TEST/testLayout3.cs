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
    <resizer name='client' size='512' layout='vertical' layoutFilled='true'>
        <div size='32' layout='horizon' layoutFilled='true' layoutInverse='true' expandX='true'>
            <label text='x'></label>
            <div layout='horizon' layoutFilled='true'>
                <label text='template' size='12'></label>
                <div name='tabs' expandY='true' layout='horizon'>
                    <rect width='*128' marginX='*1' expandY='*true'>
                        <label derived='false' text='tools' align='center' debugName='label'></label>
                    </rect>
                    <rect>
                        <label derived='false' text='tools2' debugName='lable2' align='center'></label>
                    </rect>
                </div>
            </div>
        </div>
        <rect expandX='true' height='128'></rect>
        <rect></rect>
    </resizer>
</rect>
";
        const string XMLPAGE2 = @"
<rect clip='true' shrink='true' color='DarkGoldenrod' layout='vertical' padding='5' dragAble='true' rotateAble='true' scaleAble='true'>
    <resizer name='client' size='512' layout='vertical' >
        <rect paddingX='13' size='32' layout='horizon' layoutInverse='true' expandX='true'>
            <label text='x'></label>
            <rect expand='true' layout='horizon'>
                <label text='template' size='12'></label>
                <div name='tabs' expandY='true' height='48' layout='horizon'>
                    <rect width='*128' marginX='*1' expandY='*true'>
                        <label derived='false' text='tools' align='center' debugName='label'></label>
                    </rect>
                    <rect>
                        <label derived='false' text='tools2' debugName='lable2' align='center'></label>
                    </rect>
                </div>
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
