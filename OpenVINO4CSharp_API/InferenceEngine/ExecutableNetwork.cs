using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InferenceEngine
{
    /// <summary>
    /// Inference Engine ExecutableNetwork
    /// </summary>
    public class ExecutableNetwork : IntPtrObject
    {
        /// <summary>
        /// ExecutableNetwork
        /// </summary>
        public ExecutableNetwork()
        {
        }

        /// <inheritdoc/>
        internal ExecutableNetwork(IntPtr ptr, FreeIntPtrDelegate freeFunc):base(ptr, freeFunc)
        {
        }

        /// <summary>
        /// 创建推断请求
        /// </summary>
        /// <returns></returns>
        public InferRequest CreateInferRequest()
        {
            IntPtr request;
            status = IE_C_API.ie_exec_network_create_infer_request(ptr, out request);
            if (status != IEStatusCode.OK)
                throw new Exception("创建 " + typeof(InferRequest).FullName + " 异常");

            return new InferRequest(request, IE_C_API.ie_infer_request_free);
        }

        /// <summary>
        /// 获取可执行网络的常规运行时指标。它可以是网络名称，运行可执行网络的实际设备 ID 或所有其他不能动态更改的属性。
        /// </summary>
        /// <param name="metricName"></param>
        /// <returns></returns>
        public Parameter GetMetric(string metricName)
        {
            Parameter param;
            IE_C_API.ie_exec_network_get_metric(ptr, metricName, out param);

            return param;
        }

        /// <summary>
        /// 获取当前可执行网络的配置。该方法负责提取影响可执行网络执行的信息。
        /// </summary>
        /// <param name="metricConfig"></param>
        /// <returns></returns>
        public Parameter GetConfig(string metricConfig)
        {
            Parameter param = null;
            IE_C_API.ie_exec_network_get_config(ptr, metricConfig, param);

            return param;
        }

        /// <summary>
        /// 设置当前可执行网络的配置。当网络在多设备上运行且配置参数只能是 "MULTI_DEVICE_PRIORITIES" 时，可以使用该方法。
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool SetConfig(CoreConfig config)
        {
            return IE_C_API.ie_exec_network_set_config(ptr, config) == IEStatusCode.OK;
        }

    }
}
