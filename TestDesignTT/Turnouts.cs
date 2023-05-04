using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDesignTT
{
    /// <summary>
    /// Trida, prej kterou jsou uchovavany data pro zmenu vyhybek
    /// </summary>
    public class Turnouts
    {
        public UInt32 UnitID { get; set; }
        public byte Change { get; set; }
        public byte Position { get; set; }
    }
}
