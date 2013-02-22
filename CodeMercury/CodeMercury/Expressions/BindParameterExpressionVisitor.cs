using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace CodeMercury.Expressions
{
    public class BindParameterExpressionVisitor : ExpressionVisitor
    {
        public ParameterExpression ParameterExpression { get; set; }
        public object ParameterValue { get; set; }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node != ParameterExpression)
                return node;
            return Expression.Constant(ParameterValue);
        }
    }
}
