using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Xna.Framework;

namespace Mancala
{

    public class XNAAsyncDispatcher : IApplicationService
    {

        private DispatcherTimer _frameworkDispatcherTimer;

        public XNAAsyncDispatcher(TimeSpan dispatchInterval)
        {
            FrameworkDispatcher.Update();
            this._frameworkDispatcherTimer = new DispatcherTimer();
            this._frameworkDispatcherTimer.Tick += new EventHandler(frameworkDispatcherTimer_Tick);
            this._frameworkDispatcherTimer.Interval = dispatchInterval;
        }

        void IApplicationService.StartService(ApplicationServiceContext context)
        {
            this._frameworkDispatcherTimer.Start();
        }

        void IApplicationService.StopService()
        {
            this._frameworkDispatcherTimer.Stop();
        }

        void frameworkDispatcherTimer_Tick(object sender, EventArgs e)
        {
            FrameworkDispatcher.Update();
        }

    } 

}
