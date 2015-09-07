using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapefruitopia.LiveLog
{
    public partial class View : UserControl
    {
        Appender _appender;

        List<ListViewItem> Items = new List<ListViewItem>();

        public View()
        {
            InitializeComponent();

            _appender = new Appender();
            _appender.View = this;
        }

        internal Color GetColour(log4net.Core.Level level)
        {
            switch (level.DisplayName)
            {
                case "FATAL":
                    return Color.Tomato;

                case "ERROR":
                    return Color.DarkSalmon;

                case "WARN":
                    return Color.Yellow;

                case "DEBUG":
                    return Color.LightGray;             
            }

            return Color.White;
        }

        internal bool IsVisible(log4net.Core.Level level)
        {
            switch (level.DisplayName)
            {
                case "FATAL":
                    return fatalToolStripMenuItem.Checked;

                case "ERROR":
                    return errorToolStripMenuItem.Checked;

                case "WARN":
                    return warningToolStripMenuItem.Checked;

                case "DEBUG":
                    return debugToolStripMenuItem.Checked;
            }

            return infoToolStripMenuItem.Checked;
        }

        internal void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            var lvi = new ListViewItem(new string[] { loggingEvent.Level.DisplayName, loggingEvent.TimeStamp.ToLongTimeString(), loggingEvent.RenderedMessage });

            lvi.SubItems[0].BackColor = GetColour(loggingEvent.Level);
            lvi.UseItemStyleForSubItems = false;
            lvi.Tag = loggingEvent;
            
            Items.Add(lvi);
            if (IsVisible(loggingEvent.Level))
            {
                bool scroll = (listView1.Items.Count > 0) ? listView1.Items[listView1.Items.Count - 1].Bounds.IntersectsWith(listView1.ClientRectangle) : false;

                listView1.Items.Add(lvi);

                if (scroll)
                {
                    lvi.EnsureVisible();
                }
            }


        }

        private void RefreshVisible()
        {
            var top = listView1.TopItem;
            bool nextTop = true;
            listView1.Items.Clear();
            foreach(var lvi in Items)
            {
                var ev = lvi.Tag as log4net.Core.LoggingEvent;  
                if (IsVisible(ev.Level))
                {
                    listView1.Items.Add(lvi);
                    if (nextTop || lvi == top)
                    {
                        listView1.EnsureVisible(lvi.Index);
                        nextTop = false;
                    }
                }  
                else
                {
                    if (lvi == top)
                    {
                        nextTop = true;
                    }
                }
            }
        }

        private void listView1_SizeChanged(object sender, EventArgs e)
        {
            // Make last column take up remailing available space
            listView1.Columns[2].Width = this.Width - (listView1.Columns[0].Width + listView1.Columns[1].Width) - 23;
        }

        private void filterChanged(object sender, EventArgs e)
        {
            RefreshVisible();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            Items.Clear();
        }
    }
}
