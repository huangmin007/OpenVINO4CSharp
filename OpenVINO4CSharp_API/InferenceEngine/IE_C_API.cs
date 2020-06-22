using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace InferenceEngine
{
    #region Enumerations
    /// <summary>
    /// 推理引擎支持的布局
    /// </summary>
    public enum Layout
    {
        /// <summary> "any" layout </summary>
        ANY = 0,
        /// <summary> I/O data layouts </summary>
        NCHW = 1,
        /// <summary> I/O data layouts </summary>
        NHWC = 2,
        /// <summary> I/O data layouts </summary>
        NCDHW = 3,
        /// <summary> I/O data layouts </summary>
        NDHWC = 4,
        /// <summary> weight layouts </summary>
        OIHW = 64,
        /// <summary> Scalar </summary>
        SCALAR = 95,
        /// <summary> bias layouts </summary>
        C = 96,
        /// <summary> Single image layout (for mean image) </summary>
        CHW = 128,
        /// <summary> 2D </summary>
        HW = 192,
        /// <summary> 2D </summary>
        NC = 193,
        /// <summary> 2D </summary>
        CN = 194,
        /// <summary> blocked </summary>
        BLOCKED = 200,
    }

    /// <summary>
    /// 推理引擎支持的精度
    /// </summary>
    public enum Precision
    {
        /// <summary>
        /// Unspecified value. Used by default
        /// </summary>
        UNSPECIFIED = 255,
        /// <summary>
        /// Mixed value. Can be received from network. No applicable for tensors
        /// </summary>
        MIXED = 0,
        /// <summary>
        /// 32bit floating point value
        /// </summary>
        FP32 = 10,
        /// <summary>
        /// 16bit floating point value
        /// </summary>
        FP16 = 11,
        /// <summary>
        /// 16bit specific signed fixed point precision
        /// </summary>
        Q78 = 20,
        /// <summary>
        /// 16bit signed integer value
        /// </summary>
        I16 = 30,
        /// <summary>
        /// 8bit unsigned integer value
        /// </summary>
        U8 = 40,
        /// <summary>
        /// 8bit signed integer value
        /// </summary>
        I8 = 50,
        /// <summary>
        /// 16bit unsigned integer value
        /// </summary>
        U16 = 60,
        /// <summary>
        /// 32bit signed integer value
        /// </summary>
        I32 = 70,
        /// <summary>
        /// 64bit signed integer value
        /// </summary>
        I64 = 72,
        /// <summary>
        /// 64bit unsigned integer value
        /// </summary>
        U64 = 73,
        /// <summary>
        /// 1bit integer value
        /// </summary>
        BIN = 71,
        /// <summary>
        /// custom precision has it's own name and size of elements
        /// </summary>
        CUSTOM = 80
    }

    /// <summary>
    /// 有关用于预处理的输入颜色格式的其他信息
    /// </summary>
    public enum ColorFormat : uint
    {
        /// <summary>
        /// Plain blob (default), no extra color processing required
        /// </summary>
        RAW = 0,
        /// <summary>
        /// RGB color format
        /// </summary>
        RGB,
        /// <summary>
        /// BGR color format, default in DLDT
        /// </summary>
        BGR,
        /// <summary>
        /// RGBX color format with X ignored during inference
        /// </summary>
        RGBX,
        /// <summary>
        /// BGRX color format with X ignored during inference
        /// </summary>
        BGRX,
        /// <summary>
        /// NV12 color format represented as compound Y+UV blob
        /// </summary>
        NV12,
        /// <summary>
        /// I420 color format represented as compound Y+U+V blob
        /// </summary>
        I420,
    }

    /// <summary>
    /// 表示支持的调整大小算法的列表。
    /// </summary>
    public enum ResizeAlgorithm
    {
        NO_RESIZE = 0,
        RESIZE_BILINEAR,
        RESIZE_AREA,
    }

    /// <summary>
    /// 该枚举包含接口函数所有可能返回值的代码
    /// </summary>
    public enum IEStatusCode : int
    {
        OK = 0,
        GENERAL_ERROR = -1,
        NOT_IMPLEMENTED = -2,
        NETWORK_NOT_LOADED = -3,
        PARAMETER_MISMATCH = -4,
        NOT_FOUND = -5,
        OUT_OF_BOUNDS = -6,
        /// <summary>
        /// exception not of std::exception derived type was thrown
        /// </summary>
        UNEXPECTED = -7,
        REQUEST_BUSY = -8,
        RESULT_NOT_READY = -9,
        NOT_ALLOCATED = -10,
        INFER_NOT_STARTED = -11,
        NETWORK_NOT_READ = -12
    }
    #endregion

    #region Structures

    public struct ie_available_devices
    {
        public IntPtr devices;
        public ulong num_devices;
    }

    /// <summary>
    /// 表示配置参数信息
    /// </summary>
    public struct ie_param_config
    {
        public string name;
        public Parameter param;
    }
    
    public delegate void CompleteCallBackFunc(IntPtr args);

    public struct ie_complete_call_back
    {
        public IntPtr CallbackFunc;
        public IntPtr Args;
    }
    #endregion

    

    /// <summary>
    /// InferenceEngine For C API
    /// </summary>
    internal class IE_C_API
    {
        /// <summary>
        /// dll name
        /// <para>路径：D:\Program Files (x86)\IntelSWTools\openvino\inference_engine\bin\intel64\Release</para>
        /// </summary>
//#if DEBUG
        //public const string DLL_NAME = "inference_engine_c_apid.dll";
//#else
        public const string DLL_NAME = "inference_engine_c_api.dll";
//#endif

        /// <summary>
        /// 返回 IE API 的版本号，使用 <see cref="ie_version_free"/> 释放内存。
        /// </summary>
        /// <returns>API 的版本号</returns>
        [DllImport(DLL_NAME, EntryPoint = "ie_c_api_version", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string ie_c_api_version();

        /// <summary>
        /// 释放由 <see cref="ie_c_api_version"/> 分配的内存
        /// <para>该函数还可以清除其它字符变量</para>
        /// </summary>
        /// <param name="version">版本指向 ie版本t 到可用内存的指针</param>
        [DllImport(DLL_NAME, EntryPoint = "ie_version_free", CharSet = CharSet.Ansi)]
        public extern static void ie_version_free(ref string version);

        /// <summary>
        /// 释放分配的内存.
        /// </summary>
        /// <param name="param"></param>
        [DllImport(DLL_NAME, EntryPoint = "ie_param_free", CharSet = CharSet.Ansi)]
        public extern static void ie_param_free(ref Parameter param);



        #region InferenceEngine::Core
        /// <summary>
        /// 使用带有设备描述的XML配置文件构造 Inference Engine Core 实例。
        /// Use the ie_core_free() method to free memory.
        /// </summary>
        /// <param name="xml_config_file"></param>
        /// <param name="core"></param>
        /// <returns></returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Ansi)]
        public extern static IEStatusCode ie_core_create(string xml_config_file, out IntPtr core);

        [DllImport(DLL_NAME)]
        public extern static void ie_core_free(ref IntPtr core);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_core_get_versions(IntPtr core, string device_name, out StructArray versions);
        [DllImport(DLL_NAME)]
        public extern static void ie_core_versions_free(ref StructArray vers);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_core_read_network(IntPtr core, string xml, string weights_file, out IntPtr network);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_core_load_network(IntPtr core, IntPtr network, string device_name, CoreConfig config, out IntPtr exe_network);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_core_set_config(IntPtr core, CoreConfig ie_core_config, string device_name);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_core_register_plugin(IntPtr core, string plugin_name, string device_name);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_core_register_plugins(IntPtr core, string xml_config_file);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_core_unregister_plugin(IntPtr core, string device_name);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_core_add_extension(IntPtr core, string extension_path, string device_name);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_core_get_metric(IntPtr core, string device_name, string metric_name, out Parameter param_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_core_get_config(IntPtr core, string device_name, string config_name, out Parameter param_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_core_get_available_devices(IntPtr core, out ie_available_devices avai_devices);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static void ie_core_available_devices_free(ie_available_devices avai_devices);
        #endregion


        #region InferenceEngine::ExecutableNetwork
        [DllImport(DLL_NAME, SetLastError = true)]
        public extern static void ie_exec_network_free(ref IntPtr ie_exec_network);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_exec_network_create_infer_request(IntPtr ie_exec_network, out IntPtr request);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_exec_network_get_metric(IntPtr ie_exec_network, string metric_name, out Parameter param_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_exec_network_set_config(IntPtr ie_exec_network, CoreConfig param_config);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_exec_network_get_config(IntPtr ie_exec_network, string metric_config, [Out] Parameter param_result);
        #endregion


        #region InferenceEngine::InferRequest
        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static void ie_infer_request_free(ref IntPtr infer_Request);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_infer_request_get_blob(IntPtr infer_request, string name, out IntPtr blob);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_infer_request_set_blob(IntPtr infer_request, string name, [In] IntPtr blob);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_infer_request_infer(IntPtr infer_request);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_infer_request_infer_async(IntPtr infer_request);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_infer_set_completion_callback(IntPtr infer_request, ie_complete_call_back callback);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_infer_request_wait(IntPtr infer_request, ulong timeout);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_infer_request_set_batch(IntPtr infer_request, ulong size);
        #endregion


        #region InferenceEngine::Network
        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static void ie_network_free(ref IntPtr network);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_get_name(IntPtr network, out string name);


        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_get_inputs_number(IntPtr network, out ulong size_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_get_input_name(IntPtr network, ulong number, out string name);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_get_input_precision(IntPtr network, string input_name, out Precision prec_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_set_input_precision(IntPtr network, string input_name, Precision prec_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_get_input_layout(IntPtr network, string input_name, out Layout layout_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_set_input_layout(IntPtr network, string input_name, Layout lay);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_get_input_resize_algorithm(IntPtr network, string input_name, out ResizeAlgorithm resize_alg_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_set_input_resize_algorithm(IntPtr network, string input_name, ResizeAlgorithm resize_algo);


        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_get_color_format(IntPtr network, string input_name, out ColorFormat colorformat_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_set_color_format(IntPtr network, string input_name, ColorFormat color_format);


        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_get_input_shapes(IntPtr network, out StructArray shapes);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static void ie_network_input_shapes_free(StructArray inputShapes);


        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_reshape(IntPtr network, StructArray shapes);


        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_get_outputs_number(IntPtr network, out ulong size_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_get_output_name(IntPtr network, ulong number, out string name);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_get_output_precision(IntPtr network, string output_name, out Precision prec_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_set_output_precision(IntPtr network, string output_name, Precision prec);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_get_output_layout(IntPtr network, string output_name, out Layout layout_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_set_output_layout(IntPtr network, string output_name, Layout lay);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_network_get_output_dims(IntPtr network, string output_name, out Dimensions dims_result);


        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static void ie_network_name_free(ref string name);
        #endregion


        #region InferenceEngine::Blob
        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_blob_make_memory(ref TensorDesc tensorDesc, out IntPtr blob);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_blob_make_memory_from_preallocated(ref TensorDesc tensorDesc, IntPtr ptr, ulong size, out IntPtr blob);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_blob_make_memory_from_preallocated(ref TensorDesc tensorDesc, byte[] ptr, ulong size, out IntPtr blob);


        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_blob_make_memory_with_roi(IntPtr inputBlob, ref ROI roi, out IntPtr blob);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_blob_make_memory_nv12(IntPtr y, IntPtr uv, out IntPtr nv12Blob);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_blob_make_memory_i420(IntPtr y, IntPtr u, IntPtr v, out IntPtr i420Blob);


        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_blob_size(IntPtr blob, out int size_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_blob_byte_size(IntPtr blob, out int bsize_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static void ie_blob_deallocate(ref IntPtr blob);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_blob_get_buffer(IntPtr blob, out IntPtr blob_buffer);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_blob_get_cbuffer(IntPtr blob, out IntPtr blob_cbuffer);


        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_blob_get_dims(IntPtr blob, out Dimensions dims_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_blob_get_layout(IntPtr blob, out Layout layout_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static IEStatusCode ie_blob_get_precision(IntPtr blob, out Precision prec_result);

        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, SetLastError = true)]
        public extern static void ie_blob_free(ref IntPtr blob);

        #endregion




        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint FormatMessage(int dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageZId, StringBuilder lpBuffer, uint nSize, IntPtr Arguments);

        public static string GetSysErrroMessage(uint errorCode)
        {
            StringBuilder message = new StringBuilder(255);
            int flags = 0x00000200 | 0x00001000;
            FormatMessage(flags, IntPtr.Zero, errorCode, 0, message, 255, IntPtr.Zero);

            return message.ToString().Trim();
        }
    }
}
