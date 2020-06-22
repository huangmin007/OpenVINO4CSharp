﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InferenceEngine;

namespace WPF_IE_API
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public struct Result7
        {
            public float image_id;
            public float class_id;
            public float conf;
            public float x_min;
            public float y_min;
            public float x_max;
            public float y_max;

            public override string ToString()
            {
                return $"image_id:{image_id} class_id:{class_id} conf:{conf} [{x_min},{y_min},{x_max},{y_max}]";
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        Core core;
        CNNNetwork network;
        string xml = @"E:\2020\008_AI\OpenVINO&OpenCV Modules\OpenVINO_CV\x64\Release\models\face-detection-retail-0044\FP32\face-detection-retail-0044.xml";
        //string xml = @"E:\2020\008_AI\OpenVINO&OpenCV Modules\OpenVINO_CV\x64\Release\models\face-detection-0105\FP32\face-detection-0105.xml";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            core = new Core();
            CoreVersion[] vers = core.GetVersions("CPU");
            foreach(CoreVersion ver in vers) Console.WriteLine(ver);
            //core.GetAvailableDevices();

            CoreConfig config = new CoreConfig("DYN_BATCH_ENABLED", "YES");
            //config.SetNext("CPU_THREADS_NUM", "NUMA");

            network = core.ReadNetwork(xml);
            Console.WriteLine("{0}", network.Name);

            InputShape[] shapes = network.GetInputShapes();
            foreach(InputShape shape in shapes) Console.WriteLine(shape);
            //bool result = network.SetReshape(shapes);
            //Console.WriteLine(result);

            InputInfo[] inputs = network.GetInputsInfo();
            foreach(InputInfo input in inputs) Console.WriteLine(input);
            inputs[0].Precision = Precision.U8;
            inputs[0].Layout = Layout.NHWC;
            inputs[0].ResizeAlgorithm = ResizeAlgorithm.RESIZE_BILINEAR;

            OutputInfo[] outputs = network.GetOutputsInfo();
            foreach(OutputInfo output in outputs)Console.WriteLine(output);

            ExecutableNetwork exec_network = core.LoadNetwork(network, "CPU", config);
            Console.WriteLine(exec_network.GetConfig("DYN_BATCH_ENABLED"));

            InferRequest request = exec_network.CreateInferRequest();


            Bitmap bmp = new Bitmap(@"E:\2020\008_AI\OpenVINO4CSharp_API\5.jpg");
            Console.WriteLine("Bitmap:{0} {1}", bmp.Width, bmp.Height);

            BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            
            TensorDesc tensorDesc = new TensorDesc()
            {
                Layout = Layout.NHWC,
                Dims = new Dimensions(1, 3, 499, 800),
                Precision = Precision.U8
            };
            Blob blob = Blob.MakeMemoryFromPreallocated(ref tensorDesc, bmpData.Scan0, 800 * 499 * 3);
            Console.WriteLine("{0}  {1}", bmpData.Scan0, blob.Buffer);
            request.SetBlob(inputs[0].Name, blob);
            request.SetCompletionCallback();
            request.StartAsync();
            //request.Infer();
#if false
            Blob outputBlob = request.GetBlob(outputs[0].Name);
            IntPtr buffer = outputBlob.Buffer;

            List<Result7> results = new List<Result7>(128);
            for (int i = 0; i < 200; i++)
            {
                Result7 result = (Result7)Marshal.PtrToStructure(buffer + (i * 7 * 4), typeof(Result7));

                if (result.image_id < 0) break;
                if (result.conf >= 0.5)
                {
                    results.Add(result);
                    Console.WriteLine(">{0} {1}", i, result);
                }
            }

            bmp.UnlockBits(bmpData);

            Graphics g = Graphics.FromImage(bmp);
            for (int i = 0; i < results.Count; i++)
            {
                int x = (int)(results[i].x_min * bmp.Width);
                int y = (int)(results[i].y_min * bmp.Height);
                int w = (int)(results[i].x_max * bmp.Width) - x;
                int h = (int)(results[i].y_max * bmp.Height) - y;
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(x, y, w, h);

                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 2), rect);
            }

            bmp.Save("test.jpg");
            Image.Source = ChangeBitmapToImageSource(bmp);
#endif
            //Console.WriteLine(core.SetConfig(config, "CPU"));
            //Parameter param = core.GetConfig("CPU", "DYN_BATCH_ENABLED");
            //Console.WriteLine(param);
        }

