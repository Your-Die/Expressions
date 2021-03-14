namespace Chinchillada.Expressions.Tests
{
    using NUnit.Framework;

    public static class EvaluatorTests
    {
        private static float[] values = {1, 167, 0, -14, 5.156f, 78.143f};

        [Test]
        public static void AdditionTest([ValueSource(nameof(values))] float x, [ValueSource(nameof(values))] float y )
        {
            var expected = x + y;
            var actual = Evaluator.Evaluate<float>($"{x} + {y}");

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}