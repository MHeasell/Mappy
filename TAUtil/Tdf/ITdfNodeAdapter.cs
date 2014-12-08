namespace TAUtil.Tdf
{
    public interface ITdfNodeAdapter
    {
        void BeginBlock(string name);

        void AddProperty(string name, string value);

        void EndBlock();

        void Initialize(TdfParser parser);
    }
}
