using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using HalconDotNet;
using ViDi2;
using Control = ViDi2.Runtime.Local.Control;
using IControl = ViDi2.Runtime.IControl;
using IRedTool = ViDi2.Runtime.IRedTool;
using IStream = ViDi2.Runtime.IStream;
using IWorkspace = ViDi2.Runtime.IWorkspace;

namespace 执行ViDi模板;

internal static class Program
{
    private static void Main(string[] args)
    {
        HImage hImage = new HImage("byte", 10000, 10000);
        IControl control = new Control();
        //初始化显卡
        control.InitializeComputeDevices(GpuMode.SingleDevicePerTool, new List<int>());
        //加载工作空间,加载运行时模板文件
        IWorkspace workspace = control.Workspaces.Add("六面检", @"C:\Users\admin\Desktop\六面检.vrws");
        //提取模板中的流
        IStream stream = workspace.Streams["默认"];
        string filename =
            @"C:\Users\admin\Desktop\无为外观检图片\0627\Convert NG\Camera1_SourceImage_DXF07C02FDSBC106126_NG_133000114324027214.tif.png";
        Bitmap bitmap =
            new Bitmap(
                filename);
        //设置工具参数
        IRedTool redTool = (IRedTool)stream.Tools["Analyze保护片残留"];
        redTool.Parameters.Threshold = new Interval(.1, .1);
        //创建ViDi图片
        FormsImage formsImage = new FormsImage(bitmap);
        //创建ViDi运行样本
        using ISample sample = stream.CreateSample(formsImage);
        //样本运行
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        sample.Process();
        stopwatch.Stop();
        Console.WriteLine($"运行时间:{stopwatch.Elapsed.TotalMilliseconds}ms");
        //获取样本对应工具的结果
        IRedMarking redMarking = (IRedMarking)sample.Markings["Analyze保护片残留"];
        List<List<(double X, double Y)>> outerList = new List<List<(double X, double Y)>>();
        foreach (IRedView redView in redMarking.Views)
        foreach (IRegion redViewRegion in redView.Regions)
            outerList.Add(redViewRegion.Outer.Select(point => (point.X, point.Y)).ToList());
        HWindow hWindow = new HWindow(0, 0, 1280, 720, IntPtr.Zero, "visible", "");
        hWindow.SetColored(12);
        HImage image = new HImage(filename);
        hWindow.DispImage(image);
        foreach (List<(double X, double Y)> tuples in outerList)
        {
            double[] cols = tuples.Select(x => x.X).ToArray();
            double[] rows = tuples.Select(x => x.Y).ToArray();
            if (cols.Length > 0 && rows.Length > 0)
            {
                HXLDCont hxldCont = new HXLDCont(rows, cols);
                hWindow.DispXld(hxldCont);
            }
        }

        hWindow.SetPart(0, 0, -2, -2);
        HSystem.WaitSeconds(double.MaxValue);
        hWindow.CloseWindow();
    }
}