using System.Windows.Forms;

namespace grapefruitopia.LiveLog
{
    class DoubleBufferedListView : ListView
    {
        public DoubleBufferedListView()
        {
            this.DoubleBuffered = true;
        }
    }
}
