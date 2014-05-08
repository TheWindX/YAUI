YAUI（Yet Another UI), 是一个新思路GUI系统, 它参考了一些其它UI(特别是html)的设计，和我的一些新思路：

以下是它的特点:

1. 较完备的layout, 我发觉GUI最麻烦和重要的部分，其实是排版和布局，我们看到的网页，复杂的编辑器界面，困难就在此;

2. 我用控件模板和属性继承的提供类似css的功能;

3. 基础性，组合性，并不直接提供高级控件(目前)，而是最基础的 stub, map, rect，lable(等基础绘制图元)，容易地组合出高级控件;

4. 可捡图元对象和变换，这个可用于提供可视化对象的编辑功能，(比如说实现类似于Maya的 节点图，这是这个UI实现的最初动机);

5. XML layout desc, 实用的query，消息绑定，因此UI的一般使用方法是，
      var ui = UI.loadXML(strXML);                   //创建
      UILabel lable = ui.findByID(id) as UILable;  //query
      lable.text = "click me";                             //属性
      lable.evtClick = ()=>print("i am touched!");//事件
