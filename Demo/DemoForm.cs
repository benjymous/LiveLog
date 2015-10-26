using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapefruitopia.LiveLog.Demo
{
    public partial class DemoForm : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        Random rnd = new Random();

        public DemoForm()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            timer1.Enabled = true;

            log.Debug("This is a debug message");
            log.Info("Info");
            log.Error("Error");
            log.Warn("Warning");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch(rnd.Next(6))
            {
                case 0:
                    log.Debug("This is a debug message");
                    break;
                case 2:
                    log.Error("An error occured");
                    break;
                case 3:
                    log.Fatal("ABORT!  ABORT!", new System.Exception("He's dead jim", new System.Exception("innerexception")));
                    break;
                case 4:
                    log.Info("Info!");
                    break;
                case 5:
                    log.Warn("Warning Will Robinson!");
                    break;
            }

            timer1.Interval = rnd.Next(250)+1;
            timer1.Enabled = true;
        }
    }
}
