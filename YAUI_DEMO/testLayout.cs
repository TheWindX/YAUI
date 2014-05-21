using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ns_YAUIUser
{
    using ns_YAUI;

    class testLayout : iTestInstance
    {
        const string XMLPAGE = @"
<resizer location='24, 24' scale='0.9' derived='true' name='root' size='512' layout='vertical, filled' color='0xff194e88' editMode='dragAble, rotateAble, scaleAble'>
    <div layout='horizon, inverse' align='rightTop' id='buttons'>
        <label text='×' color='*white' margin='*2'></label>
        <label text='□'></label>
        <label text='_'></label>
    </div>

    <div layout='horizon, shrink' margin='4' id='head title'>
        <round_rect size='64'>
            <label text='♛' align='center' color='gold' size='32'></label>
        </round_rect>
        <div layout='vertical, shrink'>
            <label text='风云交流讨论群' color='*white' size='14' style='bold'></label>
            <label text='(1000人群)' size='10'></label>
        </div>
    </div>

    <rect height='40' layout='expandX' id='tabs_advs' color='0xff2d6398'>
        <div layout='horizon' align='leftBottom' height='32' id='tabs'>
            <innerTemplate name='tab'>
                <round_rect layout='horizon, expandY' width='96' fillColor='royalblue' leftBottomCorner='false' rightBottomCorner='false'>
                    <label name='lb' text='聊天' align='center'></label>
                </round_rect>
            </innerTemplate>
            <apply innerTemplate='tab' lb.text='聊天' lb.color='white' fillColor='0xff81a6c3'></apply>
            <apply innerTemplate='tab' lb.text='相片' lb.color='white' fillColor='transparent' strokeColor='transparent'></apply>
            <apply innerTemplate='tab' lb.text='文件' lb.color='white' fillColor='transparent' strokeColor='transparent'></apply>
        </div>
        <div layout='horizon, inverse' align='rightBottom' height='60' id='advs'>
            <round_rect layout='horizon, expandY' width='128'>
                <label text='广告位1' align='center'></label>
            </round_rect>
        </div>
    </rect>

    <div id='leftRight' layout='horizon, inverse, filled' >
        <round_rect layout='vertical, expandY' margin='6' width='180' id='frind_list' color='0xffedf5fa'>
            <round_rect layout='expandX' height='28'>
                <label text='群成员' color='black' size='8' align='left'></label>
                <label text='?' align='right'></label>
            </round_rect>
            <label text='楚楚' margin='*4' color='*black' size='*8'></label>
            <label text='聂风'></label>
            <label text='步惊云'></label>
            <label text='无名'></label>
        </round_rect>

        <rect layout='vertical, inverse, expandY, filled' clip='true'>
            <rect layout='expandX' height='28' color='0xffced6dd'>
                <label text='广告位2' align='left' size='8'></label>
                <div align='right' layout='horizon, inverse, expand'>
                    <innerTemplate name='bt'>
                        <round_rect layout='expandY' margin='4'> 
                            <label name='lb' text='' align='center' size='8' color='black'></label> 
                        </round_rect>
                    </innerTemplate>
                    <apply innerTemplate='bt' lb.text='发送'> </apply>
                    <apply innerTemplate='bt' lb.text='关闭'> </apply>
                    
                </div>
            </rect>
            <rect layout='expandX' height='128' color='white'>
            </rect>
            <rect layout='horizon, expandX' height='32' padding='4' clip='true' color='0xffced6dd'>
                <label text='A' color='blue' style='*bold'></label>                
                <label text='☺' size='18' offsetX='2' offsetY='-3' color='seagreen'></label>
                <label text='♪' color='red' offsetX='-6' offsetY='1'></label>
            </rect>
            <rect layout='vertical, expandX' clip='true' padding='4' color='white' id='chat'>
                <label text='楚楚:' color='*blue'></label>
                <label text='   金鳞岂是池中物' color='black' size='10'></label>
                <label text='聂风:'></label>
                <label text='   一遇风云便化龙' color='black' size='10'></label>
            </rect>
        </rect>
    </div>
</resizer>
";
        
        public ECategory category()
        {
            return ECategory.example;
        }

        public string title()
        {
            return "layout";
        }

        public string desc()
        {
            return @"
simulation window
";
        }

        public UIWidget getAttach()
        {
            return UI.Instance.fromXML(XMLPAGE, false);
        }

        public void lostAttach()
        {
            UI.Instance.clearID("buttons");
            UI.Instance.clearID("head title");
            UI.Instance.clearID("tabs_advs");
            UI.Instance.clearID("tabs");
            UI.Instance.clearID("advs");
            UI.Instance.clearID("frind_list");
            UI.Instance.clearID("chat");
            UI.Instance.clearID("leftRight");
        }
    }
}
