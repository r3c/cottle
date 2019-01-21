﻿using System.IO;

namespace Cottle.Documents.Simple.Nodes.AssignNodes
{
    internal class RenderAssignNode : AssignNode
    {
        #region Attributes

        private readonly INode _body;

        #endregion

        #region Constructors

        public RenderAssignNode(string name, INode body, StoreMode mode) :
            base(name, mode)
        {
            _body = body;
        }

        #endregion

        #region Methods

        protected override Value Evaluate(IStore store, TextWriter output)
        {
            using (var buffer = new StringWriter())
            {
                store.Enter();

                _body.Render(store, buffer, out _);

                store.Leave();

                return buffer.ToString();
            }
        }

        protected override void SourceSymbol(string name, TextWriter output)
        {
            output.Write(name);
        }

        protected override void SourceValue(ISetting setting, TextWriter output)
        {
            _body.Source(setting, output);
        }

        #endregion
    }
}