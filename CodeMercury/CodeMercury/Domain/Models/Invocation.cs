﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    public class Invocation
    {
        public Argument Object { get; private set; }
        public Method Method { get; private set; }
        public IReadOnlyCollection<Argument> Arguments { get; private set; }

        public Invocation(Argument @object, Method method, IEnumerable<Argument> arguments)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }

            this.Object = @object;
            this.Method = method;
            this.Arguments = arguments.ToList().AsReadOnly();
        }
    }
}
