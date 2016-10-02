using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
using HashUtil.Hashing;

namespace HashUtil.Graphical
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

        public void Calculate(string filePath)
        {
            var client = new HashCalculateClient();
            client.ProgressChanged += Client_ProgressChanged;
            client.StatusChanged += Client_StatusChanged;

            Task.Run(() => client.Calculate(filePath));

            Show();
        }

        private void Client_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Value)
            {
                case HashStatus.Start:
                case HashStatus.Calculating:
                    break;
                case HashStatus.Success:
                case HashStatus.Failure:
                    var client = sender as HashCalculateClient;
                    if (client == null) throw new ArgumentNullException(nameof(client));
                    var result = client.Result.ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value
                    );

                    this.SafeUpdate((t) =>
                    {
                        Resources["result"] = result;
                        ProgressBarHashing.Visibility = Visibility.Collapsed;
                    });
                    break;
                default:
                    throw new Exception("Invalid HashStatus Value (should never happen)");
            }
        }

        private void Client_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarHashing.SafeUpdate((p) =>
            {
                p.IsIndeterminate = e.IsBoundUnknown;
                p.Maximum = e.UpperBound;
                p.Value = e.Current;
            });
        }

        //not sure why this is being suggested, ToList prevents multiple enumeration
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private void CopySelectedToClipboard()
        {
            var sel = ListView.SelectedItems?.Cast<KeyValuePair<string, string>>()?.ToList();
            if (sel == null) return;
            if (sel.Count == 0) return;

            var sb = new StringBuilder();
            foreach(var kv in sel)
            {
                sb.AppendLine($"{kv.Key.Trim()}\t{kv.Value}");
            }
            Clipboard.SetText(sb.ToString());
        }

        //not sure why this is being suggested, ToList prevents multiple enumeration
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private void CopyAllToClipboard()
        {
            var sel = ListView.Items?.Cast<KeyValuePair<string, string>>()?.ToList();
            if (sel == null) return;
            if (sel.Count == 0) return;

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
            var item = ListView.GetContainerAtPoint<ListViewItem>(Mouse.GetPosition(ListView))?.Content;
            if(item is KeyValuePair<string, Hash>)
            {
                var kv = (KeyValuePair<string, Hash>)item;
                Clipboard.SetText(kv.Value);
            }
        }
    }
}
