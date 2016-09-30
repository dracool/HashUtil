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
            var client = new CalculateHashesClient();
            client.Progress += Client_Progress;
            client.Calculated += Client_Calculated;
            Task.Run((Action)client.Calculate);
            Show();
        }

        private void Client_Calculated(object sender, CalculatedEventArgs e)
        {
            var client = sender as CalculateHashesClient;
            client.Calculated -= Client_Calculated;
            client.Progress -= Client_Progress;
            this.Synchronous((t) =>
            {
                Resources["result"] = e.Hashes.ToDictionary((kv) => kv.Key, (kv) => HashUtils.BytesToHex(kv.Value));
                progressBarHashing.Visibility = Visibility.Collapsed;
            });
        }
        private void Client_Progress(object sender, ProgressEventArgs e)
        {
            progressBarHashing.Asynchronous((p) =>
            {
                p.IsIndeterminate = e.IsBoundUnknown;
                p.Maximum = e.UpperBound;
                p.Value = e.Current;
            });
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
