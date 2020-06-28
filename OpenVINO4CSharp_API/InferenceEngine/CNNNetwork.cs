using System;
using static InferenceEngine.IE_C_API;

namespace InferenceEngine
{
    /// <summary>
    /// Inference Engine CNN Network Object
    /// </summary>
    public sealed class CNNNetwork : IntPtrObject
    {
        private string _Name;

        /// <summary>
        /// CNNNetwork
        /// </summary>
        //public CNNNetwork(){}

        /// <inheritdoc/>
        internal CNNNetwork(IntPtr ptr, FreeIntPtrDelegate freeFunc) : base(ptr, freeFunc)
        {
        }

        /// <inheritdoc />
        protected override void FreeDll()
        {
            base.FreeDll();
            ie_network_name_free(ref _Name);

            _Name = null;
        }
       
        /// <summary>
        /// 获取网络名称
        /// </summary>
        /// <returns></returns>        
        public string Name
        {
            get 
            {
                IE_C_API.ie_network_get_name(ptr, out _Name);
                return _Name;
            }
        }

        /// <summary>
        /// 获取网络输入层数据信息
        /// </summary>
        /// <returns></returns>
        public InputInfo[] GetInputsInfo()
        {
            ulong size;
            ie_network_get_inputs_number(ptr, out size);            
            InputInfo[] inputs = new InputInfo[size];

            InputShape[] shapes = GetInputShapes();
            for (ulong u = 0; u < size; u++)
                inputs[u] = new InputInfo(ref ptr, u, ref shapes[u]);

            return inputs;
        }

        /// <summary>
        /// 获取网络输出层数据信息
        /// </summary>
        /// <returns></returns>
        public OutputInfo[] GetOutputsInfo()
        {
            ulong size;
            ie_network_get_outputs_number(ptr, out size);
            OutputInfo[] outputs = new OutputInfo[size];

            for (ulong u = 0; u < size; u++)
                outputs[u] = new OutputInfo(ref ptr, u);

            return outputs;
        }

        /// <summary>
        /// 获取输入形状
        /// </summary>
        /// <returns></returns>
        public InputShape[] GetInputShapes()
        {
            StructArray shapes;
            IE_C_API.ie_network_get_input_shapes(ptr, out shapes);

            InputShape[] inputShapes = shapes.GetArray<InputShape>();
            IE_C_API.ie_network_input_shapes_free(shapes);

            return inputShapes;
        }

        /// <summary>
        /// 设置输入形状
        /// </summary>
        /// <param name="shapes"></param>
        /// <returns></returns>
        public bool SetReshape(InputShape[] shapes)
        {
            StructArray reshape = new StructArray();
            reshape.SetArray<InputShape>(shapes);

            return IE_C_API.ie_network_reshape(ptr, reshape) == IEStatusCode.OK;
        }
    }


    /// <summary>
    /// CNN Network Input Layer Data Info 
    /// </summary>
    public sealed class InputInfo : IntPtrObject
    {
        private readonly ulong _Index;

        private string _Name;
        private Layout _Layout;
        private Dimensions _Dims;
        private Precision _Precision;

        private InputShape _Shape;
        private ColorFormat _ColorFormat;
        private ResizeAlgorithm _ResizeAlgorithm;

        /// <summary>
        /// 网络输入层索引
        /// </summary>
        public ulong Index => _Index;

        /// <summary>
        /// 网络输入层名称
        /// </summary>
        public string Name => _Name;

        /// <summary>
        /// 网络输入层尺寸
        /// </summary>
        public Dimensions Dims => _Dims;

        /// <summary>
        /// 网络输入层形状
        /// </summary>
        public InputShape Shape => _Shape;


        /// <summary>
        /// 网络输入层布局
        /// </summary>
        public Layout Layout
        {
            get { return _Layout; }
            set
            {
                if (IE_C_API.ie_network_set_input_layout(ptr, _Name, value) == IEStatusCode.OK)
                    IE_C_API.ie_network_get_input_layout(ptr, _Name, out _Layout);
            }
        }

        /// <summary>
        /// 网络输入层精度
        /// </summary>
        public Precision Precision
        {
            get { return _Precision; }
            set
            {
                if (IE_C_API.ie_network_set_input_precision(ptr, _Name, value) == IEStatusCode.OK)
                    IE_C_API.ie_network_get_input_precision(ptr, _Name, out _Precision);
            }
        }

