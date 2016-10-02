using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace HashUtil.Console.GUI
{
    class Menu : CliElement
    {
        private ObservableCollection<string> _itemSource;
        
        public int SelectedIndex { get; private set; }

        public ObservableCollection<string> ItemSource
        {
            get { return _itemSource; }
            set
            {
                if (_itemSource != null)
                    _itemSource.CollectionChanged -= ItemSource_CollectionChanged;
                _itemSource = value;
                if (_itemSource != null)
                {
                    _itemSource.CollectionChanged += ItemSource_CollectionChanged;
                }
                InvalidateLayout();
            }
        }

        private void ItemSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Invalidate();
        }

        public void SelectPrevious()
        {
            if (SelectedIndex <= 0)
            {
                SelectedIndex = (ItemSource?.Count ?? 0) - 1;
            }
            else
            {
                SelectedIndex--;
            }
            Invalidate();
        }

        public void SelectNext()
        {
            if(ItemSource == null)
            {
                SelectedIndex = -1;
            }
            else if(SelectedIndex >= ItemSource.Count - 1)
            {
                SelectedIndex = 0;
            }
            else
            {
                SelectedIndex++;
            }
            Invalidate();
        }

        public void ClearSelection()
        {
            SelectedIndex = -1;
            Invalidate();
        }

        protected override void DoUpdate(bool recreate)
        {
            if (ItemSource == null) return;
            for (var i = 0; i < ItemSource.Count; i++)
            {
                if (2 * i > Height) break;
                if(i == SelectedIndex)
                {
                    // Disabled because the closure code is exeuted synchornously
                    // ReSharper disable AccessToModifiedClosure
                    DrawHelper.Inverted(() => DrawHelper.WriteText(Left, Top + 2 * i, Width, $"{i + 1}. {ItemSource[i]}"));
                    // ReSharper enable AccessToModifiedClosure
                }
                else
                {
                    DrawHelper.WriteText(Left, Top + 2 * i, Width, $"{i + 1}. {ItemSource[i]}");
                }
            }
        }
    }
}