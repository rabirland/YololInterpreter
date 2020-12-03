namespace Yolol.AST.Nodes
{
	/// <summary>
	/// Represents a postfix stepper operator that changes the value of a variable and returns the original value.
	/// </summary>
	public record PostfixStepperOperatorNode : IValueNode
	{
		/// <summary>
		/// The target storage node.
		/// </summary>
		public IValueHolderNode Target { get; init; }

		/// <summary>
		/// The type of the operator
		/// </summary>
		public StepperOperatorType Type { get; init; }

		/// <inheritdoc/>
		public string ToCode() => "TODO";
	}
}
