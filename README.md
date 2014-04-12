YAUI(Yeah Another implement of gUI)
的实现了一套类似于html的 UI系统，但有着更好的排版系统(advance layout)，和简单性，提倡通过简单的矢量图形与排版系统构造可用的GUI——（不特意实现复杂控件)
如：                   

&lt;rect shrink='true' clip='*true' padding='5' fillColor='blue' layout='vertical'&gt;                                                                           
    &lt;lable align='leftTop' text='标题'&gt;&lt;lable&gt;                                                                  
    &lt;lable align='rightTop' text='x'&gt;&lt;lable&gt;                                                                  
    &lt;llank length='30'&gt;lt;blank&gt;                                                                  
    &lt;resizer length='512' clip='true'&gt;&lt;resizer&gt; 
&lt;rect&gt;                         

模拟绘制一个windows上标准的工具窗口.

query & 事件绑定：
root.childOfPath("button").evtOnLeftDown
  += (ui, x, y)
    =>print("i am touched");


实现了可捡(pick, edit)图形的交互(除控件外的图形化object编辑.
如:
<round dragAble='true'></round>
表示一个UI上可捡拖拽的圆


YART
目标是构造类型系统，在正在进行的YAUI的基础上图形化表示
