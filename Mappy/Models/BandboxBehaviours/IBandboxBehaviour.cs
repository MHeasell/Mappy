namespace Mappy.Models.BandboxBehaviours
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Mappy.UI.Controls;

    public interface IBandboxBehaviour : INotifyPropertyChanged
    {
        Rectangle BandboxRectangle { get; }

        Point BandboxStart { get; }

        Point BandboxFinish { get; }

        void StartBandbox(int x, int y);

        void GrowBandbox(int x, int y);

        void CommitBandbox(ActiveTab activeTab);
    }
}