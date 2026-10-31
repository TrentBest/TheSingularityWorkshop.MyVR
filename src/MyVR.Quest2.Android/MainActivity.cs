using System.Runtime.InteropServices;
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
    private TextView? _status;
    private XR? _openXr;
    private KhrLoaderInit? _loaderInit;
    private nint _openXrLibrary;

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
            SetStatus(
                "Android host launched. Khronos OpenXR loader initialized with Android VM/context.\n\n" +
                "OpenXR instance/system/session creation, graphics binding, swapchain, and " +
                "headset rendering are not implemented yet.");
            Log.Info(LogTag, "Khronos OpenXR loader initialized with Android VM/context.");
        }
        catch (Exception exception)
        {
            SetStatus(
                "Android host launched, but OpenXR loader initialization failed.\n\n" +
                exception);
            Log.Error(LogTag, $"OpenXR loader initialization failed: {exception}");
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
        var androidInfo = new LoaderInitInfoAndroidKHR(
            applicationVM: (void*)javaVm,
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

    private void ReleaseOpenXr()
    {
        _loaderInit?.Dispose();
        _loaderInit = null;

        _openXr?.Dispose();
        _openXr = null;

        if (_openXrLibrary != 0)
        {
            NativeLibrary.Free(_openXrLibrary);
            _openXrLibrary = 0;
        }
    }

    private void SetStatus(string message)
    {
        if (_status is not null)
        {
            _status.Text = message;
        }
    }
}
