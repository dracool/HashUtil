using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HashUtil.Console.GUI;
using HashUtil.Hashing;

namespace HashUtil.Console
{
    internal class CalculateHashDialog : ConsoleDialog
    {
        public CalculateHashDialog()
        {
            _tbProgress = new TextBlock
            {
                Left = Left + Width / 2,
                Width = Width / 2 - 1,
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
                Text = "Calculating Hashes",
            };
            AddChild(_tbCurrentInfo);
            AddChild(_tbProgress);
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

                    var result = client.Result
                        .Select(kv => new Tuple<string, string>(kv.Key, kv.Value))
                        .ToList();

                    result.Sort((l,r) => string.Compare(l.Item1, r.Item1, StringComparison.InvariantCulture));

                    this.SafeUpdate(() =>
                    {
                        _tbCurrentInfo.Text = "Done calculating hashes";
                        var currentLine = Top + 9;

                        var methodTextLength = result.Max(t => t.Item1.Length) + 2;
                        var hashTextLength = Width - methodTextLength - 1;

                        foreach (var tuple in result)
                        {
                            #region create text blocks
                            var method = new TextBlock
                            {
                                Left = Left + 1,
                                Top = currentLine,
                                Width = methodTextLength,
                                Text = tuple.Item1,
                                Height = 1,
                            };
                            
                            var hash = new TextBlock
                            {
                                Left = method.Left + methodTextLength + 1,
                                Top = currentLine,
                                Width = hashTextLength - 2,
                                Text = tuple.Item2,
                                Height = 1,
                                Trimming = TextTrimming.CharacterEllipsis,
                            };
                            #endregion

                            AddChild(method);
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
                    break;
                default:
                    throw new Exception("Invalid HashStatus Value (should never happen)");
            }
        }

        private void Client_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.IsBoundUnknown) return;
            var max = e.UpperBound.ToString();
            var text = string.Join(" / ", e.Current.ToString("D" + max.Length.ToString()), max);
            _tbProgress.SafeUpdate(p => p.Text = text);
        }

        private readonly TextBlock _tbProgress;
        private readonly TextBlock _tbCurrentInfo;

        protected override void DoUpdate(bool recreate)
        {
            base.DoUpdate(recreate);
            DrawHelper.HorzLine(Left, Top + 8, Width);
        }
    }
}
