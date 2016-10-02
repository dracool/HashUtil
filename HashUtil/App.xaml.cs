using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using HashUtil.Hashing;
using HashUtil.Graphical;
using HashUtil.Console;

namespace HashUtil
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var info = new ExecutionInfo();
            
            if(e.Args.Length >= 1)
            {
                info.FilePath = e.Args[0];

                if (e.Args.Length >= 2)
                {
                    try
                    {
                        info.Hashes.Add(new HashInfo(new Hash(e.Args[1]), HashSource.Argument));
                        info.Mode = HashingMode.Match;
                    }
                    catch
                    {
                        info.Hashes.Clear();
                        info.Mode = HashingMode.Calculate;
                    }
                }
                else
                {
                    var result = SystemHashFinder.FindAll(info.FilePath);
                    if (result.Count > 0)
                    {
                        info.Hashes.AddRange(result);
                        info.Mode = HashingMode.Match;
                    }
                    else
                    {
                        info.Hashes.Clear();
                        info.Mode = HashingMode.Calculate;
                    }
                }
            }
            else
            {
                //no arguments given, using select mode
                info.FilePath = string.Empty;
                info.Hashes.Clear();
                info.Mode = HashingMode.Select;
            }
            
            if(ConsoleUtils.IsCommandLine)
            {
                new ConsoleRunner().Run(info);
            }
            else
            {
                new GuiRunner().Run(info);
            }
        }
    }
}
