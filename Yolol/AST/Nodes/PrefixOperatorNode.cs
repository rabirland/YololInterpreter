namespace Yolol.AST.Nodes
{
	/// <summary>
	/// Represents a prefix operator node.
	/// </summary>
	public record PrefixOperatorNode : IValueNode
	{
		/// <summary>
		/// The source value.
		/// </summary>
		public IValueNode Source { get; init; }

		/// <summary>
		/// The type of the operation.
		/// </summary>
		public PrefixOperatorType Type { get; init; }

		/// <inheritdoc/>
		public string ToCode() => "TODO";
	}
}
