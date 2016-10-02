using HashUtil.Hashing;
using System;
using System.Collections.Generic;
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
using System.IO;
using Microsoft.Win32;

namespace HashUtil.Graphical
{
    /// <summary>
    /// Interaktionslogik für SelectDataDialog.xaml
    /// </summary>
    public partial class SelectDataDialog : Window
    {
        public SelectDataDialog()
        {
            InitializeComponent();
        }

        private List<HashInfo> _clipboardHash;

        public void Initialize()
        {
            var h = SystemHashFinder.FindFromClipboard();
            if(h.Count > 0)
            {
                _clipboardHash = h;
                RbCheckHash.IsChecked = true;
                CbSourceClipboard.IsChecked = true;
            }
            Show();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if(RbGenerateHashes.IsChecked ?? false)
            {
                new CalculateHashesDialog().Calculate(TbFilePath.Text);
                Close();
            }
            else
            {
                var hashes = new List<HashInfo>();

                if (CbSourceClipboard.IsChecked ?? false)
                {
                    if(_clipboardHash != null)
                    {
                        hashes.AddRange(_clipboardHash);
                    }
                    var r = SystemHashFinder.FindFromClipboard();

                    if(r.Count > 0)
                    {
                        hashes.AddRange(r.Where(hi => !_clipboardHash.Any(ch => ch.Hash == hi.Hash)));
                    }
                }

                if(CbSourceInput.IsChecked ?? false)
                {
                    var r = SystemHashFinder.TryGetHash(TbSourceInput.Text);
                    if(r != null)
                    {
                        hashes.Add(new HashInfo(r, HashSource.Argument));
                    }
                }

                if(CbSourceFile.IsChecked ?? false)
                {
                    var r = SystemHashFinder.ReadFromFile(TbSourceFile.Text);
                    hashes.AddRange(r);
                }
                
                if (hashes.Count > 0)
                {
                    new MatchHashDialog().Match(new ExecutionInfo(hashes)
                    {
                        FilePath = TbFilePath.Text,
                        Mode = HashingMode.Match,
                    });
                    Close();
                }
                else
                {
                    MessageBox.Show(
                        "No valid hashes were found using the selected method(s), please try again",
                        "HashUtil",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        private void btnSelectHashSourceFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSelectFilePath_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                ValidateNames = true,
                Multiselect = false,
                Title = "Select File to Hash",
            };

            if (!string.IsNullOrWhiteSpace(TbFilePath.Text))
            {
                dlg.FileName = Path.GetFileName(TbFilePath.Text);
                dlg.InitialDirectory = Path.GetDirectoryName(TbFilePath.Text);
            }

            if (dlg.ShowDialog() ?? false)
            {
                TbFilePath.Text = dlg.FileName;
            }
        }
    }
}
