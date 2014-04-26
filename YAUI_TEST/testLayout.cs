using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YAUIUser
{
    using ns_YAUI;

    class testLayout : Singleton<testLayout>
    {
        const string XMLPAGE = @"
<resizer name='root' size='512' layout='vertical, filled' color='blue' editMode='dragAble, rotateAble, scaleAble'>
    <stub layout='horizon, inverse' align='rightTop' id='buttons'>
        <lable text='×' margin='*2'></lable>
        <lable text='□'></lable>
        <lable text='_'></lable>
    </stub>

    <blank layout='horizon, shrink' margin='4' id='head title'>
        <round_rect size='64'>
            <lable text='♛' align='center' color='gold' size='32'></lable>
        </round_rect>
        <blank layout='vertical, shrink'>
            <lable text='YAUI交流讨论群' size='14' style='bold'></lable>
            <lable text='(1000人群)' size='10'></lable>
        </blank>
    </blank>

    <rect height='48' layout='expandX' id='tabs_advs'>
        <blank layout='horizon' align='leftBottom' height='32' id='tabs'>
            <innerTemplate name='tab'>
                <round_rect layout='horizon, expandY' width='96' fillColor='royalblue' leftBottomCorner='false' rightBottomCorner='false'>
                    <lable name='lb' text='聊天' align='center'></lable>
                </round_rect>
            </innerTemplate>
            <apply innerTemplate='tab' lb.text='聊天' fillColor='blue'></apply>
            <apply innerTemplate='tab' lb.text='相片'></apply>
            <apply innerTemplate='tab' lb.text='文件'></apply>
        </blank>
        <blank layout='horizon, inverse' align='rightBottom' height='48' id='advs'>
            <round_rect layout='horizon, expandY' width='128'>
                <lable text='广告位1' align='center'></lable>
            </round_rect>
        </blank>
    </rect>

    <blank id='leftRight' layout='horizon, inverse, filled' >
        <round_rect layout='vertical, expandY' margin='6' width='180' id='frind_list'>
            <round_rect layout='expandX' height='28'>
                <lable text='群成员' color='black' size='8' align='left'></lable>
                <lable text='?' align='right'></lable>
            </round_rect>
            <lable text='西门吹雪' margin='*4' color='*black' size='*8'></lable>
            <lable text='聂风'></lable>
            <lable text='步惊云'></lable>
            <lable text='武林神话'></lable>
        </round_rect>

        <rect layout='vertical, inverse, expandY, filled' clip='true'>
            <rect layout='expandX' height='28'>
                <lable text='广告位2' align='left' size='8'></lable>
                <blank align='right' layout='horizon, inverse, expand'>
                    <innerTemplate name='bt'>
                        <round_rect layout='expandY' margin='4'> 
                            <lable name='lb' text='' align='center' size='8' color='black'></lable> 
                        </round_rect>
                    </innerTemplate>
                    <apply innerTemplate='bt' lb.text='发送'> </apply>
                    <apply innerTemplate='bt' lb.text='关闭'> </apply>
                    
                </blank>
            </rect>
            <rect layout='expandX' height='128' color='white'>
            </rect>
            <rect layout='horizon, expandX' height='32' padding='4' clip='true'>
                <lable text='A' color='blue' style='*bold'></lable>                
                <lable text='☺' size='18' offsetY='-8' color='yellow'></lable>
                <lable text='♪' color='red'></lable>
            </rect>
            <rect layout='vertical, expandX' clip='true' padding='4' color='white' id='chat'>
                <lable text='西门吹雪: xxxxx' color='*blue'></lable>
                <lable text='聂风: xxxxx'></lable>
                <lable text='步惊云: xxxxx'></lable>
                <lable text='武林神话: xxxxx'></lable>
            </rect>
        </rect>
    </blank>
</resizer>
";
        public testLayout()
        {
            UIRoot.Instance.root.appendFromXML(XMLPAGE);

        }
    }
}
