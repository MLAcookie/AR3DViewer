using Godot;
using OpenCvSharp;

namespace OpencvTest.Backends;
public static class MatHelper
{
    public static ImageTexture ToImageTexture(this Mat image)
    {
        var bytes = image.ToBytes();
        Image temp = new();
        temp.LoadPngFromBuffer(bytes);
        return ImageTexture.CreateFromImage(temp);
    }
}
