using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace InferenceEngine
{
    /// <summary>
    /// Inference Engine InferRequest
    /// </summary>
    public class InferRequest : IntPtrObject
    {
        /// <inheritdoc/>
        internal InferRequest(IntPtr ptr, FreeIntPtrDelegate freeFunc):base(ptr, freeFunc)
        {
        }

        /// <summary>
        /// InferRequest
        /// </summary>
        //public InferRequest()
        //{}

        /// <summary>
        /// 设置推断批量大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool SetBatch(ulong size)
        {
            return IE_C_API.ie_infer_request_set_batch(ptr, size) == IEStatusCode.OK;
        }

        /// <summary>
        /// 获取网络层 Blob 对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Blob GetBlob(string name)
        {
            IntPtr blob;
            if(IE_C_API.ie_infer_request_get_blob(ptr, name, out blob) != IEStatusCode.OK)
                throw new Exception("创建" + typeof(Blob).FullName + " 异常");

            return new Blob(blob, IE_C_API.ie_blob_free);
        }

        /// <summary>
        /// 设置指定网络层 Blob 对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool SetBlob(string name, Blob blob)
        {
            return IE_C_API.ie_infer_request_set_blob(ptr, name, blob.ptr) == IEStatusCode.OK;
        }

        /// <summary>
        /// 启动同步推断 
        /// </summary>
        /// <returns></returns>
        public bool Infer()
        {
            return IE_C_API.ie_infer_request_infer(ptr) == IEStatusCode.OK;
        }

        /// <summary>
        /// 等待结果变为可用。阻塞直到指定的超时时间过去或结果变为可用（以先到者为准）。
        /// <para>0 STATUS_ONLY 不会阻塞或中断当前线程，而是立即返回推断状态</para>
        /// <para>-1 RESULT_READY 等到推理结果可用</para>
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public IEStatusCode Wait(long timeout)
        {
            return IE_C_API.ie_infer_request_wait(ptr, timeout);
        }


        /// <summary>
        /// 启动异步推断
        /// </summary>
        /// <returns></returns>
        public bool StartAsync()
        {
            return IE_C_API.ie_infer_request_infer_async(ptr) == IEStatusCode.OK;
        }

        private CompleteCallback ie_callback;
        private CallBackActionFunc callbackFunc;

        /// <summary>
        /// 设置异步推断完成回调函数
        /// </summary>
        /// <returns></returns>
        public bool SetCompletionCallback(CallBackActionFunc callback)
        {
            if (callback != null)
            {
                callbackFunc = callback;
                ie_callback = new CompleteCallback(Marshal.GetFunctionPointerForDelegate((CallBackAction)CallbackHandler), ptr);

                return IE_C_API.ie_infer_set_completion_callback(ptr, ref ie_callback) == IEStatusCode.OK;
            }

            return false;
        }
        private void CallbackHandler(IntPtr args)
        {
            callbackFunc?.Invoke(new InferRequest(args, null));
        }

    }
}
