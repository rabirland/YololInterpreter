using System.Collections.Generic;

namespace Yolol.AST.Nodes
{
    /// <summary>
    /// Represents nodes that have child nodes.
    /// </summary>
    public interface ICollectionNodes
    {
        IEnumerable<INode> ChildNodes { get; }

        int ChildNodeCount { get; }
    }
}
