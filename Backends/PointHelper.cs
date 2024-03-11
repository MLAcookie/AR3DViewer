using OpenCvSharp;

namespace OpencvTest.Backends;

public class PointHelper
{
    public static Point2f[][] GetPoint2fMatrix(int x, int y)
    {
        Point2f[][] ans = new Point2f[y][];
        for (int i = 0; i < y; i++)
        {
            ans[i] = new Point2f[x];
        }
        return ans;
    }
}
