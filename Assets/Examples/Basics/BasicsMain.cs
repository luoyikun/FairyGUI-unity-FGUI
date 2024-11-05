using UnityEngine;
using System.Collections.Generic;
using FairyGUI;

public class BasicsMain : MonoBehaviour
{
    private GComponent _mainView; //主面板 相当与
    private GObject _backBtn; //返回按钮
    private GComponent _demoContainer; //内容包含
    private Controller _viewController; //
    private Dictionary<string, GComponent> _demoObjects;

    public Gradient lineGradient;

    void Awake()
    {
#if (UNITY_5 || UNITY_5_3_OR_NEWER)
        //Use the font names directly
        UIConfig.defaultFont = "Microsoft YaHei";
#else
        //Need to put a ttf file into Resources folder. Here is the file name of the ttf file.
        UIConfig.defaultFont = "afont";
#endif
        UIPackage.AddPackage("UI/Basics");

        UIConfig.verticalScrollBar = "ui://Basics/ScrollBar_VT";
        UIConfig.horizontalScrollBar = "ui://Basics/ScrollBar_HZ";
        UIConfig.popupMenu = "ui://Basics/PopupMenu";
        UIConfig.buttonSound = (NAudioClip)UIPackage.GetItemAsset("Basics", "click");
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        Stage.inst.onKeyDown.Add(OnKeyDown);
        //获取到UIPanel
        _mainView = this.GetComponent<UIPanel>().ui;
        //获取到back按钮
        _backBtn = _mainView.GetChild("btn_Back");
        _backBtn.visible = false;
        _backBtn.onClick.Add(onClickBack);

        //获取到包含组
        _demoContainer = _mainView.GetChild("container").asCom;
        //c1控制器
        _viewController = _mainView.GetController("c1");

        _demoObjects = new Dictionary<string, GComponent>();
        //获取全部子节点
        int cnt = _mainView.numChildren;
        //遍历子物体
        for (int i = 0; i < cnt; i++)
        {
            GObject obj = _mainView.GetChildAt(i);
            //找到btns组下的，组只是逻辑概念，不会映射出GameObejct在Unity中
            if (obj.group != null && obj.group.name == "btns")
                obj.onClick.Add(runDemo);
        }
    }

    private void runDemo(EventContext context)
    {
        //点击有发送者名字
        string type = ((GObject)(context.sender)).name.Substring(4);
        GComponent obj;
        if (!_demoObjects.TryGetValue(type, out obj))
        {
            //创建GObject
            obj = UIPackage.CreateObject("Basics", "Demo_" + type).asCom;
            _demoObjects[type] = obj;
        }
        //移除
        _demoContainer.RemoveChildren();
        _demoContainer.AddChild(obj);
        _viewController.selectedIndex = 1;
        _backBtn.visible = true;

        switch (type)
        {
            case "Graph":
                PlayGraph();
                break;

            case "Button":
                PlayButton();
                break;

            case "Text":
                PlayText();
                break;

            case "Grid":
                PlayGrid();
                break;

            case "Transition":
                PlayTransition();
                break;

            case "Window":
                PlayWindow();
                break;

            case "Popup":
                PlayPopup();
                break;

            case "Drag&Drop":
                PlayDragDrop();
                break;

            case "Depth":
                PlayDepth();
                break;

            case "ProgressBar":
                PlayProgressBar();
                break;
        }
    }

    private void onClickBack()
    {
        _viewController.selectedIndex = 0;
        _backBtn.visible = false;
    }

    void OnKeyDown(EventContext context)
    {
        if (context.inputEvent.keyCode == KeyCode.Escape)
        {
            Application.Quit();
        }
    }

