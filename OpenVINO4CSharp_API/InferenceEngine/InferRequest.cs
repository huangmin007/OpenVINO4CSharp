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
        public InferRequest()
        {
        }

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
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public IEStatusCode Wait(ulong timeout)
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

        /// <summary>
        /// 设置异步推断完成回调函数
        /// </summary>
        /// <returns></returns>
        public bool SetCompletionCallback()
        {
            
            Console.WriteLine(Marshal.SizeOf(typeof(ie_complete_call_back)));
            ie_complete_call_back callback = new ie_complete_call_back();
            callback.CallbackFunc = Marshal.GetFunctionPointerForDelegate((CompleteCallBackFunc)((IntPtr args) =>
            {
                Console.WriteLine("test");
            }));
            //callback.Args = new IntPtr();

            Console.WriteLine(IE_C_API.ie_infer_set_completion_callback(ptr, callback));
            return true;
        }


    }
}
