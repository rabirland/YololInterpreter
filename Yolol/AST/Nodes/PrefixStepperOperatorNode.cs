namespace Yolol.AST.Nodes
{
	/// <summary>
	/// Represents a prefix operator that changes the value of a variable while returning the modified value.
	/// </summary>
	public record PrefixStepperOperatorNode : IValueNode
	{
		/// <summary>
		/// The target storage node.
		/// </summary>
		public IValueHolderNode Target { get; init; }

		/// <summary>
		/// The type of the operator.
		/// </summary>
		public StepperOperatorType Type { get; init; }

		/// <inheritdoc/>
		public string ToCode() => "TODO";
	}
}
