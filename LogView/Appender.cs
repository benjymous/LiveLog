using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapefruitopia.LiveLog
{
    class Appender : AppenderSkeleton, IDisposable
    {
        private View _view;
        private IAppender _appender;

        public View View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
            }
        }

        public Appender()
        {
            _appender = this;
            ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.AddAppender(_appender);

        }

        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            try
            {
                _view.Invoke((MethodInvoker)delegate
                {
                    View.Append(loggingEvent);
                });
            }
            catch
            {

            }
        }

        public void Dispose()
        {
            try
            {
                if (_appender != null)
                {
                    ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.RemoveAppender(_appender);

                }
            }
            catch
            { }
            _appender = null;
        }
    }
}
