using HashUtil.Hashing;
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

namespace HashUtil.Graphical
{
    /// <summary>
    /// Interaction logic for MatchHashDialog.xaml
    /// </summary>
    public partial class MatchHashDialog : Window
    {
        public MatchHashDialog()
        {
            InitializeComponent();
        }

        List<HashInfo> _hInfoList;

        internal void Match(ExecutionInfo info)
        {   
            var client = new HashMatchClient<HashInfo>();
            client.ProgressChanged += Client_ProgressChanged;
            client.StatusChanged += Client_StatusChanged;
            _hInfoList = info.Hashes;
            Task.Run( () => client.Match(info.FilePath, _hInfoList, hi => hi.Hash) );
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
                    var client = sender as HashMatchClient<HashInfo>;
                    if(client == null) throw new ArgumentNullException(nameof(client));

                    this.SafeUpdate(() =>
                    {
                        Resources["result"] = _hInfoList
                            .Select(hi => new Tuple<string, string, string>(
                                client.Result.ContainsKey(hi) ? client.Result[hi] : " --- ",
                                Enum.GetName(typeof(HashSource), hi.Source),
                                hi.Hash
                            ))
                            .ToList();
                        ProgressBarHashing.Visibility = Visibility.Collapsed;
                    });

                    client.ProgressChanged -= Client_ProgressChanged;
                    client.StatusChanged -= Client_StatusChanged;
                    break;
                default:
                    throw new Exception("Unexpected value for HashStatus (should never happen)");
            }
        }

        private void Client_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarHashing.SafeUpdate((p) => {
                p.IsIndeterminate = e.IsBoundUnknown;
                p.Maximum = e.UpperBound;
                p.Value = e.Current;
            });
        }
    }
}
