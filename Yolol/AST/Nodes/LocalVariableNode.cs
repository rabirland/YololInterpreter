namespace Yolol.AST.Nodes
{
	/// <summary>
	/// Represents a Yolol chip local variable.
	/// </summary>
	public record LocalVariableNode : IValueNode, IValueHolderNode
	{
		/// <summary>
		/// The name of the local variable.
		/// </summary>
		public string Name { get; init; }

		/// <inheritdoc/>
		public string ToCode() => this.Name;
	}
}
