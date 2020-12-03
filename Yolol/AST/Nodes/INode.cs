namespace Yolol.AST.Nodes
{
	public interface INode
	{
		/// <summary>
		/// Generates a Yolol code from this node.
		/// </summary>
		/// <returns>The yolol code string.</returns>
		string ToCode();
	}
}
