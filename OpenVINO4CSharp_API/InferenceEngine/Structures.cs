using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace InferenceEngine
{
    #region Structures
    /// <summary>
    /// 结构体数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct StructArray //<T> where T: struct
    {
        /// <summary>
        /// 数组数据指针
        /// </summary>
        private IntPtr ptr;
        /// <summary>
        /// 数组长度
        /// </summary>
        private ulong length;

        /// <summary>
        /// 结构体数组
        /// </summary>
        /// <param name="structureArray">结构体数组</param>
        public StructArray(object structureArray)
        {
            if (!(structureArray is Array))
                throw new ArgumentException("参数错误");

            Array array = (Array)structureArray;
            this.length = (ulong)array.Length;

            int offset = Marshal.SizeOf((structureArray as Array).GetValue(0));
            Console.WriteLine(offset);
            ptr = Marshal.AllocHGlobal(offset * array.Length);

            for (int i = 0; i < array.Length; i++)
                Marshal.StructureToPtr(array.GetValue(i), ptr + (i * offset), true);
        }

        /// <summary>
        /// 设置数组数据
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="array"></param>
        public void SetArray<TStruct>(TStruct[] array) where TStruct : struct
        {
            length = (ulong)array.Length;

            int offset = Marshal.SizeOf(array[0]);
            ptr = Marshal.AllocHGlobal(offset * array.Length);

            for (int i = 0; i < array.Length; i++)
                Marshal.StructureToPtr(array[i], ptr + (i * offset), false);
        }

        /// <summary>
        /// 获取数组数据
        /// </summary>
        /// <returns></returns>
        public TStruct[] GetArray<TStruct>() where TStruct : struct
        {
            TStruct[] array = new TStruct[length];
            for (ulong u = 0; u < length; u++)
                array[u] = (TStruct)Marshal.PtrToStructure(ptr, typeof(TStruct));

            return array;
        }

        public override string ToString()
        {
            return $"{nameof(StructArray)},  Length:{length}  IntPtr:{ptr}";
        }
    }

    /// <summary>
    /// IE Core Version
    /// </summary>
    public struct CoreVersion
    {
        /// <summary>
        /// 大版本号
        /// </summary>
        public ulong Major;

        /// <summary>
        /// 次版本号
        /// </summary>
        public ulong Minor;

        /// <summary>
        /// 设备名称
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string DeviceName;

        /// <summary>
        /// Build Number
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string BuildNumber;

        /// <summary>
        /// 版本描述
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string Description;

        public override string ToString()
        {
            return $"Major:{Major} Minor:{Minor}, " +
                $" DeviceName:{DeviceName}" +
                $" BuildNumber:{BuildNumber}" +
                $" Description:{Description}";
        }
    }

    /// <summary>
    /// 表示描述设备的配置信息
    /// </summary>
    public class CoreConfig
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;

        [MarshalAs(UnmanagedType.LPStr)]
        public string Value;

        /// <summary>
        /// 指向下一个配置
        /// </summary>
        public CoreConfig Next;

        public CoreConfig(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }

        public void SetNext(CoreConfig Config)
        {
            this.Next = Config;
        }

        public void SetNext(string Name, string Value)
        {
            Next = new CoreConfig(Name, Value);
        }

        public override string ToString()
        {
            return $"{Name}:{Value}";
        }
    }

    /// <summary>
    /// 指标和配置参数
    /// </summary>
    public class Parameter
    {
        /*
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.LPStr)]
        public string param;

        [FieldOffset(0)]
        public uint number;

        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.U4, SizeConst = 3)]
        public uint[] range_for_async_infer_request;

        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.U4, SizeConst = 2)]
        public uint[] range_for_streams;
        */

        public IntPtr Params;

        public bool TryGetParam(ref string str)
        {
            try
            {
                str = Marshal.PtrToStringAnsi(Params);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryGetNumber(ref int number)
        {
            try
            {
                number = Params.ToInt32();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryGetRangeRorAsyncInferRequest(ref int[] value)
        {
            try
            {
                Marshal.Copy(Params, value, 0, 3);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryGetRangeForStreams(ref int[] value)
        {
            try
            {
                Marshal.Copy(Params, value, 0, 2);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string ToString()
        {
            string str = "";
            if (TryGetParam(ref str))
                return str;

            return base.ToString();
        }

    }

    /// <summary>
    /// Dimensions
    /// </summary>
    public struct Dimensions
    {
        /// <summary>
        /// <see cref="Dims"/> 有效数据长度
        /// </summary>
        public ulong Ranks;

        /// <summary>
        /// 为一个 8 个值的数组
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public ulong[] Dims;

        /// <summary>
        /// Dimensions
        /// </summary>
        /// <param name="Ranks"></param>
        public Dimensions(ulong Ranks)
        {
            if (Ranks > 8) throw new ArgumentException("参数超出范围 8.", nameof(Ranks));

            this.Ranks = Ranks;
            Dims = new ulong[8];
        }

        /// <summary>
        /// Dimensions
        /// </summary>
        /// <param name="dims"></param>
        public Dimensions(params ulong[] dims)
        {
            this.Ranks = 0;
            Dims = new ulong[8];

            SetValues(dims);
        }

        /// <summary>
        /// 设置 dims 值
        /// </summary>
        /// <param name="dims"></param>
        public void SetValues(params ulong[] dims)
        {
            if (dims.Length > 8) throw new ArgumentException("参数超出范围 8.", nameof(Ranks));

            this.Ranks = (ulong)dims.Length;
            for (int i = 0; i < dims.Length; i++) 
                Dims[i] = dims[i];
        }

        public override string ToString()
        {
            string str = "[";
            for (ulong i = 0; i < Ranks; i++)
                str += Dims[i] + (i != Ranks - 1 ? "x" : "");
            str += "]";
            
            return str;
        }
    }

    /// <summary>
    /// 输入形状
    /// </summary>
    public struct InputShape
    {
        /// <summary>
        /// 形状名称
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;

        /// <summary>
        /// 形状尺寸
        /// </summary>
        public Dimensions Shape;

        public override string ToString()
        {
            return $"name:{Name}, shape:{Shape}";
        }
    }

    /// <summary>
    /// 张量描述
    /// </summary>
    public struct TensorDesc
    {
        /// <summary>
        /// 布局
        /// </summary>
        public Layout Layout;
        /// <summary>
        /// 尺寸
        /// </summary>
        public Dimensions Dims;
        /// <summary>
        /// 精度
        /// </summary>
        public Precision Precision;
    }

    /// <summary>
    /// ROI
    /// </summary>
    public struct ROI
    {
        public ulong id;     // ID of a roi
        public ulong posX;   // W upper left coordinate of roi
        public ulong posY;   // H upper left coordinate of roi
        public ulong sizeX;  // W size of roi
        public ulong sizeY;  // H size of roi
    }



    public delegate void CallBackActionFunc(InferRequest request);
    public delegate void CallBackAction(IntPtr ptr);
    internal struct CompleteCallback
    {
        public IntPtr Func;
        public IntPtr Args;

        public CompleteCallback(IntPtr callback, IntPtr args)
        {
            Args = args;
            Func = callback;
        }

        public CompleteCallback(CallBackAction callback, IntPtr args)
        {
            Args = args;
            Func = Marshal.GetFunctionPointerForDelegate(callback);
        }
    }

    #endregion


}
