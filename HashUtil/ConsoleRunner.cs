using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HashUtil
{
    using static Console;
    using static ConsoleDrawer;

    static class ConsoleDrawer
    {
        public class ProgressBar
        {
            public ProgressBar(int x, int y, int w)
            {
                this.x = x;
                this.w = w;
                this.y = y;
                Value = 0;
                Maximum = 100;
                IsIndeterminate = true;
                frame = 1;
            }

            private int y { get; }
            private int x { get; }
            private int w { get; }
            private int frame;

            public bool Visible { get; set; }

            public int Value { get; set; }
            public int Maximum { get; set; }
            public bool IsIndeterminate { get; set; }

            public void Draw()
            {
                if(!Visible)
                {
                    for(var i = y - 1; i <= y + 1; i++)
                    {
                        HorzLine(x, i, w);
                    }

                    return;
                }

                Box(x, y - 1, w, 3);

                
                if(IsIndeterminate)
                {
                    Inverted(() => HorzLine(x + 1, y, w - 2));
                    if (frame >= w - 8)
                    {
                        frame = 1;
                    }

                    Colored(ConsoleColor.DarkGreen, () =>
                    {
                        HorzLine(x + frame, y, 7);
                    });
                    frame += 3;
                }
                else
                {
                    frame = 1;
                    var filled = (int)Math.Round((w - 2) * ( (double)Value / Maximum));
                    var text = $"{Value} / {Maximum}";
                    var spacer = (w - 2 - text.Length) / 2;

                    if(filled >= spacer + text.Length)
                    {
                        filled -= spacer + text.Length;
                        Colored(ConsoleColor.DarkGreen, () =>
                        {
                            HorzLine(x + 1, y, spacer);
                            Inverted(() => Write(text));
                            HorzLine(x + 1 + spacer + text.Length, y, filled);
                        });
                    }
                    else if(filled > spacer)
                    {
                        var off = filled - spacer;
                        filled = spacer;
                        var t1 = text.Substring(0, off);
                        text = text.Substring(off);

                        Colored(ConsoleColor.DarkGreen, () =>
                        {
                            HorzLine(x + 1, y, filled);
                            Inverted(() => Write(t1));
                            Write(text);
                        });
                    }
                    else
                    {
                        Colored(ConsoleColor.DarkGreen, () =>
                        {
                            HorzLine(x + 1, y, filled);
                            WriteTextOn(x + 1, y, w - 2, text);
                        });
                    }
                }
            }
        }
        
        static void Drawing(Action a)
        {
            var fg = ForegroundColor;
            var bg = BackgroundColor;
            a();
            ForegroundColor = fg;
            BackgroundColor = bg; 
        }

        static void InvertColors()
        {
            var temp = BackgroundColor;
            BackgroundColor = ForegroundColor;
            ForegroundColor = temp;
        }

        static void Inverted(Action a) => Drawing( () =>
        {
            InvertColors();
            a();
        });

        static void Colored(ConsoleColor foreground, Action a)
        {
            Colored(foreground, BackgroundColor, a);
        }

        static void Colored(ConsoleColor foreground, ConsoleColor background, Action a) => Drawing( () =>
        {
            ForegroundColor = foreground;
            BackgroundColor = background;
            a();
        });

        static void HorzLine(int x, int y, int w, char c = '=') => Drawing(() =>
        {
            for (int i = 0; i < w; i++)
            {
                SetCursorPosition(x + i, y);
                Write(c);
            }
        });

        static public void VertLine(int x, int y, int h, char c = '|') => Drawing(() =>
        {
            for (int i = 0; i < h; i++)
            {
                SetCursorPosition(x, y + i);
                Write(c);
            }
        });
        
        static public void Dot(int x, int y, char c)
        {
            SetCursorPosition(x, y);
            Write(c);
        }

        static public void Box(int x, int y, int w, int h)
        {
            HorzLine(x, y, w);
            VertLine(x, y, h);
            HorzLine(x, y + h - 1, w);
            VertLine(x + w - 1, y, h);
            Dot(x, y);
            Dot(x + w - 1, y);
            Dot(x, y + h - 1);
            Dot(x + w - 1, y + h - 1);
        }

        static public void WriteTextOn(int x, int y, int width, string text, bool center = true)
        {
            if (text.Length > width)
            {
                text = text.Substring(0, width - 3) + "...";
            }

            if (center)
            {
                x = x + (width - text.Length) / 2;
            }

            SetCursorPosition(x, y);
            Write(text);
        }
    }

    abstract class ConsoleDialog
    {
        public int Height { get; protected set; } = 20;

        protected void Update()
        {
            DoDraw();
            Box(0, 0, BufferWidth, 5);
            WriteTextOn(1, 2, BufferWidth - 1, "HashUtil");
        }

        protected abstract void DoDraw();
    }

    class ConsoleMatchHashDialog : ConsoleDialog
    {
        public ConsoleMatchHashDialog()
        {
            Height = 20;
        }
    
        private string inputHash { get; set; }
        private string inputSourceText { get; set; }
        private string currentInfo { get; set; }
        private ProgressBar progress { get; set; }

        public void Match()
        {
            progress = new ProgressBar(10, 7, BufferWidth - 20);
            progress.Visible = true;

            switch (Runtime.Parameters.HashSource)
            {
                case HashSource.Argument:
                    inputSourceText = "Hash supplied by Argument:";
                    break;
                default:
                    inputSourceText= $"Hash found using {Enum.GetName(typeof(HashSource), Runtime.Parameters.HashSource)}:";
                    break;
            }
            inputHash = Runtime.Parameters.Hash;

            currentInfo = "Calculating Hash(es) ...";

            var hasher = new HashChecker();
            hasher.Progress += hasher_ProgressChanged;
            using (var filestream = new FileStream(Runtime.Parameters.FilePath, FileMode.Open, FileAccess.Read))
            {
                hasher.Data = filestream;

                var result = hasher.FindMatch(HashUtils.HexToBytes(Runtime.Parameters.Hash));
                var text = string.Empty;
                if (result.Count > 0)
                {
                    text = string.Concat("Successfully matched hash using the following algorithm(s):", string.Join(", ", result));
                }
                else
                {
                    text = "No matching hash was found please check the input hash or repair the file";
                }
 
                progress.Visible = false;
                inputSourceText = text;
                Update();
            }
            hasher.Progress -= hasher_ProgressChanged;
        }

        private void hasher_ProgressChanged(object sender, ProgressEventArgs e)
        {
            progress.IsIndeterminate = e.IsBoundUnknown;
            progress.Maximum = e.UpperBound;
            progress.Value = e.Current;
            Update();
        }

        protected override void DoDraw()
        {
            var line = 10;

            WriteTextOn(0, line++, BufferWidth, inputSourceText, false);

            if(inputHash.Length > BufferWidth)
            {
                var ratio = (int)Math.Ceiling((double)BufferWidth / inputHash.Length);
                ratio = inputHash.Length / ratio;
                var iph = inputHash;
                while(iph.Length > 0)
                {
                    WriteTextOn(0, line++, BufferWidth, iph.Substring(0, ratio), false);
                    iph = iph.Substring(ratio, iph.Length);
                }
            }
            else
            {
                WriteTextOn(0, line++, BufferWidth, inputHash, false);
            }

            SetCursorPosition(0, line++);
            Write(currentInfo);

            progress.Draw();
        }
    }
    

    class ConsoleRunner
    {
        public void Run()
        {
            Clear();
            CursorVisible = false;
            var height = 20;
            switch(Runtime.Parameters.Mode)
            {
                case HashingMode.Match:
                    var dlg = new ConsoleMatchHashDialog();
                    dlg.Match();
                    height = dlg.Height;
                    break;
                case HashingMode.Calculate:
                    WriteLine("Calculation mode is currently not supported using console!");
                    break;
                case HashingMode.Select:
                    WriteLine("Selection mode is currently not supported using console!");
                    break;
                default:
                    throw new Exception("Invalid Value for Mode Parameter (should never happen)");
            }
            
            SetCursorPosition(0, height);
            System.Windows.Forms.SendKeys.SendWait("{Enter}");
            RuntimeUtils.FreeConsole();
            Application.Current.Shutdown();
        }
    }
}
