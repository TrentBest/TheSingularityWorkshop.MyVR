using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Widget;

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

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        _status = new TextView(this)
        {
            TextSize = 18,
            Text = "MyVR Quest 2 host bootstrap\nChecking packaged OpenXR loader…"
        };
        _status.SetPadding(32, 32, 32, 32);
        SetContentView(_status);

        try
        {
            Java.Lang.JavaSystem.LoadLibrary("openxr_loader");
            SetStatus(
                "Android host launched. Khronos OpenXR loader library loaded.\n\n" +
                "OpenXR loader initialization, instance/session creation, graphics " +
                "binding, and headset rendering are not implemented yet.");
            Log.Info(LogTag, "Khronos OpenXR loader library loaded.");
        }
        catch (Exception exception)
        {
            SetStatus(
                "Android host launched, but the OpenXR loader could not be loaded.\n\n" +
                exception);
            Log.Error(LogTag, exception, "OpenXR loader load failed.");
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
        Log.Info(LogTag, "Android activity destroyed.");
        base.OnDestroy();
    }

    private void SetStatus(string message)
    {
        if (_status is not null)
        {
            _status.Text = message;
        }
    }
}
