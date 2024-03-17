using System.Diagnostics;
using Godot;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using OpencvTest.Backends;

namespace OpenCVTest;

public partial class CameraBehavior : Sprite2D
{
    [Export]
    public Node3D node3D;
    Camera3D camera3D;
    Camera camera =
        new(0, VideoCaptureAPIs.DSHOW)
        {
            FPS = 30,
            FrameHight = 720,
            FrameWidth = 1280,
        };

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        camera3D = node3D.GetNode<Camera3D>("Camera3D");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Mat temp = camera.GetNextRaw();
        ArucoDetectedInfo arucoDetectedInfo = ArucoDetect.DetectArucoMarkers(
            temp,
            4,
            CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict7X7_1000)
        );
        CameraCalibrationInfo cameraCalibrationInfo = ArucoDetect.EstimatePoseSingleMarkers(
            arucoDetectedInfo,
            .09f
        );
        Texture = temp.DrawAllMarkerInfo(arucoDetectedInfo)
            .DrawAllAxis(cameraCalibrationInfo)
            .ToImageTexture();
        TransPair tp = ArucoDetect.GetTransPair(arucoDetectedInfo, cameraCalibrationInfo);

        if (tp is not null)
        {
            camera3D.Position = new Vector3(
                (float)tp.transform[0],
                (float)tp.transform[1] + 1f,
                (float)tp.transform[2]
            );
            camera3D.Rotation = new Vector3(
                (float)tp.rotation[0],
                (float)tp.rotation[1],
                (float)tp.rotation[2]
            );
        }
        Debug.WriteLine(camera3D.Position);
    }
}
