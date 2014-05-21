using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Xml;
using ns_YAUtils;
namespace ns_YAUIUser
{
    using ns_YAUI;

    class testLayout2 : iTestInstance
    {
        
        ECategory iTestInstance.category()
        {
            return ECategory.example;
        }

        string iTestInstance.title()
        {
            return "layout2";
        }

        string iTestInstance.desc()
        {
            return "flat UI";
        }

        UIWidget iTestInstance.getAttach()
        {
            try
            {
                const string XMLLayout1 = @"
<rect size='320, 480' location='24, 24' layout='vertical, filled' fillColor='0xff33b887'>
<div height='42' expandX='true'>
    <label align='left' name='back' text='取消' size='*16' color='*white' style='*bold'></label>
    <label align='center' text='删除' color='0xffffaf60'></label>
    <label align='right' name='enter' text='确定'></label>
</div>
<div height='100' layout='horizon,filled' expandX='true'>
    <rect width='140' expandY='true' fillColor='*0xffdeb887'> 
        <label align='center' text='03' size='20' color='white' style='bold'></label>
    </rect>
    <rect width='30' expandY='true' > 
        <label align='center' text=':' size='20' color='white' style='bold'></label>
    </rect>
    <rect >
        <label align='center' text='20' size='20' color='white' style='bold'></label>
    </rect>
</div>
<div height='48' layout='horizon' expandX='true' margin='4'>
    <label text='日' size='*16' color='silver' style='*bold'></label>
    <label text='一' color='*white'></label>
    <label text='二'></label>
    <label text='三'></label>
    <label text='四'></label>
    <label text='五'></label>
    <label text='六' color='silver'></label>
</div>
<label text='循环' size='16' color='white' style='*bold' margin='4' ></label>
<div expandX='true' height='64'>
    <label text='desc:关机提醒' align='leftTop' offsetY='20' margin='4' size='16' color='white' style='bold'></label>
</div>
</rect>
";
                const string XMLLayout2 = @"
<rect size='320, 480' location='24, 24' layout='vertical' fillColor='0xff33b887'>
    <div height='42' layout='horizon, expandX, filled'>
        <rect width='80' fillColor='*0' layout='expandY'>
            <label text='+' color='white' style='bold' size='24' align='center'></label>
        </rect>
        <rect width='80' layout='expandY'>
            <label text='shut'  color='white' style='bold' size='16' align='center'></label>
        </rect>
        <rect width='80' layout='expandY'></rect>
        <rect width='80' layout='expandY'>
            <label text='—' color='white' style='bold' size='16' align='left' margin='4'></label>
            <label text='X' color='white' style='bold' size='16' align='right' margin='4'></label>
        </rect>

    </div>
    <div height='42' layout='vertical, expandX'>
        <innerTemplate name='line'>
            <div height='42' layout='horizon'>
                <rect width='100' layout='expandY' fillColor='*0xffdeb887'>
                    <label name='lb1' text='01:24' color='white' style='bold' size='12' align='center'></label>
                </rect>
                <rect width='100' layout='expandY'>
                    <label name='lb2' text='周一二三' color='white' style='bold' size='12' align='center'></label>
                </rect>
                <rect width='120' layout='expandY'>
                    <label name='lb3' text='desc' color='white' style='bold' size='12' align='center'></label>
                </rect>
            </div>
        </innerTemplate>
        <apply innerTemplate='line' lb1.text='17:24' lb2.text='周一' lb3.text='task1'></apply>
        <apply innerTemplate='line' lb1.text='18:24' lb2.text='周二' lb3.text='task2'></apply>
        <apply innerTemplate='line' lb1.text='00:00' lb2.text='周日六'  lb3.text='no task'></apply>
    </div>
</rect>
";
                //UI.Instance.root.dragAble = false;
                var ui = UI.Instance.fromXML(XMLLayout1);
                var ui1 = UI.Instance.fromXML(XMLLayout2, false);
                var back = ui.findByPath("back");
                back.evtOnLMUp += (self, x, y) =>
                {
                    ui1.paresent = ui.paresent;
                    ui.paresent = null;
                    return false;
                };
                var enter = ui.findByPath("enter");
                enter.evtOnLMUp += (self, x, y) =>
                {
                    ui1.paresent = ui.paresent;
                    ui.paresent = null;
                    return false;
                };
                ui1.evtOnLMUp += (self, x, y) =>
                {
                    ui.paresent = ui1.paresent;
                    ui1.paresent = null;
                    return false;
                };
                return ui;
                //lb.link = true;
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
                //Console.WriteLine(e.LineNumber);
                //Console.WriteLine(e.LinePosition);
            }
            return null;
        }

        void iTestInstance.lostAttach()
        {
            return;
        }
    }
}
