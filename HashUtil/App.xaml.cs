using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.IO;

namespace HashUtil
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if(e.Args.Length >= 1)
            {
                Runtime.Builder.FilePath = e.Args[0];

                if (e.Args.Length >= 2)
                {
                    Runtime.Builder.Hash = e.Args[1];
                    Runtime.Builder.HashSource = HashSource.Argument;
                    Runtime.Builder.Mode = HashingMode.Match;
                }
                else
                {
                    byte[] hash;
                    HashSource source;
                    if (SystemHashFinder.Find(out hash, out source))
                    {
                        Runtime.Builder.Hash = HashUtils.BytesToHex(hash);
                        Runtime.Builder.HashSource = source;
                        Runtime.Builder.Mode = HashingMode.Match;
                    }
                    else
                    {
                        Runtime.Builder.Hash = string.Empty;
                        Runtime.Builder.HashSource = HashSource.None;
                        Runtime.Builder.Mode = HashingMode.Calculate;
                    }
                }
            }
            else
            {
                //no arguments given, using select mode
                Runtime.Builder.Mode = HashingMode.Select;
                Runtime.Builder.HashSource = HashSource.None;
                Runtime.Builder.Hash = string.Empty;
                Runtime.Builder.FilePath = string.Empty;
            }
            
            Runtime.Builder.Build();

            switch(Runtime.Parameters.Interface)
            {
                case Interface.Console:
                    new ConsoleRunner().Run();
                    break;
                case Interface.GUI:
                    new GuiRunner().Run();
                    break;
                default:
                    throw new Exception("Invalid value for Interface Parameter (should never happen)");
            }
        } 
    }
}
