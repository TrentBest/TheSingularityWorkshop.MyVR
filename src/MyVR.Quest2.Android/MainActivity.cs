using System.Runtime.InteropServices;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Widget;
using Java.Interop;
using Silk.NET.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenXR;
using Silk.NET.OpenXR.Extensions.KHR;
using XrResult = Silk.NET.OpenXR.Result;
using XrApplicationInfo = Silk.NET.OpenXR.ApplicationInfo;
using VkApi = Silk.NET.Vulkan.Vk;
using VkApplicationInfo = Silk.NET.Vulkan.ApplicationInfo;
using VkDevice = Silk.NET.Vulkan.Device;
using VkDeviceCreateInfo = Silk.NET.Vulkan.DeviceCreateInfo;
using VkDeviceQueueCreateInfo = Silk.NET.Vulkan.DeviceQueueCreateInfo;
using VkInstance = Silk.NET.Vulkan.Instance;
using VkInstanceCreateInfo = Silk.NET.Vulkan.InstanceCreateInfo;
using VkPhysicalDevice = Silk.NET.Vulkan.PhysicalDevice;
using VkPhysicalDeviceFeatures = Silk.NET.Vulkan.PhysicalDeviceFeatures;
using VkQueue = Silk.NET.Vulkan.Queue;
using VkQueueFamilyProperties = Silk.NET.Vulkan.QueueFamilyProperties;
using VkQueueFlags = Silk.NET.Vulkan.QueueFlags;
using VkResult = Silk.NET.Vulkan.Result;

namespace MyVR.Quest2.Android;

[Activity(
    Label = "MyVR Quest 2 Host",
    MainLauncher = true,
    Exported = true,
    ScreenOrientation = ScreenOrientation.Landscape)]
[IntentFilter(
    new[] { Intent.ActionMain },
    Categories = new[]
    {
        "org.khronos.openxr.intent.category.IMMERSIVE_HMD",
        Intent.CategoryLauncher
    })]
public sealed class MainActivity : Activity
{
    private const string LogTag = "MyVR.Quest2";
    private const string AndroidCreateInstanceExtension = "XR_KHR_android_create_instance";
    private const string VulkanEnable2Extension = "XR_KHR_vulkan_enable2";
    private TextView? _status;
    private XR? _openXr;
    private KhrLoaderInit? _loaderInit;
    private KhrVulkanEnable2? _vulkanEnable2;
    private VkApi? _vulkan;
    private nint _vulkanLibrary;
    private VkInstance _vulkanInstance;
    private VkPhysicalDevice _vulkanPhysicalDevice;
    private VkDevice _vulkanDevice;
    private VkQueue _graphicsQueue;
    private Session _session;
    private uint _queueFamilyIndex;
    private bool _hasVulkanInstance;
    private bool _hasVulkanDevice;
    private bool _hasSession;
    private nint _openXrLibrary;
    private nint _javaVm;
    private Instance _instance;
    private ulong _systemId;
    private ulong _vulkanMinApiVersion;
    private ulong _vulkanMaxApiVersion;
    private bool _hasInstance;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        _status = new TextView(this)
        {
            TextSize = 18,
            Text = "MyVR Quest 2 host bootstrap\nInitializing the OpenXR loader…"
        };
        _status.SetPadding(32, 32, 32, 32);
        SetContentView(_status);