    //-----------------------------
    //绘图
    private void PlayGraph()
    {
        GComponent obj = _demoObjects["Graph"];

        Shape shape;
        //设置圆的角度，例如缺一角显示
        shape = obj.GetChild("pie").asGraph.shape;
        EllipseMesh ellipse = shape.graphics.GetMeshFactory<EllipseMesh>();
        ellipse.startDegree = 30;
        ellipse.endDegreee = 300;
        shape.graphics.SetMeshDirty();

        //方块
        //修改方块的顶点，从正方形变为梯形
        shape = obj.GetChild("trapezoid").asGraph.shape;
        PolygonMesh trapezoid = shape.graphics.GetMeshFactory<PolygonMesh>();
        trapezoid.usePercentPositions = true;
        trapezoid.points.Clear();
        trapezoid.points.Add(new Vector2(0f, 1f));
        trapezoid.points.Add(new Vector2(0.3f, 0));
        trapezoid.points.Add(new Vector2(0.7f, 0));
        trapezoid.points.Add(new Vector2(1f, 1f));
        trapezoid.texcoords.Clear();
        trapezoid.texcoords.AddRange(VertexBuffer.NormalizedUV);
        shape.graphics.SetMeshDirty();
        //给shape载入一张图
        shape.graphics.texture = (NTexture)UIPackage.GetItemAsset("Basics", "change");

        //线
        //由直线变为爱心交叉曲线
        shape = obj.GetChild("line").asGraph.shape;
        LineMesh line = shape.graphics.GetMeshFactory<LineMesh>();
        line.lineWidthCurve = AnimationCurve.Linear(0, 25, 1, 10);
        line.roundEdge = true;
        line.gradient = lineGradient;
        line.path.Create(new GPathPoint[] {
            new GPathPoint(new Vector3(0, 120, 0)),
            new GPathPoint(new Vector3(20, 120, 0)),
            new GPathPoint(new Vector3(100, 100, 0)),
            new GPathPoint(new Vector3(180, 30, 0)),
            new GPathPoint(new Vector3(100, 0, 0)),
            new GPathPoint(new Vector3(20, 30, 0)),
            new GPathPoint(new Vector3(100, 100, 0)),
            new GPathPoint(new Vector3(180, 120, 0)),
            new GPathPoint(new Vector3(200, 120, 0)),
        });
        shape.graphics.SetMeshDirty();
        GTween.To(0, 1, 5).SetEase(EaseType.Linear).SetTarget(shape.graphics).OnUpdate((GTweener t) =>
        {
            ((NGraphics)t.target).GetMeshFactory<LineMesh>().fillEnd = t.value.x;
            ((NGraphics)t.target).SetMeshDirty();
        });

        //线
        //由直线变为折线
        shape = obj.GetChild("line2").asGraph.shape;
        LineMesh line2 = shape.graphics.GetMeshFactory<LineMesh>();
        line2.lineWidth = 3;
        line2.roundEdge = true;
        line2.path.Create(new GPathPoint[] {
            new GPathPoint(new Vector3(0, 120, 0), GPathPoint.CurveType.Straight),
            new GPathPoint(new Vector3(60, 30, 0), GPathPoint.CurveType.Straight),
            new GPathPoint(new Vector3(80, 90, 0), GPathPoint.CurveType.Straight),
            new GPathPoint(new Vector3(140, 30, 0), GPathPoint.CurveType.Straight),
            new GPathPoint(new Vector3(160, 90, 0), GPathPoint.CurveType.Straight),
            new GPathPoint(new Vector3(220, 30, 0), GPathPoint.CurveType.Straight)
        });
        shape.graphics.SetMeshDirty();

        //线
        //由直线变为正弦曲线
        GObject image = obj.GetChild("line3");
        LineMesh line3 = image.displayObject.graphics.GetMeshFactory<LineMesh>();
        line3.lineWidth = 30;
        line3.roundEdge = false;
        line3.path.Create(new GPathPoint[] {
            new GPathPoint(new Vector3(0, 30, 0), new Vector3(50, -30), new Vector3(150, -50)),
            new GPathPoint(new Vector3(200, 30, 0), new Vector3(300, 130)),
            new GPathPoint(new Vector3(400, 30, 0))
        });
        image.displayObject.graphics.SetMeshDirty();
    }

