using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace HashUtil.Console.GUI
{
    public abstract class CliElement : DispatcherObject
    {
        protected CliElement()
        {
            _children = new List<CliElement>();
            Children = new ReadOnlyCollection<CliElement>(_children);
        }

        private int _left;
        private int _top;
        private int _width;
        private int _height;

        private bool _overrideColors;
        private ConsoleColor _foreground;
        private ConsoleColor _background;

        public bool OverrideColors
        {
            get
            {
                return _overrideColors;
            }

            set
            {
                _overrideColors = value;
                InvalidateLayout();
            }
        }

        public ConsoleColor Foreground
        {
            get
            {
                return _foreground;
            }

            set
            {
                _foreground = value;
                if (OverrideColors) InvalidateLayout();
            }
        }

        public ConsoleColor Background
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
                if (OverrideColors) InvalidateLayout();
            }
        }

        public int Left
        {
            get { return _left; }
            set
            {
                _left = value;
                InvalidateLayout();
            }
        }

        public int Top
        {
            get { return _top; }
            set
            {
                _top = value;
                InvalidateLayout();
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                InvalidateLayout();
            }
        }

        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                InvalidateLayout();
            }
        }

        private bool _invalidated;
        private bool _layoutInvalidated;

        public CliElement Parent { get; private set; }

        private readonly List<CliElement> _children;
        public IReadOnlyCollection<CliElement> Children { get; }

        private void SetParent(CliElement element)
        {
            if (Parent != null)
            {
                Parent._children.Remove(this);
                Parent.InvalidateLayout();
            }
            Parent = element;
        }

        public void AddChild(CliElement element)
        {
            VerifyAccess();
            element.SetParent(this);
            _children.Add(element);
        }

        public void RemoveChild(CliElement element)
        {
            VerifyAccess();
            element.SetParent(null);
            _children.Remove(element);
        }

        public void Invalidate()
        {
            VerifyAccess();
            _invalidated = true;
            Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)Update);
        }

        public void InvalidateLayout()
        {
            VerifyAccess();
            _invalidated = true;
            _layoutInvalidated = true;
            foreach (var child in Children)
            {
                child.InvalidateLayout();
            }
            Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)Update);
        }

        public void Update()
        {
            VerifyAccess();
            if (_invalidated)
            {
                _invalidated = false;
                if (OverrideColors)
                {
                    DrawHelper.Color(Foreground, Background, () => DoUpdate(_layoutInvalidated));
                }
                else
                {
                    DoUpdate(_layoutInvalidated);
                }
                _layoutInvalidated = false;
            }
            foreach (var child in Children)
            {
                child.Update();
            }
        }

        protected abstract void DoUpdate(bool recreate);
    }
}