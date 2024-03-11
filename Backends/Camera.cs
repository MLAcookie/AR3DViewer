using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using OpenCvSharp;

namespace OpencvTest.Backends;

public class Camera
{
    readonly VideoCapture camera;
    readonly Mat frame = new();

    public bool IsOpened
    {
        get { return camera.IsOpened(); }
    }

    public double FPS
    {
        get { return camera.Fps; }
        set { camera.Fps = value; }
    }
    public int FrameWidth
    {
        get { return camera.FrameWidth; }
        set { camera.FrameWidth = value; }
    }
    public int FrameHight
    {
        get { return camera.FrameHeight; }
        set { camera.FrameHeight = value; }
    }

    public Camera(int index = 0, VideoCaptureAPIs captureAPI = VideoCaptureAPIs.ANY)
    {
        camera = new VideoCapture(index, captureAPI);
    }

    public Mat GetNextRaw()
    {
        camera.Read(frame);
        return frame;
    }

    public ImageTexture GetImageTexture()
    {
        camera.Read(frame);
        if (frame.Empty())
        {
            Debug.WriteLine("No Image");
            return null;
        }
        var bytes = frame.ToBytes();
        Image img = new();
        img.LoadPngFromBuffer(bytes);
        return ImageTexture.CreateFromImage(img);
    }
}
