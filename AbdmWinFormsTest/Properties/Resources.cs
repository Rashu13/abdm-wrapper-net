using System.Drawing;

namespace HMS.Properties
{
    public class Resources
    {
        private static Image? _nhalogo;
        public static Image nhalogo
        {
            get
            {
                if (_nhalogo == null)
                {
                    _nhalogo = new Bitmap(100, 100);
                }
                return _nhalogo;
            }
        }
    }
}
