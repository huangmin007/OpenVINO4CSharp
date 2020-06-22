using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InferenceEngine
{
    public class Blob : IntPtrObject
    {
        private string name;

        internal Blob(IntPtr ptr, FreeIntPtrDelegate freeFunc) : base(ptr, freeFunc)
        {
        }

    }
}