    //-----------------------------
    private void PlayButton()
    {
        GComponent obj = _demoObjects["Button"];
        obj.GetChild("n34").onClick.Add(() => { UnityEngine.Debug.Log("click button"); });
    }

    //------------------------------
    //文本控制
    private void PlayText()
    {
        GComponent obj = _demoObjects["Text"];
        obj.GetChild("n12").asRichTextField.onClickLink.Add((EventContext context) =>
        {
            //一个富文本对应多个超链，href后面字符串是超链的内容
            GRichTextField t = context.sender as GRichTextField;
            t.text = "[img]ui://Basics/pet[/img][color=#FF0000]You click the link[/color]：" + context.data;
        });
        obj.GetChild("n25").onClick.Add(() =>
        {
            obj.GetChild("n24").text = obj.GetChild("n22").text;
        });
    }

    //------------------------------
    private void PlayGrid()
    {
        GComponent obj = _demoObjects["Grid"];
        GList list1 = obj.GetChild("list1").asList;
        list1.RemoveChildrenToPool();
        string[] testNames = System.Enum.GetNames(typeof(RuntimePlatform));
        Color[] testColor = new Color[] { Color.yellow, Color.red, Color.white, Color.cyan };
        int cnt = testNames.Length;
        for (int i = 0; i < cnt; i++)
        {
            //item是GComponent
            GButton item = list1.AddItemFromPool().asButton;
            item.GetChild("t0").text = "" + (i + 1);
            item.GetChild("t1").text = testNames[i];
            item.GetChild("t2").asTextField.color = testColor[UnityEngine.Random.Range(0, 4)];
            item.GetChild("star").asProgress.value = (int)((float)UnityEngine.Random.Range(1, 4) / 3f * 100);
        }

        GList list2 = obj.GetChild("list2").asList;
        list2.RemoveChildrenToPool();
        for (int i = 0; i < cnt; i++)
        {
            GButton item = list2.AddItemFromPool().asButton;
            item.GetChild("cb").asButton.selected = false;
            item.GetChild("t1").text = testNames[i];
            item.GetChild("mc").asMovieClip.playing = i % 2 == 0;
            item.GetChild("t3").text = "" + UnityEngine.Random.Range(0, 10000);
        }
    }

    //------------------------------
    private void PlayTransition()
    {
        GComponent obj = _demoObjects["Transition"];
        obj.GetChild("n2").asCom.GetTransition("t0").Play(int.MaxValue, 0, null);
        obj.GetChild("n3").asCom.GetTransition("peng").Play(int.MaxValue, 0, null);

        obj.onAddedToStage.Add(() =>
        {
            obj.GetChild("n2").asCom.GetTransition("t0").Stop();
            obj.GetChild("n3").asCom.GetTransition("peng").Stop();
        });
    }

    //------------------------------
    private Window _winA;
    private Window _winB;
    private void PlayWindow()
    {
        GComponent obj = _demoObjects["Window"];
        obj.GetChild("n0").onClick.Add(() =>
        {
            if (_winA == null)
                _winA = new Window1();
            _winA.Show();
        });

        obj.GetChild("n1").onClick.Add(() =>
        {
            if (_winB == null)
                _winB = new Window2();
            _winB.Show();
        });
    }

    //------------------------------
    private PopupMenu _pm;
    private GComponent _popupCom;
    private void PlayPopup()
    {
        if (_pm == null)
        {
            _pm = new PopupMenu();
            GButton btn = _pm.AddItem("Item 1", __clickMenu);
            btn.data = 123;
            _pm.AddItem("Item 2", __clickMenu);
            _pm.AddItem("Item 3", __clickMenu);
            _pm.AddItem("Item 4", __clickMenu);
        }

        if (_popupCom == null)
        {
            _popupCom = UIPackage.CreateObject("Basics", "Component12").asCom;
            _popupCom.Center();
        }
        GComponent obj = _demoObjects["Popup"];
        obj.GetChild("n0").onClick.Add((EventContext context) =>
        {
            _pm.Show((GObject)context.sender, PopupDirection.Down);
        });

        obj.GetChild("n1").onClick.Add(() =>
        {
            //默认出现在鼠标位置，挂载在Stage/GRoot/Component12路径下
            GRoot.inst.ShowPopup(_popupCom);
        });


        obj.onRightClick.Add(() =>
        {
            _pm.Show();
        });
    }

