using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InferenceEngine
{
    public class ExecutableNetwork : IntPtrObject
    {
        public ExecutableNetwork()
        {
        }

        /// <inheritdoc/>
        internal ExecutableNetwork(IntPtr ptr, FreeIntPtrDelegate freeFunc):base(ptr, freeFunc)
        {
        }

        public InferRequest CreateInferRequest()
        {
            IntPtr request;
            status = IE_C_API.ie_exec_network_create_infer_request(ptr, out request);
            if (status != IEStatusCode.OK) throw new Exception("创建 " + typeof(InferRequest).FullName + " 异常");

            return new InferRequest(request, IE_C_API.ie_infer_request_free);
        }

        public Parameter GetMetric(string metricName)
        {
            Parameter param;
            IE_C_API.ie_exec_network_get_metric(ptr, metricName, out param);

            return param;
        }

        public Parameter GetConfig(string metricConfig)
        {
            Parameter param;
            IE_C_API.ie_exec_network_get_config(ptr, metricConfig, out param);

            return param;
        }

        public bool SetConfig(CoreConfig config)
        {
            return IE_C_API.ie_exec_network_set_config(ptr, config) == IEStatusCode.OK;
        }

    }
}
