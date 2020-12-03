namespace Yolol.AST.Nodes
{
	/// <summary>
	/// Repersents an assignment.
	/// </summary>
	public record AssignmentNode : INode
	{
		/// <summary>
		/// The target storage of the assignment.
		/// </summary>
		public IValueHolderNode Target { get; init; }

		/// <summary>
		/// The value of the assignment.
		/// </summary>
		public IValueNode Souce { get; init; }

		/// <inheritdoc/>
		public string ToCode() => "TODO";
	}
}
