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

        Dictionary<log4net.Core.Level, ListViewItem> Counts = new Dictionary<log4net.Core.Level, ListViewItem>();

        public View()
        {
            InitializeComponent();

            AddCountItem(log4net.Core.Level.Fatal);
            AddCountItem(log4net.Core.Level.Error);
            AddCountItem(log4net.Core.Level.Warn);
            AddCountItem(log4net.Core.Level.Info);
            AddCountItem(log4net.Core.Level.Debug);

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
            return IsVisible(level.DisplayName);
        }

        internal bool IsVisible(string level)
        {
            switch (level)
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

            var colour = GetColour(loggingEvent.Level);

            lvi.SubItems[0].BackColor = colour;
            lvi.UseItemStyleForSubItems = false;
            lvi.Tag = loggingEvent;

            if (!Counts.ContainsKey(loggingEvent.Level))
            {
                var countLvi = new ListViewItem(loggingEvent.Level + ": 1");
                countLvi.Tag = 1;
                countLvi.Name = loggingEvent.Level.DisplayName;
                countLvi.BackColor = colour;
                countLvi.Checked = IsVisible(loggingEvent.Level);
                Counts.Add(loggingEvent.Level, countLvi);
            }
            else
            {
                var countLvi = Counts[loggingEvent.Level];
                int? count = countLvi.Tag as int?;
                count++;
                countLvi.Tag = count;
            }
            RefreshCounts();

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

        private void AddCountItem(log4net.Core.Level level)
        {
            var countLvi = new ListViewItem(level + ": 0");
            countLvi.Tag = 1;
            countLvi.Name = level.DisplayName;
            countLvi.BackColor = GetColour(level);
            countLvi.Checked = IsVisible(level);                        
            Counts.Add(level, countLvi);
        }

        private void RefreshCounts()
        {
            foreach(KeyValuePair<log4net.Core.Level, ListViewItem> kvp in Counts)
            {
                var lvi = kvp.Value;
                lvi.Text = lvi.Name + ": " + (lvi.Tag as int?).ToString();
                lvi.Checked = IsVisible(lvi.Name);             
                if(!listView2.Items.Contains(lvi))
                {
                    listView2.Items.Add(lvi);
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
            RefreshCounts();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            Items.Clear();

            foreach (KeyValuePair<log4net.Core.Level, ListViewItem> kvp in Counts)
            {
                kvp.Value.Tag = 0;
            }

            RefreshCounts();
        }

        private void listView2_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var lvi = e.Item;

            switch (lvi.Name)
            {
                case "FATAL":
                    fatalToolStripMenuItem.Checked = lvi.Checked;
                    break;

                case "ERROR":
                    errorToolStripMenuItem.Checked = lvi.Checked;
                    break;

                case "WARN":
                    warningToolStripMenuItem.Checked = lvi.Checked;
                    break;

                case "DEBUG":
                    debugToolStripMenuItem.Checked = lvi.Checked;
                    break;

                case "INFO":
                    infoToolStripMenuItem.Checked = lvi.Checked;
                    break;
            }
        }
    }
}
