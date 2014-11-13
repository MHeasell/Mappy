namespace Mappy.Views
{
    using System;

    public interface IProgressView
    {
        event EventHandler CancelPressed;

        string MessageText { get; set; }

        string Title { get; set; }

        int Progress { get; set; }

        void Display();

        void Close();
    }
}
