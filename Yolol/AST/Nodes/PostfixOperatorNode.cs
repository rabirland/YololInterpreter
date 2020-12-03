namespace Yolol.AST.Nodes
{
	/// <summary>
	/// Represents a postfix operator
	/// </summary>
	public record PostfixOperatorNode : IValueNode
	{
		/// <summary>
		/// The source value.
		/// </summary>
		public IValueNode Source { get; init; }

		/// <summary>
		/// The type of the operator.
		/// </summary>
		public PostfixOperatorType Type { get; init; }

		/// <inheritdoc/>
		public string ToCode() => "TODO";
	}
}
