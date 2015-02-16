using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crumble
{
    public class Channel
    {
        public uint Id { get; internal set; }
        public uint Parent { get; internal set; }
        public string Name { get; internal set; }
        public List<uint> Links { get; internal set; }
        public string Description { get; internal set; }
        public bool Temporary { get; internal set; }
        public int Position { get; internal set; }
    }
}
