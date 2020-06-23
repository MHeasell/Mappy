namespace Mappy.Operations
{
    using System;
    using System.Collections.Generic;

    public class OperationManager
    {
        private readonly Stack<IReplayableOperation> undoStack = new Stack<IReplayableOperation>();
        private readonly Stack<IReplayableOperation> redoStack = new Stack<IReplayableOperation>();

        private bool canUndo;
        private bool canRedo;

        private int markCounter;
        private bool isMarked = true;

        public event EventHandler CanUndoChanged;

        public event EventHandler CanRedoChanged;

        public event EventHandler IsMarkedChanged;

        public bool IsMarked
        {
            get => this.isMarked;

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
            get => this.canUndo;

            private set
            {
                var old = this.CanUndo;
                this.canUndo = value;
                if (value != old)
                {
                    this.OnCanUndoChanged();
                }
            }
        }

        public bool CanRedo
        {
            get => this.canRedo;

            private set
            {
                var old = this.CanRedo;
                this.canRedo = value;
                if (value != old)
                {
                    this.OnCanRedoChanged();
                }
            }
        }

        private int MarkCounter
        {
            get => this.markCounter;

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
            var op = this.undoStack.Pop();
            op.Undo();
            this.redoStack.Push(op);

            this.MarkCounter -= 1;

            this.RefreshProperties();
        }

        public void Redo()
        {
            var op = this.redoStack.Pop();
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
            this.CanRedoChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnCanUndoChanged()
        {
            this.CanUndoChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnIsMarkedChanged()
        {
            this.IsMarkedChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
