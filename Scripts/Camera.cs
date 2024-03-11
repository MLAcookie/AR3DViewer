using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Godot;
using OpenCvSharp;

namespace OpenCVTest;

public partial class Camera : Sprite2D
{
    VideoCapture camera;
    Mat image = new Mat();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        camera = new VideoCapture(0, VideoCaptureAPIs.ANY)
        {
            Fps = 20,
            FrameWidth = 1280,
            FrameHeight = 720,
        };
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        camera.Read(image); //获取图像
        if (image.Empty())
        {
            Debug.WriteLine("NoImage");
            return;
        }
        var bytes = image.ToBytes(); //图像转byte
        Image img = new(); //实例化godot Image节点
        img.LoadPngFromBuffer(bytes); //装载图像
        Texture = ImageTexture.CreateFromImage(img);
    }
}
