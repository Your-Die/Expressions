namespace Chinchillada.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Parser
    {
        private readonly IList<Token> tokens;

        private readonly ILog log;

        private int currentIndex = 0;

        private Token Previous => this.tokens[this.currentIndex - 1];

        private Token Current => this.tokens[this.currentIndex];

        private bool IsAtEnd => this.Current.Type == TokenType.EOF;


        public Parser(IList<Token> tokens, ILog log = null)
        {
            this.tokens = tokens;
            this.log    = log ?? new VoidLog();
        }

        public Expr Parse() => this.ParseExpression();

        #region Expressions

        private Expr ParseExpression()
        {
            return this.ParseOr();
        }

        private Expr ParseOr()
        {
            var expr = this.ParseAnd();

            while (this.Match(TokenType.OR))
            {
                var @operator = this.Previous;
                var right     = this.ParseAnd();

                expr = new Expr.Logical(expr, @operator, right);
            }

            return expr;
        }

        private Expr ParseAnd()
        {
            var expr = this.ParseEquality();

            while (this.Match(TokenType.AND))
            {
                var @operator = this.Previous;
                var right     = this.ParseEquality();

                expr = new Expr.Logical(expr, @operator, right);
            }

            return expr;
        }

        private Expr ParseEquality()
        {
            var expr = this.ParseComparison();

            while (this.Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                var @operator = this.Previous;
                var right     = this.ParseComparison();

                expr = new Expr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr ParseComparison()
        {
            var expr = this.ParseTerm();

            while (this.Match(
                TokenType.GREATER,
                TokenType.GREATER_EQUAL,
                TokenType.LESS,
                TokenType.LESS_EQUAL))
            {
                var @operator = this.Previous;
                var right     = this.ParseTerm();

                expr = new Expr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr ParseTerm()
        {
            var expr = this.ParseFactor();

            while (this.Match(TokenType.MINUS, TokenType.PLUS))
            {
                var @operator = this.Previous;
                var right     = this.ParseFactor();

                expr = new Expr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr ParseFactor()
        {
            var expr = this.ParseUnary();

            while (this.Match(TokenType.SLASH, TokenType.STAR))
            {
                var @operator = this.Previous;
                var right     = this.ParseUnary();

                expr = new Expr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr ParseUnary()
        {
            if (!this.Match(TokenType.BANG, TokenType.MINUS))
                return this.ParsePrimary();

            var @operator = this.Previous;
            var right     = this.ParseUnary();

            return new Expr.Unary(@operator, right);
        }

        private Expr ParsePrimary()
        {
            if (this.Match(TokenType.FALSE))
                return new Expr.Literal(false);
            if (this.Match(TokenType.TRUE))
                return new Expr.Literal(true);

            if (this.Match(TokenType.NUMBER))
                return new Expr.Literal(this.Previous.Literal);

            if (this.Match(TokenType.IDENTIFIER))
                return new Expr.Variable(this.Previous);

            if (this.Match(TokenType.LEFT_PAREN))
            {
                var expr = this.ParseExpression();
                this.Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            throw this.GenerateError(this.Current, "Expect expression.");
        }

        #endregion

        #region Helpers

        private bool Match(params TokenType[] tokenTypes)
        {
            if (!tokenTypes.Any(this.Check))
                return false;

            this.Advance();
            return true;
        }

        private Token Advance()
        {
            if (!this.IsAtEnd)
                this.currentIndex++;

            return this.Previous;
        }

        private bool Check(TokenType type)
        {
            if (this.IsAtEnd)
                return false;

            return this.Current.Type == type;
        }

        private Token Consume(TokenType type, string errorMessage)
        {
            if (this.Check(type))
                return this.Advance();

            throw this.GenerateError(this.Current, errorMessage);
        }

        #endregion

        private Exception GenerateError(Token token, string errorMessage)
        {
            this.log.Error(token, errorMessage);
            return new ParseError();
        }
    }

    internal class ParseError : Exception
    {
    }
}