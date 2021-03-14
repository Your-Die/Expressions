namespace Chinchillada.Expressions
{
    using System.Collections.Generic;
    using System.Globalization;

    internal class Scanner
    {
        private readonly string      source;
        private readonly ILog        log;
        private readonly List<Token> tokens = new List<Token>();

        private int startIndex   = 0;
        private int currentIndex = 0;
        private int line         = 1;

        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            {"and", TokenType.AND},
            {"else", TokenType.ELSE},
            {"false", TokenType.FALSE},
            {"or", TokenType.OR},
            {"true", TokenType.TRUE},
        };

        private char Current => this.IsAtEnd ? '\0' : this.source[this.currentIndex];

        private char Next
        {
            get
            {
                var nextIndex = this.currentIndex + 1;
                return nextIndex > this.source.Length ? '\0' : this.source[nextIndex];
            }
        }

        private bool IsAtEnd => this.currentIndex >= this.source.Length;

        public Scanner(string source, ILog log = null)
        {
            this.source = source;
            this.log    = log ?? new VoidLog();
        }

        public List<Token> ScanTokens()
        {
            while (!this.IsAtEnd)
            {
                this.startIndex = this.currentIndex;
                this.ScanToken();
            }

            var endOfFile = Token.EndOfFile(this.line);
            this.tokens.Add(endOfFile);

            return this.tokens;
        }

        private void ScanToken()
        {
            var character = this.Advance();

            switch (character)
            {
                case '(':
                    this.AddToken( TokenType.LEFT_PAREN);
                    break;
                case ')':
                    this.AddToken( TokenType.RIGHT_PAREN);
                    break;
                case '.':
                    this.AddToken( TokenType.DOT);
                    break;
                case '-':
                    this.AddToken( TokenType.MINUS);
                    break;
                case '+':
                    this.AddToken( TokenType.PLUS);
                    break;
                case '*':
                    this.AddToken( TokenType.STAR);
                    break;

                case '!':
                    this.AddToken(this.Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    this.AddToken(this.Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    this.AddToken(this.Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    this.AddToken(this.Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;

                case '/':
                    if (this.Match('/'))
                    {
                        while (this.Current != '\n' && !this.IsAtEnd)
                        {
                            this.Advance();
                        }
                    }
                    else
                    {
                        this.AddToken(TokenType.SLASH);
                    }
                    break;

                case ' ':
                case '\r':
                case '\t':
                    break;

                case '\n':
                    this.line++;
                    break;

                default:
                    if (IsDigit(character))
                    {
                        this.ScanNumber();
                    }
                    else if (IsAlpha(character))
                        this.ScanIdentifier();
                    else
                        this.log.Error(this.line, $"Unexpected character: '{character}'");

                    break;
            }
        }

        private void ScanIdentifier()
        {
            while (IsAlphaNumeric(this.Current))
                this.Advance();

            var text = this.GetCurrentSubstring();

            if (!Keywords.TryGetValue(text, out var tokenType))
                tokenType = TokenType.IDENTIFIER;

            this.AddToken(tokenType);
        }

        private void ScanNumber()
        {
            // Get whole numbers.
            while (IsDigit(this.Current))
                this.Advance();

            // optional fractional part.
            if (this.Current == '.' && IsDigit(this.Next))
            {
                // Consume the dot.
                this.Advance();

                // digits after the dot.
                while (IsDigit(this.Current))
                    this.Advance();
            }

            var text  = this.GetCurrentSubstring();
            var value = float.Parse(text, CultureInfo.InvariantCulture);

            this.AddToken(TokenType.NUMBER, value);
        }

        private bool Match(char expected)
        {
            if (this.IsAtEnd)
                return false;

            if (this.source[this.currentIndex] != expected)
                return false;

            this.currentIndex++;
            return true;
        }

        private char Advance()
        {
            this.currentIndex++;
            return this.source[this.currentIndex - 1];
        }

        private void AddToken(TokenType tokenType, object literal = null)
        {
            var text = this.GetCurrentSubstring();

            var token = new Token(tokenType, text, literal, this.line);
            this.tokens.Add(token);
        }

        private string GetCurrentSubstring()
        {
            var length = this.currentIndex - this.startIndex;
            return this.source.Substring(this.startIndex, length);
        }

        private static bool IsDigit(char character) => character >= '0' && character <= '9';

        private static bool IsAlpha(char character)
        {
            return character >= 'a' && character <= 'z' ||
                   character >= 'A' && character <= 'Z' ||
                   character == '_';
        }

        private static bool IsAlphaNumeric(char character) => IsAlpha(character) || IsDigit(character);
    }
}