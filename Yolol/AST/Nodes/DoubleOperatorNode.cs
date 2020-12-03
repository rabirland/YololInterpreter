namespace Yolol.AST.Nodes
{
	/// <summary>
	/// Represents a two operand operator.
	/// </summary>
	public record DoubleOperatorNode : IValueNode
	{
		/// <summary>
		/// The left hand side of the operation.
		/// </summary>
		public IValueNode LeftHand { get; init; }
		
		/// <summary>
		/// The right hand side of the operation.
		/// </summary>
		public IValueNode RightHand { get; init; }

		/// <summary>
		/// The type of the operation.
		/// </summary>
		public TwoOperandOperatorType Type { get; init; }

		/// <inheritdoc/>
		public string ToCode() => this.LeftHand.ToCode() + " TODO " + this.RightHand.ToCode();
	}
}
