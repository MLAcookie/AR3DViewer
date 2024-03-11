using OpenCvSharp;
using OpenCvSharp.Aruco;

namespace OpencvTest.Backends;

public class ArucoDetectedInfo
{
    public Point2f[][] corners;
    public Point2f[][] rejected;
    public int[] ids;
    public DetectorParameters parameters;

    public ArucoDetectedInfo(int size)
    {
        corners = PointHelper.GetPoint2fMatrix(size, 4);
        rejected = PointHelper.GetPoint2fMatrix(size, 4);
        ids = new int[size];
        parameters = new DetectorParameters();
    }
}

public class CameraCalibrationInfo
{
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

    public static Mat DrawDetectMarkerInfo(Mat image, ArucoDetectedInfo info)
    {
        Mat ans = image.Clone();
        CvAruco.DrawDetectedMarkers(ans, info.corners, info.ids);
        return ans;
    }

    public static CameraCalibrationInfo EstimatePoseSingleMarkers(
        ArucoDetectedInfo arucoDetectedInfo,
        float markerLength
    )
    {
        CameraCalibrationInfo calibrationInfo = new();
        CvAruco.EstimatePoseSingleMarkers(
            arucoDetectedInfo.corners,
            markerLength,
            calibrationInfo.cameraMatrix,
            calibrationInfo.distCoeffs,
            calibrationInfo.rvecs,
            calibrationInfo.tvecs
        );
        return calibrationInfo;
    }
    //CvAruco.EstimatePoseSingleMarkers(corners, 0.09f, cameraMatrix, distCoeffs, rvecs, tvecs);
    //    if (rvecs.Rows != 0)
    //    {
    //        Cv2.DrawFrameAxes(image, cameraMatrix, distCoeffs, rvecs, tvecs, 0.05f);
    //        Vec3d outRvec = rvecs.Get<Vec3d>(1);
    //Vec3d outTvec = tvecs.Get<Vec3d>(1);
    //Console.WriteLine("{0:f2} {1:f2} {2:f2}", outRvec.Item0, outRvec.Item1, outRvec.Item2);
    //        Console.WriteLine("{0:f2} {1:f2} {2:f2}", outTvec.Item0, outTvec.Item1, outTvec.Item2);
    //        Console.WriteLine();
    //    }
}
