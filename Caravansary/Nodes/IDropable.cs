using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caravansary
{
    internal interface IDropable
    {
        string DataType { get; }

        void Drop(object data, int index = -1);
    }
}