#if false
        private void Window_Loaded_t(object sender, RoutedEventArgs e)
        {
            IEStatusCode status;
            string xml = @"E:\2020\008_AI\OpenVINO&OpenCV Modules\OpenVINO_CV\x64\Release\models\face-detection-retail-0044\FP32\face-detection-retail-0044.xml";
            //string xml = @"E:\2020\008_AI\OpenVINO&OpenCV Modules\OpenVINO_CV\x64\Release\models\face-detection-0105\FP32\face-detection-0105.xml";

            ie_core core;
            ie_network network;

            // --------------------------- 1. Load inference engine instance -------------------------------------
            status = ie_core_create("", out core);
            Console.WriteLine("Core:{0} {1}", status, core);
            if (status != IEStatusCode.OK)
            {
                ie_core_free(ref core);
                return;
            }

            // --------------------------- 2. Read IR Generated by ModelOptimizer (.xml and .bin files) ------------
            status = ie_core_read_network(core, xml, null, out network);
            Console.WriteLine("Read Network:{0} {1}", status, network);
            if(status != IEStatusCode.OK) 
            {
                ie_core_free(ref core);
                return;
            }

            // --------------------------- 3. Configure input & output ---------------------------------------------
            // --------------------------- Prepare input blobs -----------------------------------------------------
            ulong input_size;
            ie_network_get_inputs_number(network, out input_size);
            Console.WriteLine("Input Size:{0}", input_size);
            string input_name;
            status = ie_network_get_input_name(network, 0, out input_name);
            Console.WriteLine("Input Name:{0}  {1}", status, input_name);

            status |= ie_network_set_input_resize_algorithm(network, input_name, resize_alg.RESIZE_BILINEAR);
            status |= ie_network_set_input_layout(network, input_name, layout.NHWC);
            status |= ie_network_set_input_precision(network, input_name, precision.U8);
            if(status != IEStatusCode.OK)
            {
                ie_core_free(ref core);
                return;
            }
            // --------------------------- Prepare output blobs ----------------------------------------------------
            ulong output_size;
            ie_network_get_outputs_number(network, out output_size);
            Console.WriteLine("Output Size:{0}", output_size);
            string[] output_name = new string[output_size];
            for (ulong i = 0; i < output_size; i++)
            {
                status |= ie_network_get_output_name(network, i, out output_name[i]);
                Console.WriteLine("Output Name:{0} {1}", status, output_name[i]);
            }
            status |= ie_network_set_output_precision(network, output_name[0], precision.FP32);
            if (status != IEStatusCode.OK)
            {
                ie_core_free(ref core);
                return;
            }

            // --------------------------- 4. Loading model to the device ------------------------------------------
            ie_config config = new ie_config();
            ie_executable_network exe_network;
            status = ie_core_load_network(core, network, "CPU", config, out exe_network);
            Console.WriteLine("Exe Network:{0} {1}", status, exe_network);
            if (status != IEStatusCode.OK)
            {
                ie_core_free(ref core);
                return;
            }

            // --------------------------- 5. Create infer request -------------------------------------------------
            ie_infer_request infer_request;
            status = ie_exec_network_create_infer_request(exe_network, out infer_request);
            Console.WriteLine("Create Infer:{0} {1}", status, infer_request);

            // --------------------------- 6. Prepare input --------------------------------------------------------
            /* Read input image to a blob and set it to an infer request without resize and layout conversions. */
            //byte[] data = SaveImage(@"E:\2020\008_AI\IE_C_API_2_CSharp\5.jpg");
            Bitmap bmp = new Bitmap(@"E:\2020\008_AI\OpenVINO4CSharp_API\5.jpg");
            Console.WriteLine("Bitmap:{0} {1}", bmp.Width, bmp.Height);

            BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), 
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            ulong[] dims = { 1ul, 3ul, 499ul, 800ul };
            dimensions dimens = new dimensions(4);
            //dimens.ranks = 4;
            //dimens.dims = new ulong[8];
            dimens.dims[0] = 1;
            dimens.dims[1] = 3;
            dimens.dims[2] = 499;
            dimens.dims[3] = 800;
            tensor_desc tensorDesc = new tensor_desc()
            {
                layout = layout.NHWC, 
                dims = dimens, 
                precision = precision.U8 
            };
            ie_blob blob;
            status = ie_blob_make_memory_from_preallocated(ref tensorDesc, bmpData.Scan0, 800 * 499 * 3, out blob);
            Console.WriteLine("Make Blob:{0} {1} {2}", status, Marshal.SizeOf(dimens), Marshal.SizeOf(typeof(tensor_desc)));


            status = ie_infer_request_set_blob(infer_request, input_name, blob);
            Console.WriteLine("Set Blob:{0}", status);

            // --------------------------- 7. Do inference --------------------------------------------------------
            /* Running the request synchronously */
            status = ie_infer_request_infer(infer_request);
            Console.WriteLine("Infer:{0}", status);

            // --------------------------- 8. Process output ------------------------------------------------------
            ie_blob output_blob;
            status = ie_infer_request_get_blob(infer_request, output_name[0], out output_blob);
            Console.WriteLine("Result:{0}", status);
            dimensions output_dims;
            status = ie_blob_get_dims(output_blob, out output_dims);
            Console.WriteLine("Output Dims:{0} {1}", status, output_dims);

            ie_blob_buffer buffer;
            status = ie_blob_get_cbuffer(output_blob, out buffer);
            Console.WriteLine("Buffer:{0} {1}", status, buffer);

            List<Result7> results = new List<Result7>(128);
            for (int i = 0; i < 200; i++)
            {
                Result7 result = (Result7)Marshal.PtrToStructure(buffer.buffer + (i * 7 * 4), typeof(Result7));

                if (result.image_id < 0) break;
                if (result.conf >= 0.5)
                {
                    results.Add(result);
                    Console.WriteLine(">{0} {1}", i, result);
                }
            }

            bmp.UnlockBits(bmpData);

            Graphics g = Graphics.FromImage(bmp);
            for (int i = 0; i < results.Count; i ++)
            {
                int x = (int)(results[i].x_min * bmp.Width);
                int y = (int)(results[i].y_min * bmp.Height);
                int w = (int)(results[i].x_max * bmp.Width) - x;
                int h = (int)(results[i].y_max * bmp.Height) - y;
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(x, y, w, h);
                
                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 2), rect);
            }

            bmp.Save("test.jpg");
            Image.Source = ChangeBitmapToImageSource(bmp);

            //float[] data = new float[200*7];
            //Marshal.Copy(buffer.buffer, data, 0, 200 * 7);

            


            Console.WriteLine("Dispose..");
            ie_network_free(ref network);
            Console.WriteLine("Network:{0}", network);

            ie_core_free(ref core);
            Console.WriteLine("Core:{0}", core);
        }
#endif

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        public static ImageSource ChangeBitmapToImageSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new System.ComponentModel.Win32Exception();
            }
            //GC.Collect();

            return wpfBitmap;
        }

    }
}
