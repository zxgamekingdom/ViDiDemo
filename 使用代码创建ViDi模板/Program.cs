using System;
using System.Collections.Generic;
using ViDi2;
using ViDi2.Training.Local;
using Control = ViDi2.Training.Local.Control;
using IRedTool = ViDi2.Training.IRedTool;

namespace 使用代码创建ViDi模板;

public static class Program
{
    public static void Main(string[] args)
    {
        // var dir = @"C:\Users\admin\Desktop\新建文件夹 (2)";
        // var directory = new WorkspaceDirectory
        // {
        //     Path = @"C:\Users\admin\Desktop\新建文件夹 (3) - 副本\"
        // };
        // var libraryAccess = new LibraryAccess(directory);
        // var control = new Control(libraryAccess, GpuMode.SingleDevicePerTool, new List<int>());
        // foreach (var workspace in control.Workspaces)
        // {
        //     workspace.DisplayName.WriteLine();
        //     workspace.Open();
        //     foreach (var stream in workspace.Streams)
        //     {
        //         stream.Name.WriteLine();
        //         foreach (var redTool in stream.Tools.OfType<IRedTool>())
        //         {
        //             var database = redTool.Database;
        //             foreach (var key in database.List())
        //             {
        //                 database.GetImage(key).Bitmap.Save(Path.Combine(dir, $"{key.SampleName}.bmp"), ImageFormat.Bmp);
        //                 database.GetRegionsImage(key, "defect")?.Bitmap
        //                     ?.Save(Path.Combine(dir, $"{key.SampleName}_defect.bmp"), ImageFormat.Bmp);
        //             }
        //         }
        //     }
        // }
        var directory = new WorkspaceDirectory
        {
            Path = @"C:\Users\admin\Desktop\新建文件夹 (2)\新建文件夹"
        };
        var libraryAccess = new LibraryAccess(directory);
        var control = new Control(libraryAccess, GpuMode.SingleDevicePerTool, new List<int>());
        var workspace = control.Workspaces["测试"];
        workspace.Open();
        var stream = workspace.Streams["默认"];
        var redTool = stream.Tools["分析"] as IRedTool;
        foreach (var key in redTool.Database.List())
        {
            redTool.Database.SetViewMask($"'{key.SampleName}'", new FormsImage(
                @"C:\Users\admin\Desktop\新建文件夹 (2)\235.bmp"
            ));
            redTool.Database.SetRegionsImage(key, "defect",
                new FormsImage(@"C:\Users\admin\Desktop\新建文件夹 (2)\233.bmp"));
        }

        redTool.Wait();
        workspace.Save();
    }
}

public static class ConsoleExtensions
{
    public static void WriteLine<T>(this T t)
    {
        Console.WriteLine(t);
    }
}