using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public class Gizmo
    {
        public int Id { get; private set; }
        public string Content { get; private set; }
        public bool IsWidget { get; private set; }

        public Gizmo(int id, string content, bool isWidget)
        {
            Id = id;
            Content = content;
            IsWidget = isWidget;
        }
    }
}
