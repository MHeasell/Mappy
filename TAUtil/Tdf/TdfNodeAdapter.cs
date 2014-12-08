namespace TAUtil.Tdf
{
    using System.Collections.Generic;

    public class TdfNodeAdapter : ITdfNodeAdapter
    {
        private readonly Stack<TdfNode> nodeStack = new Stack<TdfNode>();

        public TdfNodeAdapter()
        {
            this.RootNode = new TdfNode();
            this.nodeStack.Push(this.RootNode);
        }

        public TdfNode RootNode { get; private set; }

        public void Initialize(TdfParser parser)
        {
        }

        public void BeginBlock(string name)
        {
            TdfNode n = new TdfNode(name);
            this.nodeStack.Peek().Keys[name] = n;
            this.nodeStack.Push(n);
        }

        public void AddProperty(string name, string value)
        {
            this.nodeStack.Peek().Entries[name] = value;
        }

        public void EndBlock()
        {
            this.nodeStack.Pop();
        }
    }
}
