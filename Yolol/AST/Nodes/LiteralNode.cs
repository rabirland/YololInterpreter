namespace Yolol.AST.Nodes
{
	public record LiteralNode : IValueNode
	{
		/// <summary>
		/// The value of the literal. In case of text, without the wrapping quotions.
		/// In case of numbers, the more than 3 digits stripped.
		/// </summary>
		public string Value { get; init; }

		/// <summary>
		/// The type of the literal.
		/// </summary>
		public LiteralType Type { get; init; }

		/// <inheritdoc/>
		public string ToCode() => "TODO";
	}
}
