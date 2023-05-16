using System.Drawing;

namespace Diplom
{
    public static class ImageLoader
    {
        public static Image Load(string path)
        {
            return Bitmap.FromFile(path);
        }
    }
}
