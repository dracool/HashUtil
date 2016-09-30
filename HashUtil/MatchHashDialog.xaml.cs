using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HashUtil
{
    /// <summary>
    /// Interaction logic for MatchHashDialog.xaml
    /// </summary>
    public partial class MatchHashDialog : Window
    {
        public MatchHashDialog()
        {
            InitializeComponent();
            textBoxInputHash.IsReadOnly = true;
        }

        public void Match()
        {
            switch(Runtime.Parameters.HashSource)
            {
                case HashSource.Argument:
                    labelHashSourceInfo.Content = "Hash supplied by Argument:";
                    break;
                default:
                    labelHashSourceInfo.Content = $"Hash found using {Enum.GetName(typeof(HashSource), Runtime.Parameters.HashSource)}:"; 
                    break;
            }

            textBoxInputHash.Text = Runtime.Parameters.Hash;
            labelCurrentInfo.Content = "Calculating Hash(es)";

            Task.Run(() =>
            {
                var hasher = new HashChecker();
                hasher.Progress += Hasher_Progress;
                using (var filestream = new FileStream(Runtime.Parameters.FilePath, FileMode.Open, FileAccess.Read))
                {
                    hasher.Data = filestream;

                    var result = hasher.FindMatch(HashUtils.HexToBytes(Runtime.Parameters.Hash));
                    var text = string.Empty;
                    if(result.Count > 0)
                    {
                        text = string.Concat("Successfully matched hash using the following algorithm(s):\n", string.Join(", ", result));
                    }
                    else
                    {
                        text = "No matching hash was found\nplease check the input hash or repair the file";
                    }
                    this.Synchronous((t) =>
                    {
                        progressBarHashing.Visibility = Visibility.Collapsed;
                        labelCurrentInfo.Content = text;
                    });
                }
                hasher.Progress -= Hasher_Progress;
            });

            Show();
        }

        private void Hasher_Progress(object sender, ProgressEventArgs e)
        {
            progressBarHashing.Synchronous((p) => {
                p.IsIndeterminate = e.IsBoundUnknown;
                p.Maximum = e.UpperBound;
                p.Value = e.Current;
            });
        }
    }
}
