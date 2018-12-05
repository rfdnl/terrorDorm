using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrorWindows.Mapping
{
    public class AreaNode : VertexNode
    {
        public AreaNode(int x, int y, List<GameObject> objects) : base(x, y, objects)
        {
            Texture = Game1.content.GetTexture("AreaNode");
        }
    }
}
