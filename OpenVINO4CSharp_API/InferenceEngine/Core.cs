using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static InferenceEngine.IE_C_API;

namespace InferenceEngine
{
    /// <summary>
    /// InferenceEngine Core
    /// </summary>
    public class Core : IntPtrObject
    {
        /// <summary>
        /// 使用带有插件描述的 XML 配置文件构造 Inference Engine Core 实例。  
        /// <para>See RegisterPlugins for more details.</para>
        /// </summary>
        /// <param name="xmlConfigFile">带有要加载的插件的 .xml 文件的路径。如果未指定 XML 配置文件，则将从默认的 plugin.xml 文件加载默认的 InferenceEngine 插件。</param>
        public Core(string xmlConfigFile = "")
        {
            this.freeFunc = IE_C_API.ie_core_free;
            status = IE_C_API.ie_core_create(xmlConfigFile, out ptr);
            if (status != IEStatusCode.OK) throw new Exception("创建 " + typeof(Core).FullName + " 异常");
        }

        /// <summary>
        /// 读取网络模型文件(中间表示文件)
        /// </summary>
        /// <param name="modelPath">path to IR file</param>
        /// <param name="binPath">bin文件的路径，如果path为空，将尝试读取与xml相同名称的bin文件，如果未找到具有相同名称的bin文件，将不带权重地加载IR</param>
        /// <returns></returns>
        public CNNNetwork ReadNetwork(string modelPath, string binPath = "")
        {
            IntPtr network;
            status = IE_C_API.ie_core_read_network(ptr, modelPath, binPath, out network);
            if (status != IEStatusCode.OK) throw new Exception("创建 " + typeof(CNNNetwork).FullName + " 异常");

            return new CNNNetwork(network, IE_C_API.ie_network_free);
        }

        /// <summary>
        /// 从网络对象创建可执行网络。
        /// <para>用户可以创建所需数量的网络并同时使用它们（取决于硬件资源的限制）</para>
        /// </summary>
        /// <param name="network"></param>
        /// <param name="deviceName"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public ExecutableNetwork LoadNetwork(CNNNetwork network, string deviceName, CoreConfig config)
        {
            IntPtr exec_network;
            status = IE_C_API.ie_core_load_network(ptr, network.ptr, deviceName, config, out exec_network);
            if (status != IEStatusCode.OK) throw new Exception("创建 " + typeof(ExecutableNetwork).FullName + " 异常");

            return new ExecutableNetwork(exec_network, IE_C_API.ie_exec_network_free);
        }

        /// <summary>
        /// 注册扩展.
        /// </summary>
        /// <param name="extensionPath"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public bool AddExtension(string extensionPath, string deviceName)
        {
            return IE_C_API.ie_core_add_extension(ptr, extensionPath, deviceName) == IEStatusCode.OK;
        }

        /// <summary>
        /// 在 Inference Engine 中注册实现该设备的新设备和插件。
        /// </summary>
        /// <param name="pluginName"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public bool RegisterPlugin(string pluginName, string deviceName)
        {
            return IE_C_API.ie_core_register_plugin(ptr, pluginName, deviceName) == IEStatusCode.OK;
        }
        /// <summary>
        /// 使用带有插件描述的 XML 配置文件将插件注册到 Inference Engine Core 实例。
        /// </summary>
        /// <param name="xmlConfigFile"></param>
        /// <returns></returns>
        public bool RegisterPlugins(string xmlConfigFile)
        {
            return IE_C_API.ie_core_register_plugins(ptr, xmlConfigFile) == IEStatusCode.OK;
        }
        /// <summary>
        /// 从 Inference Engine 卸载具有指定名称的先前加载的插件该方法是删除插件实例并释放其资源所必需的。
        /// <para>如果以前没有为指定设备创建插件，则该方法将引发异常。</para>
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public bool UnregisterPlugin(string deviceName)
        {
            return IE_C_API.ie_core_unregister_plugin(ptr, deviceName) == IEStatusCode.OK;
        }

        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="config"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public bool SetConfig(CoreConfig config, string deviceName)
        {
            status = IE_C_API.ie_core_set_config(ptr, config, deviceName);
            return status == IEStatusCode.OK;
        }

        /// <summary>
        /// 获取计算设备行为的配置。
        /// 该方法旨在提取可通过 <see cref="SetConfig"/> 方法设置的信息。
        /// </summary>
        /// <param name="deviceName">要获取配置值的设备的名称.</param>
        /// <param name="configName">与配置键对应的配置值。</param>
        /// <returns></returns>
        public Parameter GetConfig(string deviceName, string configName)
        {
            Parameter param;
            status = IE_C_API.ie_core_get_config(ptr, deviceName, configName, out param);
            if (status != IEStatusCode.OK) throw new Exception("获取 IE Config 异常");

            return param;
        }
        /// <summary>
        /// 获取专用硬件的常规运行时指标。
        /// </summary>
        /// <param name="deviceName">A name of a device to get a metric value.</param>
        /// <param name="metricName">metric name to request</param>
        /// <returns></returns>
        public Parameter GetMetric(string deviceName, string metricName)
        {
            Parameter param;
            status = IE_C_API.ie_core_get_metric(ptr, deviceName, metricName, out param);
            if (status != IEStatusCode.OK) throw new Exception("获取 IE Metric 异常");

            return param;
        }

        /// <summary>
        /// Returns plugins version information.
        /// </summary>
        /// <param name="deviceName">Device name to indentify plugin</param>
        /// <returns></returns>
        public CoreVersion[] GetVersions(string deviceName)
        {
            StructArray ie_vers;
            status = IE_C_API.ie_core_get_versions(ptr, deviceName, out ie_vers);
            //if (status != IEStatusCode.OK) throw new Exception("获取 IE Versions 异常");

            //CoreVersion[] vers = new CoreVersion[ie_vers.num_vers];

            //for (ulong i = 0; i < ie_vers.num_vers; i++)
            //    vers[i] = (CoreVersion)Marshal.PtrToStructure(ie_vers.versions + ((int)i * Marshal.SizeOf(typeof(CoreVersion))), typeof(CoreVersion));

            CoreVersion[] vs = ie_vers.GetArray<CoreVersion>();
            IE_C_API.ie_core_versions_free(ref ie_vers);


            return vs;
        }

        /// <summary>
        /// 获取可用计算设备
        /// </summary>
        /// <returns></returns>
        public string[] GetAvailableDevices()
        {
            ie_available_devices avai_devices;
            status = IE_C_API.ie_core_get_available_devices(ptr, out avai_devices);
            if (status != IEStatusCode.OK) throw new Exception("获取 IE 可用设备异常");

            Console.WriteLine(avai_devices.num_devices);

            return null;
        }


        /// <summary>
        /// get ie api version
        /// </summary>
        /// <returns></returns>
        public static string GetAPIVersion()
        {
            return IE_C_API.ie_c_api_version();
        }


    }
}
