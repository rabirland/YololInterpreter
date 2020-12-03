namespace Yolol.Grammar
{
    /// <summary>
    /// A lex in the code.
    /// </summary>
    public readonly struct Lex
    {
        /// <summary>
        /// The original part of the code.
        /// </summary>
        public string Part { get; init; }

        /// <summary>
        /// The index of the first character of the part.
        /// </summary>
        public int Start { get; init; }

        /// <summary>
        /// The amount of characters in the code part.
        /// </summary>
        public int Length { get; init; }

        /// <summary>
        /// The type of the lex.
        /// </summary>
        public LexType Type { get; init; }

        public override string ToString()
        {
            return $"{{ {Part} [{Type}] }}";
        }
    }
}
