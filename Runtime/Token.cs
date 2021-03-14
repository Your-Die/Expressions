namespace Chinchillada.Expressions
{
    public class Token
    {
        public readonly TokenType Type;
        public readonly string Lexeme;
        public readonly object Literal;
        public readonly int Line;

        public Token(TokenType type, string lexeme, object literal, int line)
        {
            this.Type = type;
            this.Lexeme = lexeme;
            this.Literal = literal;
            this.Line = line;
        }

        public override string ToString() => $"{this.Type} {this.Lexeme} {this.Literal}";

        public static Token EndOfFile(int line) => new Token(TokenType.EOF, string.Empty, null, line);
    }
}