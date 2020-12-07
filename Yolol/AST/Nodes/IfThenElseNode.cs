using System.Collections.Generic;

namespace Yolol.AST.Nodes
{
	/// <summary>
	/// Represents an if-then-else branch
	/// </summary>
	public record IfThenElseNode : INode
	{
		public IValueNode Condition { get; init; }

		/// <summary>
		/// The then branch.
		/// </summary>
		public IEnumerable<INode> ThenNodes { get; init; }

		/// <summary>
		/// The else branch.
		/// </summary>
		public IEnumerable<INode> ElseNodes { get; init; }

		/// <inheritdoc/>
		public string ToCode() => "TODO";
	}
}