    private void __clickMenu(EventContext context)
    {
        GObject itemObject = (GObject)context.data;
        UnityEngine.Debug.Log("click " + itemObject.text);

        if (itemObject.data != null)
        {
            Debug.Log($"item.data={itemObject.data}");
        }
    }

    //------------------------------
    Vector2 startPos;
    private void PlayDepth()
    {
        GComponent obj = _demoObjects["Depth"];
        GComponent testContainer = obj.GetChild("n22").asCom;
        GObject fixedObj = testContainer.GetChild("n0");
        fixedObj.sortingOrder = 100;
        fixedObj.draggable = true;

        int numChildren = testContainer.numChildren;
        int i = 0;
        while (i < numChildren)
        {
            GObject child = testContainer.GetChildAt(i);
            if (child != fixedObj)
            {
                testContainer.RemoveChildAt(i);
                numChildren--;
            }
            else
                i++;
        }
        startPos = new Vector2(fixedObj.x, fixedObj.y);

        obj.GetChild("btn0").onClick.Add(() =>
        {
            GGraph graph = new GGraph();
            startPos.x += 10;
            startPos.y += 10;
            graph.xy = startPos;
            graph.DrawRect(150, 150, 1, Color.black, Color.red);
            obj.GetChild("n22").asCom.AddChild(graph);
        });

        obj.GetChild("btn1").onClick.Add(() =>
        {
            GGraph graph = new GGraph();
            startPos.x += 10;
            startPos.y += 10;
            graph.xy = startPos;
            graph.DrawRect(150, 150, 1, Color.black, Color.green);
            graph.sortingOrder = 200;
            obj.GetChild("n22").asCom.AddChild(graph);
        });
    }

    //------------------------------
    private void PlayDragDrop()
    {
        GComponent obj = _demoObjects["Drag&Drop"];
        obj.GetChild("a").draggable = true;

        GButton b = obj.GetChild("b").asButton;
        b.draggable = true;
        b.onDragStart.Add((EventContext context) =>
        {
            //Cancel the original dragging, and start a new one with a agent.
            context.PreventDefault(); //注释这句，整个图标都会被拖动。使用这句创建个虚影

            DragDropManager.inst.StartDrag(b, b.icon, b.icon, (int)context.data);
        });

        GButton c = obj.GetChild("c").asButton;
        c.icon = null;
        //onDrop有GObject被拖入组件内
        c.onDrop.Add((EventContext context) =>
        {
            c.icon = (string)context.data;
        });

        GObject bounds = obj.GetChild("n7");
        Rect rect = bounds.TransformRect(new Rect(0, 0, bounds.width, bounds.height), GRoot.inst);
        //rect x:-503,y:146,width:440,height:413
        
        //---!!Because at this time the container is on the right side of the stage and beginning to move to left(transition), so we need to caculate the final position
        //container在右边
        rect.x -= obj.parent.x;
        //rect.x = 640
        //----

        GButton d = obj.GetChild("d").asButton;
        d.draggable = true;
        //禁止拖出边界
        d.dragBounds = rect;
    }

    //------------------------------
    private void PlayProgressBar()
    {
        GComponent obj = _demoObjects["ProgressBar"];
        Timers.inst.Add(0.001f, 0, __playProgress);
        obj.onRemovedFromStage.Add(() => { Timers.inst.Remove(__playProgress); });
    }

    void __playProgress(object param)
    {
        GComponent obj = _demoObjects["ProgressBar"];
        int cnt = obj.numChildren;
        for (int i = 0; i < cnt; i++)
        {
            GProgressBar child = obj.GetChildAt(i) as GProgressBar;
            if (child != null)
            {

                child.value += 1;
                if (child.value > child.max)
                    child.value = 0;
            }
        }
    }
}