        /// <summary>
        /// 网络输入层颜色格式
        /// </summary>
        public ColorFormat ColorFormat
        {
            get { return _ColorFormat; }
            set
            {
                if (IE_C_API.ie_network_set_color_format(ptr, _Name, value) == IEStatusCode.OK)
                    IE_C_API.ie_network_get_color_format(ptr, _Name, out _ColorFormat);
            }
        }

        /// <summary>
        /// 网络输入层大小调整算法
        /// </summary>
        public ResizeAlgorithm ResizeAlgorithm
        {
            get { return _ResizeAlgorithm; }
            set
            {
                if (IE_C_API.ie_network_set_input_resize_algorithm(ptr, _Name, value) == IEStatusCode.OK)
                    IE_C_API.ie_network_get_input_resize_algorithm(ptr, _Name, out _ResizeAlgorithm);
            }
        }

        /// <summary>
        /// Input Data Info
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="index"></param>
        /// <param name="shape"></param>
        internal InputInfo(ref IntPtr ptr, ulong index, ref InputShape shape) : base(ptr, null)
        {
            _Index = index;
            _Shape = shape;
            _Dims = _Shape.Shape;

            UpdateDataInfo();
        }


        /// <summary>
        /// 更新数据信息
        /// </summary>
        internal void UpdateDataInfo()
        {
            status = IE_C_API.ie_network_get_input_name(ptr, _Index, out _Name);
            status |= IE_C_API.ie_network_get_input_layout(ptr, _Name, out _Layout);
            status |= IE_C_API.ie_network_get_color_format(ptr, _Name, out _ColorFormat);
            status |= IE_C_API.ie_network_get_input_precision(ptr, _Name, out _Precision);
            status |= IE_C_API.ie_network_get_input_resize_algorithm(ptr, _Name, out _ResizeAlgorithm);

            //StructArray shapes;
            //status |= IE_C_API.ie_network_get_input_shapes(ptr, out shapes);
            //_Shape = shapes.GetArray<InputShape>()[_Index];
            //_Dims = _Shape.Shape;
            //IE_C_API.ie_network_input_shapes_free(shapes);
        }

        public override string ToString()
        {
            return $"Index:{_Index}, Name:{_Name}, Layout:{_Layout}, Precision:{_Precision}, Dims:{_Dims}, ColorFormat:{_ColorFormat}, ResizeAlgorithm:{_ResizeAlgorithm}";
        }
    }

    /// <summary>
    /// CNN Network Output Layer Data Info 
    /// </summary>
    public sealed class OutputInfo : IntPtrObject
    {
        private readonly ulong _Index;

        private string _Name;
        private Layout _Layout;
        private Dimensions _Dims;
        private Precision _Precision;

        /// <summary>
        /// 网络输出层索引
        /// </summary>
        public ulong Index => _Index;

        /// <summary>
        /// 网络输出层名称
        /// </summary>
        public string Name => _Name;

        /// <summary>
        /// 网络输出层尺寸
        /// </summary>
        public Dimensions Dims => _Dims;

        /// <summary>
        /// 网络输出层布局
        /// </summary>
        public Layout Layout
        {
            get { return _Layout; }
            set
            {
                if (IE_C_API.ie_network_set_output_layout(ptr, _Name, value) == IEStatusCode.OK)
                    IE_C_API.ie_network_get_output_layout(ptr, _Name, out _Layout);
            }
        }

        /// <summary>
        /// 网络输出层精度
        /// </summary>
        public Precision Precision
        {
            get { return _Precision; }
            set
            {
                if (IE_C_API.ie_network_set_output_precision(ptr, _Name, value) == IEStatusCode.OK)
                    IE_C_API.ie_network_get_output_precision(ptr, _Name, out _Precision);
            }
        }

        /// <summary>
        /// Output Data Info
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="index"></param>
        internal OutputInfo(ref IntPtr ptr, ulong index) : base(ptr, null)
        {
            _Index = index;
            UpdateDataInfo();
        }

        /// <summary>
        /// 更新数据信息
        /// </summary>
        internal void UpdateDataInfo()
        {
            status = IE_C_API.ie_network_get_output_name(ptr, _Index, out _Name);
            status = IE_C_API.ie_network_get_output_dims(ptr, _Name, out _Dims);
            status = IE_C_API.ie_network_get_output_layout(ptr, _Name, out _Layout);
            status = IE_C_API.ie_network_get_output_precision(ptr, _Name, out _Precision);
        }

        public override string ToString()
        {
            return $"Index:{_Index}, Name:{_Name}, Layout:{_Layout}, Precision:{_Precision}, Dims:{_Dims}";
        }
    }

}
