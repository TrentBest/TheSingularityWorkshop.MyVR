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
    private TextView? _status;
    private XR? _openXr;
    private KhrLoaderInit? _loaderInit;
    private nint _openXrLibrary;
    private nint _javaVm;
    private Instance _instance;
    private ulong _systemId;
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
            SetStatus(
                $"OpenXR initialized. HMD system handle: 0x{_systemId:x}.\n\n" +
                "Session creation, graphics binding, swapchain, frame submission, " +
                "Workshop moniker rendering, and controller input are not implemented yet.");
            Log.Info(LogTag, $"OpenXR instance created and HMD system found: 0x{_systemId:x}.");
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
            null, 0, ref extensionCount, null);
        EnsureSuccess(result, "xrEnumerateInstanceExtensionProperties(count)");

        var extensions = new ExtensionProperties[extensionCount];
        for (var index = 0; index < extensions.Length; index++)
        {
            extensions[index] = new ExtensionProperties();
        }

        fixed (ExtensionProperties* extensionPointer = extensions)
        {
            result = _openXr.EnumerateInstanceExtensionProperties(
                null, extensionCount, ref extensionCount, extensionPointer);
        }
        EnsureSuccess(result, "xrEnumerateInstanceExtensionProperties(values)");

        var hasAndroidCreateInstance = false;
        for (var index = 0; index < extensions.Length; index++)
        {
            fixed (byte* extensionName = extensions[index].ExtensionName)
            {
                if (Marshal.PtrToStringAnsi((nint)extensionName) == AndroidCreateInstanceExtension)
                {
                    hasAndroidCreateInstance = true;
                    break;
                }
            }
        }

        if (!hasAndroidCreateInstance)
        {
            throw new NotSupportedException(
                $"The active OpenXR runtime does not advertise {AndroidCreateInstanceExtension}.");
        }

        var applicationInfo = new ApplicationInfo(
            applicationVersion: 1,
            engineVersion: 1,
            apiVersion: 1UL << 48);

        fixed (byte* applicationName = applicationInfo.ApplicationName)
        {
            Encoding.UTF8.GetBytes("MyVR Quest 2 Host")
                .CopyTo(new Span<byte>(applicationName, 128));
        }

        fixed (byte* engineName = applicationInfo.EngineName)
        {
            Encoding.UTF8.GetBytes("The Singularity Workshop")
                .CopyTo(new Span<byte>(engineName, 128));
        }

        var androidCreateInfo = new InstanceCreateInfoAndroidKHR(
            applicationVM: (void*)_javaVm,
            applicationActivity: (void*)Handle);

        var extensionNameBytes = Encoding.UTF8.GetBytes(AndroidCreateInstanceExtension + "\0");
        fixed (byte* extensionName = extensionNameBytes)
        {
            byte** enabledExtensions = stackalloc byte*[1];
            enabledExtensions[0] = extensionName;

            var createInfo = new InstanceCreateInfo(
                next: &androidCreateInfo,
                applicationInfo: applicationInfo,
                enabledExtensionCount: 1,
                enabledExtensionNames: enabledExtensions);

            result = _openXr.CreateInstance(&createInfo, ref _instance);
        }
        EnsureSuccess(result, "xrCreateInstance");
        _hasInstance = true;

        var systemGetInfo = new SystemGetInfo(formFactor: FormFactor.HeadMountedDisplay);
        result = _openXr.GetSystem(_instance, ref systemGetInfo, ref _systemId);
        EnsureSuccess(result, "xrGetSystem(HMD)");
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
