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
        <label text='×' margin='*2'></label>
        <label text='□'></label>
        <label text='_'></label>
    </stub>

    <div layout='horizon, shrink' margin='4' id='head title'>
        <round_rect size='64'>
            <label text='♛' align='center' color='gold' size='32'></label>
        </round_rect>
        <div layout='vertical, shrink'>
            <label text='YAUI交流讨论群' size='14' style='bold'></label>
            <label text='(1000人群)' size='10'></label>
        </div>
    </div>

    <rect height='48' layout='expandX' id='tabs_advs'>
        <div layout='horizon' align='leftBottom' height='32' id='tabs'>
            <innerTemplate name='tab'>
                <round_rect layout='horizon, expandY' width='96' fillColor='royalblue' leftBottomCorner='false' rightBottomCorner='false'>
                    <label name='lb' text='聊天' align='center'></label>
                </round_rect>
            </innerTemplate>
            <apply innerTemplate='tab' lb.text='聊天' fillColor='blue'></apply>
            <apply innerTemplate='tab' lb.text='相片'></apply>
            <apply innerTemplate='tab' lb.text='文件'></apply>
        </div>
        <div layout='horizon, inverse' align='rightBottom' height='48' id='advs'>
            <round_rect layout='horizon, expandY' width='128'>
                <label text='广告位1' align='center'></label>
            </round_rect>
        </div>
    </rect>

    <div id='leftRight' layout='horizon, inverse, filled' >
        <round_rect layout='vertical, expandY' margin='6' width='180' id='frind_list'>
            <round_rect layout='expandX' height='28'>
                <label text='群成员' color='black' size='8' align='left'></label>
                <label text='?' align='right'></label>
            </round_rect>
            <label text='西门吹雪' margin='*4' color='*black' size='*8'></label>
            <label text='聂风'></label>
            <label text='步惊云'></label>
            <label text='武林神话'></label>
        </round_rect>

        <rect layout='vertical, inverse, expandY, filled' clip='true'>
            <rect layout='expandX' height='28'>
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
            <rect layout='horizon, expandX' height='32' padding='4' clip='true'>
                <label text='A' color='blue' style='*bold'></label>                
                <label text='☺' size='18' offsetY='-8' color='yellow'></label>
                <label text='♪' color='red'></label>
            </rect>
            <rect layout='vertical, expandX' clip='true' padding='4' color='white' id='chat'>
                <label text='西门吹雪: xxxxx' color='*blue'></label>
                <label text='聂风: xxxxx'></label>
                <label text='步惊云: xxxxx'></label>
                <label text='武林神话: xxxxx'></label>
            </rect>
        </rect>
    </div>
</resizer>
";
        public testLayout()
        {
            UIRoot.Instance.root.appendFromXML(XMLPAGE);

        }
    }
}
