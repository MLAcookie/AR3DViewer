using OpenCvSharp;

namespace OpencvTest.Backends;

public class PointHelper
{
    public static Point2f[][] NewPoint2fMatrix(int x, int y)
    {
        Point2f[][] ans = new Point2f[y][];
        for (int i = 0; i < y; i++)
        {
            ans[i] = new Point2f[x];
        }
        return ans;
    }

    public static Mat GetLineOfMatrix(Point2f[][] matrix, int index = 0)
    {
        Mat ans = new(4, 2, MatType.CV_32F);
        for (int i = 0; i < 4; i++)
        {
            ans.Set(i, matrix[index][i]);
        }
        return ans;
    }
}
