using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace InferenceEngine
{
    /// <summary>
    /// InferenceEngine CNNNetwork
    /// </summary>
    public class CNNNetwork : IntPtrObject
    {
        /// <summary>
        /// 
        /// </summary>
        public CNNNetwork()
        {
        }

        /// <inheritdoc/>
        internal CNNNetwork(IntPtr ptr, FreeIntPtrDelegate freeFunc) : base(ptr, freeFunc)
        {
        }

        /// <summary>
        /// 获取网络输入层名称
        /// </summary>
        /// <returns></returns>
        public string[] GetInputNames()
        {
            ulong inputCount;
            status = IE_C_API.ie_network_get_inputs_number(ptr, out inputCount);
            if (status != IEStatusCode.OK) return null;

            string[] inputNames = new string[inputCount];
            for(ulong i = 0; i < inputCount; i ++)
                status |= IE_C_API.ie_network_get_input_name(ptr, i, out inputNames[i]);

            if (status != IEStatusCode.OK) return null;

            return inputNames;
        }

        public Precision GetPrecision(string inputName)
        {
            Precision result;
            status = IE_C_API.ie_network_get_input_precision(ptr, inputName, out result);
            if (status != IEStatusCode.OK) throw new Exception("获取网络输入层精度错误");

            return result;
        }
        public bool SetPrecision(string inputName, Precision precision)
        {
            return IE_C_API.ie_network_set_input_precision(ptr, inputName, precision) == IEStatusCode.OK;
        }

        public Layout GetLayout(string inputName)
        {
            Layout layout;
            status = IE_C_API.ie_network_get_input_layout(ptr, inputName, out layout);
            if (status != IEStatusCode.OK) throw new Exception("获取网络输入层布局错误");

            return layout;
        }

        public bool SetLayout(string inputName, Layout layout)
        {
            return IE_C_API.ie_network_set_input_layout(ptr, inputName, layout) == IEStatusCode.OK;
        }


        public string GetName()
        {
            string name;
            IE_C_API.ie_network_get_name(ptr, out name);
            return name;
        }


        
    }
}
