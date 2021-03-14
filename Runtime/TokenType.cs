namespace Chinchillada.Expressions
{
    public enum TokenType
    {
        // Single-character tokens.
        LEFT_PAREN, RIGHT_PAREN, DOT,
        MINUS, PLUS, SLASH, STAR,

        // One or two character tokens.
        BANG, BANG_EQUAL,
        EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL,

        // Literals.
        IDENTIFIER, NUMBER,

        // Keywords.
        AND, ELSE, FALSE, OR,
        TRUE,

        EOF
    }
}