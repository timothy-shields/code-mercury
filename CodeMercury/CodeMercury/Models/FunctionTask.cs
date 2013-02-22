using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CodeMercury.Models
{
    /// <summary>
    /// A handle to an executing FunctionApplication.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    public class FunctionTask<T>
    {
        public Guid Identity { get; set; }
        public Type Type
        {
            get { return typeof(T); }
            set { }
        }
        public FunctionResult Result { get; set; }

        public static FunctionTask<T> FromResultValue(FunctionResult result)
        {
            return new FunctionTask<T>
            {
                Identity = Guid.NewGuid(),
                Result = result
            };
        }
    }
}
