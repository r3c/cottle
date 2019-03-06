using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Parsers.Post;

namespace Cottle.Parsers
{
    internal class PostParser : IParser
    {
        #region Constructors

        public PostParser(IParser parser, IEnumerable<IOptimizer> optimizers)
        {
            _optimizers = optimizers.ToArray();
            _parser = parser;
        }

        #endregion

        #region Methods / Public

        public Command Parse(TextReader reader)
        {
            return Optimize(_parser.Parse(reader));
        }

        #endregion

        #region Attributes

        private readonly IOptimizer[] _optimizers;

        private readonly IParser _parser;

        #endregion

        #region Methods / Private

        private Command Optimize(Command command)
        {
            foreach (var optimizer in _optimizers)
                command = optimizer.Optimize(command);

            if (command.Body != null)
                command.Body = Optimize(command.Body);

            if (command.Next != null)
                command.Next = Optimize(command.Next);

            if (command.Operand != null)
                command.Operand = Optimize(command.Operand);

            return command;
        }

        private Expression Optimize(Expression expression)
        {
            foreach (var optimizer in _optimizers)
                expression = optimizer.Optimize(expression);

            if (expression.Arguments != null)
                for (var i = 0; i < expression.Arguments.Length; ++i)
                    expression.Arguments[i] = Optimize(expression.Arguments[i]);

            if (expression.Elements != null)
                for (var i = 0; i < expression.Elements.Length; ++i)
                    expression.Elements[i] = new ExpressionElement(Optimize(expression.Elements[i].Key), Optimize(expression.Elements[i].Value));

            if (expression.Source != null)
                expression.Source = Optimize(expression.Source);

            if (expression.Subscript != null)
                expression.Subscript = Optimize(expression.Subscript);

            return expression;
        }

        #endregion
    }
}