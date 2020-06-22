using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InferenceEngine
{
    public class InferRequest : IntPtrObject
    {
        /// <inheritdoc/>
        internal InferRequest(IntPtr ptr, FreeIntPtrDelegate freeFunc):base(ptr, freeFunc)
        {
        }
        
        public InferRequest()
        {
        }

        public override string ToString()
        {
            return nameof(InferRequest);
        }
    }
}
