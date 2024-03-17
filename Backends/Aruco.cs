using System;
using Godot;
using OpenCvSharp;
using OpenCvSharp.Aruco;

namespace OpencvTest.Backends;

public class TransPair
{
    public Vec3d transform;
    public Vec3d rotation;

    public TransPair(Vec3d transform, Vec3d rotation)
    {
        this.transform = transform;
        this.rotation = rotation;
    }
}

public class ArucoDetectedInfo
{
    public Point2f[][] corners;
    public Point2f[][] rejected;
    public int[] ids;
    public DetectorParameters parameters;

    public ArucoDetectedInfo(int size)
    {
        corners = PointHelper.NewPoint2fMatrix(size, 4);
        rejected = PointHelper.NewPoint2fMatrix(size, 4);
        ids = new int[size];
        parameters = new DetectorParameters();
    }
}

public class CameraCalibrationInfo
{
    // Iphone13的标定数据
    public Mat cameraMatrix =
        new(
            3,
            3,
            MatType.CV_64F,
            new double[]
            {
                2.22233333e+03,
                0.0,
                1.52536364e+03,
                0.0,
                2.22455556e+03,
                1.14799795e+03,
                0.0,
                0.0,
                1.0
            }
        );
    public Mat distCoeffs =
        new(
            1,
            5,
            MatType.CV_64F,
            new double[] { -0.24431426, 0.12126514, -0.00231412, -0.00021857, 0.00000000 }
        );

    public Mat rvecs = new(3, 1, MatType.CV_64F, new double[] { 0.0, 0.0, 0.0 });
    public Mat tvecs = new(3, 1, MatType.CV_64F, new double[] { 0.0, 0.0, 0.0 });
}

public static class ArucoDraw
{
    public static Mat DrawAllMarkerInfo(this Mat image, ArucoDetectedInfo info)
    {
        CvAruco.DrawDetectedMarkers(image, info.corners, info.ids);
        return image;
    }

    public static Mat DrawAllAxis(this Mat image, CameraCalibrationInfo info, float length = 0.05f)
    {
        for (int i = 0; i < info.rvecs.Rows; i++)
        {
            Cv2.DrawFrameAxes(
                image,
                info.cameraMatrix,
                info.distCoeffs,
                info.rvecs.Get<Vec3d>(i),
                info.tvecs.Get<Vec3d>(i),
                length
            );
        }
        return image;
    }
}

public class ArucoDetect
{
    public static ArucoDetectedInfo DetectArucoMarkers(Mat image, int size, Dictionary dictionary)
    {
        ArucoDetectedInfo detectedInfo = new(size);
        Mat grayImage = new();
        Cv2.CvtColor(image, grayImage, ColorConversionCodes.RGB2GRAY);

        CvAruco.DetectMarkers(
            grayImage,
            dictionary,
            out detectedInfo.corners,
            out detectedInfo.ids,
            detectedInfo.parameters,
            out detectedInfo.rejected
        );

        return detectedInfo;
    }

    public static CameraCalibrationInfo EstimatePoseSingleMarkers(
        ArucoDetectedInfo info,
        float markerLength
    )
    {
        CameraCalibrationInfo calibrationInfo = new();
        CvAruco.EstimatePoseSingleMarkers(
            info.corners,
            markerLength,
            calibrationInfo.cameraMatrix,
            calibrationInfo.distCoeffs,
            calibrationInfo.rvecs,
            calibrationInfo.tvecs
        );
        return calibrationInfo;
    }

    public static TransPair? GetTransPair(
        ArucoDetectedInfo arucoInfo,
        CameraCalibrationInfo calibrationInfo
    )
    {
        if (arucoInfo.corners.Length == 0)
        {
            return null;
        }
        Mat objectPoints =
            new(
                4,
                3,
                MatType.CV_64F,
                new double[] { -.1, -.1, 0, .1, -.1, 0, .1, .1, 0, -.1, .1, 0 }
            );
        Mat imagePoints = PointHelper.GetLineOfMatrix(arucoInfo.corners, 0);
        Mat tr = new();
        Mat tt = new();
        Cv2.SolvePnP(
            objectPoints,
            imagePoints,
            calibrationInfo.cameraMatrix,
            calibrationInfo.distCoeffs,
            tr,
            tt
        );
        return new TransPair(tt.Get<Vec3d>(0), tr.Get<Vec3d>(0));
    }
}
