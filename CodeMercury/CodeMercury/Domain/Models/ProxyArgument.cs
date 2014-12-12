using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    public class ProxyArgument : Argument
    {
        public Guid Id { get; private set; }

        public ProxyArgument(Guid id)
        {
            this.Id = id;
        }
    }
}
