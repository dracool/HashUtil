using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace HashUtil.Console.GUI
{
    public class ConsoleDialog : CliElement
    {
        public ConsoleDialog()
        {
            Left = 0;
            Top = 0;
            Height = 10;
            Width = System.Console.BufferWidth;

            _tbTitle = new TextBox
            {
                Alignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Left = 0,
                Top = 0,
                Width = System.Console.BufferWidth,
                Height = 5,
            };
        }

        public string Title
        {
            get { return _tbTitle.Text; }
            set { _tbTitle.Text = value; }
        }

        private TextBox _tbTitle;
        private bool _closeRequested;

        public event EventHandler<ConsoleKeyEventArgs> KeyReceived;
        public event EventHandler Closed;

        private void DoKeyReceived(ConsoleKeyInfo info)
        {
            KeyReceived?.Invoke(this, new ConsoleKeyEventArgs(info));
        }

        private void DoClosed()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        protected override void DoUpdate(bool recreate)
        {
            //does not render anything itself, uses textbox to render title
        }
        
        private void DoMessageLoop()
        {
            while (!_closeRequested)
            {
                try
                {

                    var info = System.Console.ReadKey(true);
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)(() => DoKeyReceived(info)));
                }
                catch(InvalidOperationException)
                {
                    break;
                }
            }
        }

        public void Show()
        {
            System.Console.CancelKeyPress += ConsoleDialog_CancelKeyPress;
            Task.Run((Action)DoMessageLoop);
            Dispatcher.PushFrame(new DispatcherFrame());
        }

        private void ConsoleDialog_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Close();
        }

        public void Close()
        {
            _closeRequested = true;
            System.Console.CancelKeyPress -= ConsoleDialog_CancelKeyPress;
            System.Console.SetCursorPosition(0, Height + 1);
            System.Console.CursorVisible = true;
            System.Windows.Forms.SendKeys.SendWait("{Enter}");
            ConsoleUtils.FreeConsole();
            Dispatcher.InvokeShutdown();
        }
    }
}