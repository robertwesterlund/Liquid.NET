﻿using System;
using System.Collections.Generic;
using System.Linq;
using Liquid.NET.Constants;
using Liquid.NET.Symbols;

namespace Liquid.NET.Expressions
{
    public class AndExpression : ExpressionDescription
    {
        public override void Accept(IExpressionDescriptionVisitor expressionDescriptionVisitor)
        {
            expressionDescriptionVisitor.Visit(this);
        }

        public override IExpressionConstant Eval(SymbolTableStack symbolTableStack, IEnumerable<IExpressionConstant> expressions)
        {
            if (!expressions.Any() || expressions.Count() == 1)
            {
                throw new Exception("An AND expression must have two values"); // TODO: when the Eval is separated this will be redundant.
            }
            return new BooleanValue(expressions.All(x => x.IsTrue));
        }
    }
}
