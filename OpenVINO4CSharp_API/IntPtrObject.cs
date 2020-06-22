using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace InferenceEngine
{
    /// <summary>
    /// 指针释放代理函数
    /// </summary>
    /// <param name="ptr"></param>
    /// <returns></returns>
    public delegate void FreeIntPtrDelegate(ref IntPtr ptr);

    /// <summary>
    /// IntPtr Object
    /// </summary>
    public class IntPtrObject : IDisposable
    {
        protected bool disposedValue;
        protected FreeIntPtrDelegate freeFunc;

        /// <summary>
        /// 源指针对对象
        /// </summary>
        protected internal IntPtr ptr;

        /// <summary>
        /// IE Status Code
        /// </summary>
        protected IEStatusCode status;

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~IntPtrObject()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public IntPtrObject()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ptr">对象指针</param>
        /// <param name="freeFunc">释放对象指针的代理函数</param>
        protected internal IntPtrObject(IntPtr ptr, FreeIntPtrDelegate freeFunc)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentNullException("参数不能为空", nameof(ptr));
            if (freeFunc == null)
                throw new ArgumentNullException("参数不能为空", nameof(freeFunc));

            this.ptr = ptr;
            this.freeFunc = freeFunc;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null

                if (ptr != IntPtr.Zero && freeFunc != null)
                {
                    freeFunc(ref ptr);
                    Console.WriteLine("Dispose {0} ...", this);
                }

                freeFunc = null;
                ptr = IntPtr.Zero;
                disposedValue = true;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.GetType().FullName;
        }
    }
}
