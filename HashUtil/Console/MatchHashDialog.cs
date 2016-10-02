using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using HashUtil.Console.GUI;
using HashUtil.Hashing;

namespace HashUtil.Console
{
    internal class MatchHashDialog : ConsoleDialog
    {
        public MatchHashDialog()
        {
            _tbProgress = new TextBlock
            {
                Left = Left + Width/2,
                Width = Width/2 - 1,
                Top = Top + 6,
                Height = 1,
                Alignment = TextAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Text = string.Empty,
            };

            _tbCurrentInfo = new TextBlock
            {
                Left = Left + 1,
                Width = Width / 2 - 1,
                Top = Top + 6,
                Height = 1,
                Alignment = TextAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Text = string.Empty,
            };
            AddChild(_tbCurrentInfo);
            AddChild(_tbProgress);
        }

        List<HashInfo> _hInfoList;

        private TextBlock _tbProgress;
        private readonly TextBlock _tbCurrentInfo;

        public void Match(ExecutionInfo info)
        {
            var client = new HashMatchClient<HashInfo>();
            client.ProgressChanged += Client_ProgressChanged;
            client.StatusChanged += Client_StatusChanged;
            _hInfoList = info.Hashes;
            _tbCurrentInfo.Text = "Calculating Hashes";
            Task.Run(() => client.Match(info.FilePath, _hInfoList, hi => hi.Hash));
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
                    var client = sender as HashMatchClient<HashInfo>;
                    if (client == null) throw new ArgumentNullException(nameof(client));

                    var result = _hInfoList
                        .Select(hi => new Tuple<string, string, string>(
                            client.Result.ContainsKey(hi) ? client.Result[hi] : " --- ",
                            Enum.GetName(typeof(HashSource), hi.Source),
                            hi.Hash
                        ))
                        .ToList();

                    var methodTextLength = result.Max(t => t.Item1.Length) + 2;
                    var sourceTextLength = result.Max(t => t.Item2.Length) + 2;
                    var hashTextLength = Width - methodTextLength - sourceTextLength;

                    this.SafeUpdate(() =>
                    {
                        _tbCurrentInfo.Text = "Find matching hashes below";
                        var currentLine = Top + 9;

                        RemoveChild(_tbProgress);
                        _tbProgress = null;

                        foreach (var tuple in result)
                        {
                            #region create text blocks
                            var method = new TextBlock
                            {
                                Left = Left,
                                Top = currentLine,
                                Width = methodTextLength,
                                Text = tuple.Item1,
                                Height = 1,
                            };

                            var source = new TextBlock
                            {
                                Left = method.Left + methodTextLength + 1,
                                Top = currentLine,
                                Width = sourceTextLength,
                                Text = tuple.Item2,
                                Height = 1,
                            };

                            var hash = new TextBlock
                            {
                                Left = source.Left + sourceTextLength + 1,
                                Top = currentLine,
                                Width = hashTextLength - 2,
                                Text = tuple.Item3,
                                Height = 1,
                                Trimming = TextTrimming.CharacterEllipsis,
                            };
                            #endregion

                            AddChild(method);
                            AddChild(source);
                            AddChild(hash);

                            currentLine += 2;
                        }

                        if (currentLine > Height)
                        {
                            Height = currentLine + 1;
                        }

                        InvalidateLayout();
                        Close();
                    });

                    client.ProgressChanged -= Client_ProgressChanged;
                    client.StatusChanged -= Client_StatusChanged;
                    break;
                case HashStatus.Failure:
                    client = sender as HashMatchClient<HashInfo>;
                    if (client == null) throw new ArgumentNullException(nameof(client));

                    client.ProgressChanged -= Client_ProgressChanged;
                    client.StatusChanged -= Client_StatusChanged;
                    this.SafeUpdate(() => 
                    {
                        _tbCurrentInfo.Text = "No matching hashes were found";
                        Height = Top + 8;
                        Close();
                    });
                    break;
                default:
                    throw new Exception("Unexpected value for HashStatus (should never happen)");
            }
        }

        private void Client_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.IsBoundUnknown) return;
            var max = e.UpperBound.ToString();
            var text = string.Join(" / ", e.Current.ToString("D" + max.Length.ToString()), max);
            _tbProgress.SafeUpdate(p => p.Text = text);
        }

        protected override void DoUpdate(bool recreate)
        {
            base.DoUpdate(recreate);
            DrawHelper.HorzLine(Left, Top + 8, Width);
        }
    }
}