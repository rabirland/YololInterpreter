namespace Yolol.AST.Nodes
{
    /// <summary>
    /// Represent a value wrapped in brackets
    /// </summary>
    public record BracketValueNode : IValueNode
    {
        /// <summary>
        /// The value node in the bracket
        /// </summary>
        public IValueNode Value { get; init; }

        /// <inheritdoc/>
        public string ToCode() => "(TODO)";
    }
}
