YAUI
----

YAUI（**Y** et **A** nother **UI**), 是一个新思路GUI系统, 它参考了传统GUI和HTML的设计

以下是它的特点:

- 较完备的**layout**, GUI比较麻烦的地方，在于排版布局，YAUI的布局系统类似html;
- **组合性**，并不直接提供高级控件(目前)，而是基础绘制图元，通过控件模板容易地组合出高级控件，属性继承的提供类似css的功能;
- **可捡图元**对象和变换，可用于提供可视化对象的编辑功能;
- **XML** layout desc, 使用**query**, 事件绑定;
    
代码示例;

```
  var strXML = @"
    <div layout='vertical, shrink'>
      <label id='lb' text='touch me!'><label>
      <label id='lb2' text='touch me again!'><label>
      <label id='lb3' text='donnot touch me!'><label>
    </div>";                                //layout 
```
    
```
  var ui = UI.loadXML(strXML);                 
  var lb = ui.findByID('lb1') as UILable;     //query
  lb.text = "touch me!";                     //属性
  lb.evtClick = ()=>print("i am touched!");  //事件
```

demo
----
[download][1]

![demo pic][21]
![demo pic][3]
![demo pic][4]
![demo pic][5]



todo list
----
- more feathers, 位图支持, 渐变色填充，xml定义控件，预置控件(window style)...
- 修改dirty rect 实现, 优化;
- (visula) runtime, demo.
- 渲染支持，render by cairo or direct2D，direct3D/openGL/opengl es?
- layered window? app UI system?
- canvas(in html5) version?
- c++ & scripts version?



Version
----

0.1




License
----

MIT


me
----
> lxf0525@gmail.com

> 453588006@qq.com
[1]:https://raw.githubusercontent.com/TheWindX/YAUI/master/demo.zip
[2]:https://raw.githubusercontent.com/TheWindX/YAUI/master/doc/demo2.png
[21]:https://raw.githubusercontent.com/TheWindX/YAUI/master/doc/demo21.png
[3]:https://raw.githubusercontent.com/TheWindX/YAUI/master/doc/demo3.png
[4]:https://raw.githubusercontent.com/TheWindX/YAUI/master/doc/demo4.png
[5]:https://raw.githubusercontent.com/TheWindX/YAUI/master/doc/demo5.png
    
