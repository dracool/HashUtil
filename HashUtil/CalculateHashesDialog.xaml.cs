using System;
using System.Collections;
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
    /// Interaction logic for CalculateHashesDialog.xaml
    /// </summary>
    public partial class CalculateHashesDialog : Window
    {
        public CalculateHashesDialog()
        {
            InitializeComponent();
        }

        public void Calculate()
        {
            Task.Run(() =>
            {
                var hasher = new HashChecker();
                hasher.Progress += Hasher_Progress;
                using (var filestream = new FileStream(Runtime.Parameters.FilePath, FileMode.Open, FileAccess.Read))
                {
                    hasher.Data = filestream;

                    var result = hasher.CalculateHashes().ToDictionary(
                        kv => kv.Key,
                        kv => HashUtils.BytesToHex(kv.Value)
                    );
                    
                    this.SafeUpdate((t) =>
                    {
                        Resources["result"] = result;
                        progressBarHashing.Visibility = Visibility.Collapsed;
                    });
                }
                hasher.Progress -= Hasher_Progress;
            });
            Show();
        }

        private void CopySelectedToClipboard()
        {
            var sel = listView.SelectedItems?.Cast<KeyValuePair<string, string>>();
            if (sel == null) return;
            if (sel.Count() == 0) return;

            var sb = new StringBuilder();
            foreach(var kv in sel)
            {
                sb.AppendLine($"{kv.Key.Trim()}\t{kv.Value}");
            }
            Clipboard.SetText(sb.ToString());
        }

        private void CopyAllToClipboard()
        {
            var sel = listView.Items?.Cast<KeyValuePair<string, string>>();
            if (sel == null) return;
            if (sel.Count() == 0) return;

            var sb = new StringBuilder();
            foreach (var kv in sel)
            {
                sb.AppendLine($"{kv.Key.Trim()}\t{kv.Value}");
            }
            Clipboard.SetText(sb.ToString());
        }

        private void Hasher_Progress(object sender, ProgressEventArgs e)
        {
            progressBarHashing.SafeUpdate((p) =>
            {
                p.IsIndeterminate = e.IsBoundUnknown;
                p.Maximum = e.UpperBound;
                p.Value = e.Current;
            });
        }

        private void Menu_CopySelected_Click(object sender, RoutedEventArgs e)
        {
            CopySelectedToClipboard();
        }

        private void Menu_CopyAll_Click(object sender, RoutedEventArgs e)
        {
            CopyAllToClipboard();
        }

        private void Menu_CopyUnderCursor_Click(object sender, RoutedEventArgs e)
        {
            var item = listView.GetContainerAtPoint<ListViewItem>(Mouse.GetPosition(listView))?.Content;
            if(item is KeyValuePair<string, string>)
            {
                var kv = (KeyValuePair<string, string>)item;
                Clipboard.SetText(kv.Value);
            }
        }
    }
}
