Yeah Another implement of gUI, for data model visualization


YAUI的实现了一套类似于html的 UI系统，但有着更好的排版系统(advance layout)，和简单性，提倡通过简单的矢量图形与排版系统构造可用的GUI——（不特意实现复杂控件)

如：
<rect shrink='true' clip='*true' padding='5' fillColor='blue' layout='vertical'>
    <lable align='leftTop' text='标题'></lable>
    <lable align='rightTop' text='x'></lable>
    <blank length='30'></blank>
    <resizer length='512' clip='true'></resizer>
</rect>
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
目标是构造类型系统，不容易对应普遍的编程语言(缺少algebraic data type,  但在haskell和ML是类型基础),可在正在进行的YAUI的基础上图形化表示
