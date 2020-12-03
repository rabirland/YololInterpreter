namespace Yolol.AST.Nodes
{
	/// <summary>
	/// Represents an if-then-else branch
	/// </summary>
	public record IfThenElseNode : INode
	{
		/// <summary>
		/// The then branch.
		/// </summary>
		public INode Then { get; init; }

		/// <summary>
		/// The else branch.
		/// </summary>
		public INode Else { get; init; }

		/// <inheritdoc/>
		public string ToCode() => "TODO";
	}
}
