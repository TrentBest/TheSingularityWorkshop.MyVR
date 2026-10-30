![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)

# Quest 2 OpenXR host feasibility

## Decision

Validate a **.NET for Android host with Silk.NET OpenXR bindings first**, while keeping a native Khronos/Meta C++ OpenXR host as the explicit fallback. This is a time-boxed feasibility choice, not an approved long-term runtime commitment.

The reason to test managed hosting first is concrete: Silk.NET publishes both `Silk.NET.OpenXR` and `Silk.NET.OpenXR.Extensions.ANDROID`; its Android extension project targets .NET Standard 2.0/2.1 and .NET 5, and Silk.NET documents Android workloads in its build prerequisites. That establishes a plausible compatibility path for a `net8.0-android` application referencing the existing `net8.0` MyVR.Core. It does **not** prove the Android loader, JNI context, graphics binding, or Quest runtime work end-to-end.

## Platform facts that constrain the host

Meta's current Quest 2 native path is OpenXR on Android. New work must not use the deprecated Mobile SDK/VrApi path.

The Android application must:

1. Declare the immersive HMD launcher intent filter:
   - `android.intent.action.MAIN`
   - `org.khronos.openxr.intent.category.IMMERSIVE_HMD`
   - `android.intent.category.LAUNCHER`
2. Use Khronos OpenXR Android Loader **1.0.34 or newer**. Meta documents that older loader versions crash on supported Quest OS versions.
3. Initialize the loader before instance enumeration/creation, using `xrInitializeLoaderKHR` and `XrLoaderInitInfoAndroidKHR` populated with the Android VM and Activity/context.
4. Chain Android application VM/Activity data through `XR_KHR_android_create_instance` when creating the OpenXR instance.
5. Select and validate a real Android graphics binding (OpenGL ES or Vulkan), then create the OpenXR session and swapchains.

The Khronos `hello_xr` Android sample demonstrates the loader initialization sequence using the NativeActivity VM and Activity object, followed by instance/system/session/swapchain creation and a lifecycle-aware event/render loop. It is the reference behavior, not code to copy wholesale into MyVR.

## Managed-host feasibility gate

The first implementation experiment should be deliberately smaller than the Moniker vertical slice:

- Create a deployable `net8.0-android` app project outside MyVR.Core.
- Reference the existing MyVR.Core project without changing its target framework or adding Android/OpenXR dependencies to it.
- Pin Silk.NET OpenXR packages to a reviewed version; the current package listing identifies 2.23.0. Verify the Android extension API surface and native loader packaging before relying on either.
- Implement only the Android loader initialization and OpenXR instance/system/session lifecycle first. Add a clear diagnostic for every failed OpenXR result.
- Add the Quest immersive manifest filter and target a physical Quest 2.
- Once lifecycle works, add the smallest valid graphics binding, swapchain, frame submission, and a simple test presentation.
- Keep controller action mapping behind a semantic MyVR interaction boundary; do not add FSM_UserIO before its dependency gate is satisfied.

### Pass criteria

The managed route remains viable only when the same app can:

1. Restore and build as `net8.0-android` with the documented Android workload/toolchain.
2. Package a Khronos loader version at or above 1.0.34 and load it on Quest 2.
3. Initialize the loader with the actual Android VM/Activity context.
4. Create an OpenXR instance, identify the HMD system, create a session with a supported graphics binding, and create swapchains.
5. Enter and leave the immersive session across pause/resume without leaking or corrupting session state.
6. Render and submit a frame on the physical Quest 2.

A desktop OpenXR run, simulator, successful C# compilation, or headless CI is not a pass for the headset criteria.

### Fallback

If the managed experiment cannot satisfy loader initialization, native library packaging, graphics interop, or lifecycle requirements without fragile workarounds, use the Khronos/Meta-supported native C/C++ Android OpenXR host. Keep the host boundary narrow and explicit. Do not pretend native C++ can directly consume the C# core: define and test a small interop contract before coupling the two.

## Experience integration is intentionally later

This host spike must not invent or duplicate the Workshop Moniker Experience. The final vertical slice must consume the published, host-neutral Moniker artifact and canonical session/requirements contracts owned by Experiences. Until those contracts are available, a generic test presentation is only an OpenXR host smoke test and must be labeled as such.

No companion is required for this first device proof. The first Moniker manifestation must run standalone; companion services remain optional and capability-declared.

## Evidence and limits

- Meta: [OpenXR support for Quest and Quest 2](https://developers.meta.com/vr/documentation/native/android/mobile-openxr/) — Quest 2 OpenXR adoption, manifest filter, loader minimum, Android loader initialization and instance-creation requirements.
- Meta: [OpenXR Mobile SDK](https://developers.meta.com/vr/documentation/native/android/mobile-intro/) — current native direction and Mobile SDK/VrApi deprecation.
- Khronos: [OpenXR hello_xr Android sample](https://github.com/KhronosGroup/OpenXR-SDK-Source/blob/main/src/tests/hello_xr/main.cpp) — Android VM/Activity loader initialization and lifecycle/render loop.
- Silk.NET: [OpenXR package](https://www.nuget.org/packages/Silk.NET.OpenXR) and [Android extension source project](https://github.com/dotnet/Silk.NET/tree/main/src/OpenXR/Extensions/Silk.NET.OpenXR.Extensions.ANDROID) — available bindings and target frameworks.
- Silk.NET's [repository build prerequisites](https://github.com/dotnet/Silk.NET) document Android workloads.

**Not yet proven:** a .NET Android app using Silk.NET can package and initialize the Khronos loader on Quest 2, create the required Android graphics binding, or render on physical hardware. No device test is claimed by this report.

---

<p align="center">
  <a href="https://github.com/TrentBest/TheSingularityWorkshop" aria-label="The Singularity Workshop living avatar">
    <img src="https://raw.githubusercontent.com/TrentBest/WebPage/development/TheSingularityWorkshop/wwwroot/Images/TheSingularityWorkshopLivingAvatar.svg" alt="The Singularity Workshop living avatar" width="112" />
  </a><br />
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em><br />
  <a href="https://github.com/TrentBest">GitHub</a> · <a href="https://www.patreon.com/TheSingularityWorkshop">Support the Workshop</a>
</p>
