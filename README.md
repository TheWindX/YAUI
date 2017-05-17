YAUI
----

YAUI（**Y** et **A** nother **UI**), 是一个新思路的GUI系统, 它参考了传统GUI和HTML的设计

以下是它的特点:

- **layout**, GUI比较麻烦的地方，在于排版布局，YAUI的布局系统类似HTML;
- **组合性**，并不直接提供高级控件(目前)，而是基础图元(div, rect, round, map等)，通过控件模板容易地组合出高级控件，属性继承的提供类似css的功能;
- **可捡图元**对象和变换，可用于提供可视化对象的编辑功能;
- **XML** layout desc, 使用**query**, 事件绑定;
- YAUI,可用做传统UI(如编辑器),也适合做平面化设计;

代码示例;

```
  var strXML = @"
    <div layout='vertical, shrink'>
      <label id='lb1' text='touch me!'><label>
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
[download](https://raw.githubusercontent.com/TheWindX/YAUI/master/demo.zip)


![21](https://raw.githubusercontent.com/TheWindX/YAUI/master/doc/demo21.png)


![3](https://raw.githubusercontent.com/TheWindX/YAUI/master/doc/demo3.png)


![31](https://raw.githubusercontent.com/TheWindX/YAUI/master/doc/demo31.png)


![4](https://raw.githubusercontent.com/TheWindX/YAUI/master/doc/demo4.png)


![5](https://raw.githubusercontent.com/TheWindX/YAUI/master/doc/demo5.png)


todo list
----
(implement in html canvas)?

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
