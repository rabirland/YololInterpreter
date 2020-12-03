namespace Yolol.AST.Nodes
{
	/// <summary>
	/// Represents a Yolol network variable.
	/// </summary>
	public record GlobalVariableNode : IValueNode, IValueHolderNode
	{
		/// <summary>
		/// The name of the global variable (without the prefixing ':')
		/// </summary>
		public string Name { get; init; }

		/// <inheritdoc/>
		public string ToCode() => ':' + this.Name;
	}
}