        try
        {
            InitializeOpenXrLoader();
            CreateOpenXrInstanceAndFindHeadset();
            CreateVulkanDeviceAndOpenXrSession();
            SetStatus(
                $"OpenXR session created for HMD system 0x{_systemId:x}.\n" +
                $"Vulkan API range: 0x{_vulkanMinApiVersion:x}–0x{_vulkanMaxApiVersion:x}.\n" +
                $"Graphics queue family: {_queueFamilyIndex}.\n\n" +
                "Session event processing, swapchain, frame submission, " +
                "Workshop moniker rendering, and controller input are not implemented yet.");
            Log.Info(
                LogTag,
                $"OpenXR session created. Vulkan graphics queue family: {_queueFamilyIndex}.");
        }
        catch (Exception exception)
        {
            SetStatus(
                "Android host launched, but OpenXR initialization failed.\n\n" +
                exception);
            Log.Error(LogTag, $"OpenXR initialization failed: {exception}");
            ReleaseOpenXr();
        }
    }

    protected override void OnResume()
    {
        base.OnResume();
        Log.Info(LogTag, "Android activity resumed.");
    }

    protected override void OnPause()
    {
        Log.Info(LogTag, "Android activity paused.");
        base.OnPause();
    }

    protected override void OnDestroy()
    {
        ReleaseOpenXr();
        Log.Info(LogTag, "Android activity destroyed.");
        base.OnDestroy();
    }

    private unsafe void InitializeOpenXrLoader()
    {
        // Use the exact AAR library name. Silk.NET's default Android search name
        // includes a .so.1 suffix, while the Khronos AAR packages libopenxr_loader.so.
        _openXrLibrary = NativeLibrary.Load("libopenxr_loader.so");
        _openXr = new XR(new LamdaNativeContext(
            procedure => NativeLibrary.GetExport(_openXrLibrary, procedure)));

        JniEnvironment.References.GetJavaVM(out var javaVm);
        _javaVm = javaVm;
        var androidInfo = new LoaderInitInfoAndroidKHR(
            applicationVM: (void*)_javaVm,
            applicationContext: (void*)Handle);

        // xrInitializeLoaderKHR must be resolved with a null instance and called
        // before extension enumeration or any other OpenXR operation.
        _loaderInit = new KhrLoaderInit(new LamdaNativeContext(procedure =>
        {
            PfnVoidFunction function = default;
            var result = _openXr.GetInstanceProcAddr(default, procedure, ref function);
            return result == XrResult.Success ? (nint)function : 0;
        }));

        var initializeResult = _loaderInit.InitializeLoader(
            (LoaderInitInfoBaseHeaderKHR*)&androidInfo);

        if (initializeResult != XrResult.Success)
        {
            throw new InvalidOperationException(
                $"xrInitializeLoaderKHR returned {initializeResult}.");
        }
    }

    private unsafe void CreateOpenXrInstanceAndFindHeadset()
    {
        if (_openXr is null || _javaVm == 0)
        {
            throw new InvalidOperationException("The OpenXR loader has not been initialized.");
        }

        uint extensionCount = 0;
        var result = _openXr.EnumerateInstanceExtensionProperties(
            (byte*)null, 0, ref extensionCount, null);
        EnsureSuccess(result, "xrEnumerateInstanceExtensionProperties(count)");

        var extensions = new ExtensionProperties[extensionCount];
        for (var index = 0; index < extensions.Length; index++)
        {
            extensions[index] = new ExtensionProperties();
        }

        fixed (ExtensionProperties* extensionPointer = extensions)
        {
            result = _openXr.EnumerateInstanceExtensionProperties(
                (byte*)null, extensionCount, ref extensionCount, extensionPointer);
        }
        EnsureSuccess(result, "xrEnumerateInstanceExtensionProperties(values)");

        var hasAndroidCreateInstance = false;
        var hasVulkanEnable2 = false;
        for (var index = 0; index < extensions.Length; index++)
        {
            fixed (byte* extensionName = extensions[index].ExtensionName)
            {
                var name = Marshal.PtrToStringAnsi((nint)extensionName);
                hasAndroidCreateInstance |= name == AndroidCreateInstanceExtension;
                hasVulkanEnable2 |= name == VulkanEnable2Extension;
            }
        }

        if (!hasAndroidCreateInstance)
        {
            throw new NotSupportedException(
                $"The active OpenXR runtime does not advertise {AndroidCreateInstanceExtension}.");
        }

        if (!hasVulkanEnable2)
        {
            throw new NotSupportedException(
                $"The active OpenXR runtime does not advertise {VulkanEnable2Extension}.");
        }

        var applicationInfo = new XrApplicationInfo(
            applicationVersion: 1,
            engineVersion: 1,
            apiVersion: 1UL << 48);

        byte* applicationName = applicationInfo.ApplicationName;
        Encoding.UTF8.GetBytes("MyVR Quest 2 Host")
            .CopyTo(new Span<byte>(applicationName, 128));

        byte* engineName = applicationInfo.EngineName;
        Encoding.UTF8.GetBytes("The Singularity Workshop")
            .CopyTo(new Span<byte>(engineName, 128));

        var androidCreateInfo = new InstanceCreateInfoAndroidKHR(
            applicationVM: (void*)_javaVm,
            applicationActivity: (void*)Handle);

        var androidExtensionBytes = Encoding.UTF8.GetBytes(AndroidCreateInstanceExtension + "\0");
        var vulkanExtensionBytes = Encoding.UTF8.GetBytes(VulkanEnable2Extension + "\0");
        fixed (byte* androidExtensionName = androidExtensionBytes)
        fixed (byte* vulkanExtensionName = vulkanExtensionBytes)
        {
            byte** enabledExtensions = stackalloc byte*[2];
            enabledExtensions[0] = androidExtensionName;
            enabledExtensions[1] = vulkanExtensionName;

            var createInfo = new InstanceCreateInfo(
                next: &androidCreateInfo,
                applicationInfo: applicationInfo,
                enabledExtensionCount: 2,
                enabledExtensionNames: enabledExtensions);

            result = _openXr.CreateInstance(&createInfo, ref _instance);
        }
        EnsureSuccess(result, "xrCreateInstance");
        _hasInstance = true;

        var systemGetInfo = new SystemGetInfo(formFactor: FormFactor.HeadMountedDisplay);
        result = _openXr.GetSystem(_instance, ref systemGetInfo, ref _systemId);
        EnsureSuccess(result, "xrGetSystem(HMD)");

        _vulkanEnable2 = new KhrVulkanEnable2(new LamdaNativeContext(procedure =>
        {
            PfnVoidFunction function = default;
            var procResult = _openXr.GetInstanceProcAddr(_instance, procedure, ref function);
            return procResult == XrResult.Success ? (nint)function : 0;
        }));

        var graphicsRequirements = new GraphicsRequirementsVulkanKHR();
        result = _vulkanEnable2.GetVulkanGraphicsRequirements2(
            _instance,
            _systemId,
            ref graphicsRequirements);
        EnsureSuccess(result, "xrGetVulkanGraphicsRequirements2KHR");

        _vulkanMinApiVersion = graphicsRequirements.MinApiVersionSupported;
        _vulkanMaxApiVersion = graphicsRequirements.MaxApiVersionSupported;
    }

    private unsafe void CreateVulkanDeviceAndOpenXrSession()
    {
        if (_openXr is null || _vulkanEnable2 is null || !_hasInstance)
        {
            throw new InvalidOperationException("The OpenXR instance and Vulkan extension are not ready.");
        }

        _vulkanLibrary = NativeLibrary.Load("libvulkan.so");
        _vulkan = new VkApi(new LamdaNativeContext(
            procedure => NativeLibrary.GetExport(_vulkanLibrary, procedure)));

        var getInstanceProcAddr = _vulkan.GetInstanceProcAddr(default, "vkGetInstanceProcAddr");
        if (getInstanceProcAddr.Handle == 0)
        {
            throw new InvalidOperationException("vkGetInstanceProcAddr could not be resolved.");
        }

        var appNameBytes = Encoding.UTF8.GetBytes("MyVR Quest 2 Host\\0");
        var engineNameBytes = Encoding.UTF8.GetBytes("The Singularity Workshop\\0");
        fixed (byte* appName = appNameBytes)
        fixed (byte* engineName = engineNameBytes)
        {
            var applicationInfo = new VkApplicationInfo(
                pApplicationName: appName,
                applicationVersion: 1,
                pEngineName: engineName,
                engineVersion: 1,
                apiVersion: ToVulkanApiVersion(_vulkanMinApiVersion));
            var instanceCreateInfo = new VkInstanceCreateInfo(
                pApplicationInfo: &applicationInfo);

            var xrInstanceCreateInfo = new VulkanInstanceCreateInfoKHR(
                systemId: _systemId,
                pfnGetInstanceProcAddr: getInstanceProcAddr,
                vulkanCreateInfo: &instanceCreateInfo);

            var vkInstanceHandle = default(Silk.NET.Core.Native.VkHandle);
            uint vulkanResult = 0;
            var xrResult = _vulkanEnable2.CreateVulkanInstance(
                _instance,
                ref xrInstanceCreateInfo,
                ref vkInstanceHandle,
                ref vulkanResult);
            EnsureSuccess(xrResult, "xrCreateVulkanInstanceKHR");
            EnsureVulkanSuccess(vulkanResult, "vkCreateInstance");
            _vulkanInstance = new VkInstance(vkInstanceHandle.Handle);
            _hasVulkanInstance = true;

            var graphicsDeviceInfo = new VulkanGraphicsDeviceGetInfoKHR(
                systemId: _systemId,
                vulkanInstance: vkInstanceHandle);
            var physicalDeviceHandle = default(Silk.NET.Core.Native.VkHandle);
            xrResult = _vulkanEnable2.GetVulkanGraphicsDevice2(
                _instance,
                ref graphicsDeviceInfo,
                ref physicalDeviceHandle);
            EnsureSuccess(xrResult, "xrGetVulkanGraphicsDevice2KHR");
            _vulkanPhysicalDevice = new VkPhysicalDevice(physicalDeviceHandle.Handle);

            uint queueFamilyCount = 0;
            _vulkan.GetPhysicalDeviceQueueFamilyProperties(
                _vulkanPhysicalDevice,
                &queueFamilyCount,
                null);
            if (queueFamilyCount == 0)
            {
                throw new NotSupportedException("The runtime-selected Vulkan device exposes no queue families.");
            }

            var queueFamilies = new VkQueueFamilyProperties[(int)queueFamilyCount];
            fixed (VkQueueFamilyProperties* queueFamilyPointer = queueFamilies)
            {
                _vulkan.GetPhysicalDeviceQueueFamilyProperties(
                    _vulkanPhysicalDevice,
                    &queueFamilyCount,
                    queueFamilyPointer);
            }

            var foundGraphicsQueue = false;
            for (uint index = 0; index < queueFamilyCount; index++)
            {
                var queueFamily = queueFamilies[index];
                if (queueFamily.QueueCount > 0 && (queueFamily.QueueFlags & VkQueueFlags.GraphicsBit) != 0)
                {
                    _queueFamilyIndex = index;
                    foundGraphicsQueue = true;
                    break;
                }
            }

            if (!foundGraphicsQueue)
            {
                throw new NotSupportedException("The runtime-selected Vulkan device exposes no graphics queue.");
            }

            var queuePriority = 1.0f;
            var queueCreateInfo = new VkDeviceQueueCreateInfo(
                queueFamilyIndex: _queueFamilyIndex,
                queueCount: 1,
                pQueuePriorities: &queuePriority);
            var enabledFeatures = new VkPhysicalDeviceFeatures();
            var deviceCreateInfo = new VkDeviceCreateInfo(
                queueCreateInfoCount: 1,
                pQueueCreateInfos: &queueCreateInfo,
                pEnabledFeatures: &enabledFeatures);
            var xrDeviceCreateInfo = new VulkanDeviceCreateInfoKHR(
                systemId: _systemId,
                pfnGetInstanceProcAddr: getInstanceProcAddr,
                vulkanPhysicalDevice: physicalDeviceHandle,
                vulkanCreateInfo: &deviceCreateInfo);

            var vkDeviceHandle = default(Silk.NET.Core.Native.VkHandle);
            vulkanResult = 0;
            xrResult = _vulkanEnable2.CreateVulkanDevice(
                _instance,
                ref xrDeviceCreateInfo,
                ref vkDeviceHandle,
                ref vulkanResult);
            EnsureSuccess(xrResult, "xrCreateVulkanDeviceKHR");
            EnsureVulkanSuccess(vulkanResult, "vkCreateDevice");
            _vulkanDevice = new VkDevice(vkDeviceHandle.Handle);
            _hasVulkanDevice = true;

            _vulkan.GetDeviceQueue(_vulkanDevice, _queueFamilyIndex, 0, out _graphicsQueue);

            var graphicsBinding = new GraphicsBindingVulkan2KHR(
                instance: vkInstanceHandle,
                physicalDevice: physicalDeviceHandle,
                device: vkDeviceHandle,
                queueFamilyIndex: _queueFamilyIndex,
                queueIndex: 0);
            var sessionCreateInfo = new SessionCreateInfo(
                next: &graphicsBinding,
                systemId: _systemId);
            xrResult = _openXr.CreateSession(_instance, &sessionCreateInfo, ref _session);
            EnsureSuccess(xrResult, "xrCreateSession(Vulkan)");
            _hasSession = true;
        }
    }

    private static uint ToVulkanApiVersion(ulong xrVersion)
    {
        var major = (uint)((xrVersion >> 48) & 0x7f);
        var minor = (uint)((xrVersion >> 32) & 0x3ff);
        var patch = (uint)(xrVersion & 0xfff);
        return (major << 22) | (minor << 12) | patch;
    }

    private static void EnsureVulkanSuccess(uint result, string operation)
    {
        if (result != (uint)VkResult.Success)
        {
            throw new InvalidOperationException($"{operation} returned {(VkResult)result}.");
        }
    }

    private static void EnsureSuccess(XrResult result, string operation)
    {
        if (result != XrResult.Success)
        {
            throw new InvalidOperationException($"{operation} returned {result}.");
        }
    }

    private void ReleaseOpenXr()
    {
        if (_hasSession && _openXr is not null)
        {
            var result = _openXr.DestroySession(_session);
            if (result != XrResult.Success)
            {
                Log.Warn(LogTag, $"xrDestroySession returned {result}.");
            }

            _hasSession = false;
            _session = default;
        }

        if (_hasVulkanDevice && _vulkan is not null)
        {
            _vulkan.DestroyDevice(_vulkanDevice, null);
            _hasVulkanDevice = false;
            _vulkanDevice = default;
            _graphicsQueue = default;
        }

        if (_hasVulkanInstance && _vulkan is not null)
        {
            _vulkan.DestroyInstance(_vulkanInstance, null);
            _hasVulkanInstance = false;
            _vulkanInstance = default;
            _vulkanPhysicalDevice = default;
        }

        _vulkanEnable2?.Dispose();
        _vulkanEnable2 = null;

        if (_hasInstance && _openXr is not null)
        {
            var result = _openXr.DestroyInstance(_instance);
            if (result != XrResult.Success)
            {
                Log.Warn(LogTag, $"xrDestroyInstance returned {result}.");
            }

            _hasInstance = false;
            _instance = default;
            _systemId = 0;
        }

        _vulkan?.Dispose();
        _vulkan = null;

        if (_vulkanLibrary != 0)
        {
            NativeLibrary.Free(_vulkanLibrary);
            _vulkanLibrary = 0;
        }

        _loaderInit?.Dispose();
        _loaderInit = null;

        _openXr?.Dispose();
        _openXr = null;

        if (_openXrLibrary != 0)
        {
            NativeLibrary.Free(_openXrLibrary);
            _openXrLibrary = 0;
        }

        _javaVm = 0;
    }

    private void SetStatus(string message)
    {
        if (_status is not null)
        {
            _status.Text = message;
        }
    }
}
