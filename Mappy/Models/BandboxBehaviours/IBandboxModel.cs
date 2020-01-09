namespace Mappy.Models.BandboxBehaviours
{
    public interface IBandboxModel
    {
        void LiftAndSelectSectionArea(int x, int y, int width, int height);

        void LiftAndSelectFeatureArea(int x, int y, int width, int height);
    }
}