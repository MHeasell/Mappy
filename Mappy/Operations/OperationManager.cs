namespace Mappy.Operations
{
    using System;
    using System.Collections.Generic;

    public class OperationManager
    {
        private Stack<IReplayableOperation> undoStack = new Stack<IReplayableOperation>();
        private Stack<IReplayableOperation> redoStack = new Stack<IReplayableOperation>();

        private bool canUndo = false;
        private bool canRedo = false;

        private int markCounter = 0;
        private bool isMarked = true;

        public event EventHandler CanUndoChanged;

        public event EventHandler CanRedoChanged;

        public event EventHandler IsMarkedChanged;

        public bool IsMarked
        {
            get
            {
                return this.isMarked;
            }

            private set
            {
                if (value != this.isMarked)
                {
                    this.isMarked = value;
                    this.OnIsMarkedChanged();
                }
            }
        }

        public bool CanUndo
        {
            get
            {
                return this.canUndo;
            }

            private set
            {
                bool old = this.CanUndo;
                this.canUndo = value;
                if (value != old)
                {
                    this.OnCanUndoChanged();
                }
            }
        }

        public bool CanRedo
        {
            get
            {
                return this.canRedo;
            }

            private set
            {
                bool old = this.CanRedo;
                this.canRedo = value;
                if (value != old)
                {
                    this.OnCanRedoChanged();
                }
            }
        }

        private int MarkCounter
        {
            get
            {
                return this.markCounter;
            }

            set
            {
                this.markCounter = value;
                this.IsMarked = value == 0;
            }
        }

        public void SetNowAsMark()
        {
            this.MarkCounter = 0;
        }

        public void Execute(IReplayableOperation op)
        {
            op.Execute();
            this.Push(op);
        }

        public void Push(IReplayableOperation op)
        {
            this.redoStack.Clear();
            this.undoStack.Push(op);

            this.MarkCounter += 1;

            this.RefreshProperties();
        }

        public void Undo()
        {
            IReplayableOperation op = this.undoStack.Pop();
            op.Undo();
            this.redoStack.Push(op);

            this.MarkCounter -= 1;

            this.RefreshProperties();
        }

        public void Redo()
        {
            IReplayableOperation op = this.redoStack.Pop();
            op.Execute();
            this.undoStack.Push(op);

            this.MarkCounter += 1;

            this.RefreshProperties();
        }

        public void Clear()
        {
            this.undoStack.Clear();
            this.redoStack.Clear();

            this.RefreshProperties();

            this.MarkCounter = 0;
        }

        public IReplayableOperation PeekUndo()
        {
            return this.undoStack.Peek();
        }

        public void Replace(IReplayableOperation op)
        {
            this.undoStack.Pop();
            this.undoStack.Push(op);
        }

        private void RefreshProperties()
        {
            this.CanRedo = this.redoStack.Count != 0;
            this.CanUndo = this.undoStack.Count != 0;
        }

        private void OnCanRedoChanged()
        {
            EventHandler h = this.CanRedoChanged;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }

        private void OnCanUndoChanged()
        {
            EventHandler h = this.CanUndoChanged;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }

        private void OnIsMarkedChanged()
        {
            EventHandler h = this.IsMarkedChanged;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }
    }
}
