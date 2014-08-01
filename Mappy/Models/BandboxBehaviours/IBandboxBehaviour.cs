namespace Mappy.Models.BandboxBehaviours
{
    using System.ComponentModel;
    using System.Drawing;

    public interface IBandboxBehaviour : INotifyPropertyChanged
    {
        Rectangle BandboxRectangle { get; }

        void StartBandbox(int x, int y);

        void GrowBandbox(int x, int y);

        void CommitBandbox();
    }
}