namespace Yolol.AST.Nodes
{
	/// <summary>
	/// Represents a single parameter command (function) node.
	/// </summary>
	public record CommandNode : INode
	{
		/// <summary>
		/// The parameter of the command.
		/// </summary>
		public IValueNode Parameter { get; init; }

		/// <summary>
		/// The type of the command.
		/// </summary>
		public CommandType Type { get; init; }

		/// <inheritdoc/>
		public string ToCode() => "TODO";
	}
}
