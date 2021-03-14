namespace Chinchillada.Expressions
{
    using System;

    internal class Interpreter :
        Expr.IVisitor<object>
    {
        private readonly ILog log;

        public Interpreter(ILog log = null)
        {
            this.log = log ?? new VoidLog();
        }

        public object Interpret(Expr expression)
        {
            object result = null;
            try
            {
                result = this.Evaluate(expression);
            }
            catch (RuntimeError error)
            {
                this.log.RuntimeError(error);
            }

            return result;
        }

        private object Evaluate(Expr expression) => expression.Accept(this);

        object Expr.IVisitor<object>.VisitBinaryExpr(Expr.Binary expr)
        {
            var left  = this.Evaluate(expr.left);
            var right = this.Evaluate(expr.right);

            switch (expr.@operator.Type)
            {
                case TokenType.GREATER:
                    this.CheckNumberOperands(expr.@operator, left, right);
                    return (float) left > (float) right;
                case TokenType.GREATER_EQUAL:
                    this.CheckNumberOperands(expr.@operator, left, right);
                    return (float) left >= (float) right;
                case TokenType.LESS:
                    this.CheckNumberOperands(expr.@operator, left, right);
                    return (float) left < (float) right;
                case TokenType.LESS_EQUAL:
                    this.CheckNumberOperands(expr.@operator, left, right);
                    return (float) left <= (float) right;
                case TokenType.MINUS:
                    this.CheckNumberOperands(expr.@operator, left, right);
                    return (float) left - (float) right;
                case TokenType.SLASH:
                    this.CheckNumberOperands(expr.@operator, left, right);
                    return (float) left / (float) right;
                case TokenType.STAR:
                    this.CheckNumberOperands(expr.@operator, left, right);
                    return (float) left * (float) right;

                case TokenType.BANG_EQUAL:  return !AreEqual(left, right);
                case TokenType.EQUAL_EQUAL: return AreEqual(left, right);

                case TokenType.PLUS:
                    if (left is float leftValue && right is float rightValue)
                        return leftValue + rightValue;

                    if (left is string leftString && right is string rightString)
                        return leftString + rightString;

                    throw new RuntimeError(expr.@operator, "Operands must be two numbers or two strings.");
            }

            return null;
        }

        object Expr.IVisitor<object>.VisitGroupingExpr(Expr.Grouping expr)
        {
            return this.Evaluate(expr.expression);
        }

        object Expr.IVisitor<object>.VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        object Expr.IVisitor<object>.VisitLogicalExpr(Expr.Logical expr)
        {
            var left = this.Evaluate(expr.left);
            if (expr.@operator.Type == TokenType.OR)
            {
                if (IsTruthy(left))
                    return left;
            }
            else
            {
                if (!IsTruthy(left))
                    return left;
            }

            var right = this.Evaluate(expr.right);
            return IsTruthy(right);
        }

        object Expr.IVisitor<object>.VisitUnaryExpr(Expr.Unary expr)
        {
            var right = this.Evaluate(expr.right);

            switch (expr.@operator.Type)
            {
                case TokenType.MINUS:
                    this.CheckNumberOperand(expr.@operator, right);
                    var value = (float) right;
                    return -value;

                case TokenType.BANG:
                    return !IsTruthy(right);
            }

            return null;
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            throw new NotImplementedException();
        }

        private static bool IsTruthy(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case bool value:
                    return value;
                default:
                    return true;
            }
        }

        private static bool AreEqual(object a, object b)
        {
            return a == null && b == null ||
                   a != null && a.Equals(b);
        }

        private void CheckNumberOperand(Token @operator, object operand)
        {
            if (operand is float)
                return;

            throw new RuntimeError(@operator, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token @operator, object left, object right)
        {
            if (left is float && right is float)
                return;

            throw new RuntimeError(@operator, "Operand must be a number.");
        }

        private static string Stringify(object value)
        {
            if (value == null)
                return "nil";

            if (value is float)
            {
                var text = value.ToString();

                if (text.EndsWith(".0"))
                    text = text.Substring(0, text.Length - 2);

                return text;
            }

            return value.ToString();
        }
    }

    public class Return : Exception
    {
        public object Value { get; }

        public Return(object value) => this.Value = value;
    }
}