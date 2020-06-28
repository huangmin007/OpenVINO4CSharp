using System;

namespace InferenceEngine
{
    /// <summary>
    /// Inference Engine Blob Object
    /// </summary>
    public sealed class Blob : IntPtrObject
    {
        /// <inheritdoc />
        internal Blob(IntPtr ptr, FreeIntPtrDelegate freeFunc) : base(ptr, freeFunc)
        {
        }

        #region Read Only Properties
        /// <summary>
        /// Bolb 元素的总数，它是所有维度的乘积。
        /// </summary>
        /// <returns></returns>
        public int Size
        {
            get
            {
                int size;
                IE_C_API.ie_blob_size(ptr, out size);
                return size;
            }
        }

        /// <summary>
        /// Blob 的大小（以字节为单位）。
        /// </summary>
        /// <returns></returns>
        public int ByteSize
        {
            get
            {
                int size;
                IE_C_API.ie_blob_byte_size(ptr, out size);
                return size;
            }
        }

        /// <summary>
        /// Blob 张量的维数.
        /// </summary>
        public Dimensions Dims
        {
            get
            {
                Dimensions dims;
                IE_C_API.ie_blob_get_dims(ptr, out dims);
                return dims;
            }
        }

        /// <summary>
        /// Blob 张量的布局
        /// </summary>
        public Layout Layout
        {
            get
            {
                Layout layout;
                IE_C_API.ie_blob_get_layout(ptr, out layout);
                return layout;
            }
        }

        /// <summary>
        /// Blob 张量的精度
        /// </summary>
        public Precision Precision
        {
            get
            {
                Precision precision;
                IE_C_API.ie_blob_get_precision(ptr, out precision);
                return precision;
            }
        }

        /// <summary>
        /// 获取对分配的内存的只读访问权限
        /// </summary>
        /// <returns></returns>
        public IntPtr Buffer
        {
            get
            {
                IntPtr buffer;
                IE_C_API.ie_blob_get_cbuffer(ptr, out buffer);
                return buffer;
            }
        }
        #endregion

        /// <summary>
        /// 获取对分配的内存的访问
        /// </summary>
        /// <returns></returns>
        public IntPtr GetBuffer()
        {
            IntPtr buffer;
            IE_C_API.ie_blob_get_buffer(ptr, out buffer); 
            return buffer;
        }


        public override string ToString()
        {
            return $"{base.ToString()}, Size:{Size}, ByteSize:{ByteSize}, Layout:{Layout}, Dims:{Dims}, Precision:{Precision}";
        }

        #region Blob Static Functions
        /// <summary>
        /// 创建具有指定尺寸，布局的 blob 并分配内存。
        /// </summary>
        /// <param name="tensor"></param>
        /// <returns></returns>
        public static Blob MakeMemory(ref TensorDesc tensor)
        {
            IntPtr blob;
            if (IE_C_API.ie_blob_make_memory(ref tensor, out blob) != IEStatusCode.OK)
                throw new Exception("创建 " + typeof(Blob).FullName + " 失败");
            
            return new Blob(blob, IE_C_API.ie_blob_free);
        }

        /// <summary>
        /// 使用给定张量描述符，从指向预分配内存的指针，创建一个 Blob。
        /// </summary>
        /// <param name="tensor">用于创建 Blob 的张量描述符</param>
        /// <param name="input">指向预分配内存的指针</param>
        /// <param name="size">预分配指针数据(数组)的长度</param>
        /// <returns></returns>
        public static Blob MakeMemoryFromPreallocated(ref TensorDesc tensor, IntPtr input, ulong size)
        {
            IntPtr blob;
            if (IE_C_API.ie_blob_make_memory_from_preallocated(ref tensor, input, size, out blob) != IEStatusCode.OK)
                throw new Exception("创建 " + typeof(Blob).FullName + " 失败"); 

            return new Blob(blob, IE_C_API.ie_blob_free);
        }

        /// <summary>
        /// 使用给定张量描述符，从指向预分配内存的数组，创建一个 Blob。
        /// </summary>
        /// <param name="tensor">用于创建 Blob 的张量描述符</param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Blob MakeMemoryFromPreallocated(ref TensorDesc tensor, byte[] input)
        {
            IntPtr blob;
            if (IE_C_API.ie_blob_make_memory_from_preallocated(ref tensor, input, (ulong)input.LongLength, out blob) != IEStatusCode.OK)
                throw new Exception("创建 " + typeof(Blob).FullName + " 失败");

            return new Blob(blob, IE_C_API.ie_blob_free);
        }

        /// <summary>
        /// 预分配内存的 ROI 创建一个 Blob 实例
        /// </summary>
        /// <param name="input"></param>
        /// <param name="roi"></param>
        /// <returns></returns>
        public static Blob MakeMemoryWithROI(IntPtr input, ref ROI roi)
        {
            IntPtr blob;
            if(IE_C_API.ie_blob_make_memory_with_roi(input, ref roi, out blob) != IEStatusCode.OK)
                throw new Exception("创建 " + typeof(Blob).FullName + " 失败");

            return new Blob(blob, IE_C_API.ie_blob_free);
        }

        /// <summary>
        /// Creates a NV12 blob from two planes Y and UV.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="uv"></param>
        /// <returns></returns>
        public static Blob MakeMemoryNV12(IntPtr y, IntPtr uv)
        {
            IntPtr blob;
            if (IE_C_API.ie_blob_make_memory_nv12(y, uv, out blob) != IEStatusCode.OK)
                throw new Exception("创建 " + typeof(Blob).FullName + " 失败");

            return new Blob(blob, IE_C_API.ie_blob_free);
        }

        /// <summary>
        /// Creates I420 blob from three planes Y, U and V.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Blob MakeMemoryI420(IntPtr y, IntPtr u, IntPtr v)
        {
            IntPtr blob;
            if (IE_C_API.ie_blob_make_memory_i420(y, u, v, out blob) != IEStatusCode.OK)
                throw new Exception("创建 " + typeof(Blob).FullName + " 失败");

            return new Blob(blob, IE_C_API.ie_blob_free);
        }
        #endregion
    }

    /// <summary>
    /// Blob Buffer 
    /// </summary>
    [Obsolete]
    public sealed class Buffer:IntPtrObject
    {
        /// <summary>
        /// 是否为只读 Buffer 对象
        /// </summary>
        public readonly bool ReadOnly = false;

        public IntPtr IntPtr => ptr;

        internal Buffer(IntPtr ptr, FreeIntPtrDelegate freeFunc, bool readOnly = false):base(ptr, freeFunc)
        {
            this.ReadOnly = readOnly;
        }
    }
}
