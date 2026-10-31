![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)

# Quest 2 Android host bootstrap

## Purpose

This project is the first deployable Android application experiment for the standalone Quest 2 target. It is deliberately separate from `MyVR.Core`: Android lifecycle, OpenXR bindings, and native loader packaging belong to the device host, not the portable session contract.

## Current slice

- Targets `net10.0-android` and packages only the `arm64-v8a` ABI.
- References the existing `net8.0` `MyVR.Core` project without changing its target or dependencies.
- Uses Silk.NET OpenXR, KHR extension, and Vulkan 2.23.0 bindings.
- Includes the Khronos `openxr_loader_for_android` 1.1.62 AAR from Maven Central.
- Declares Meta's immersive-HMD Android launcher intent filter.
- CI publishes a signed APK, verifies `lib/arm64-v8a/libopenxr_loader.so`, and uploads the APK artifact.
- The Android Activity obtains the JavaVM and Activity JNI handle, resolves `xrInitializeLoaderKHR` through `xrGetInstanceProcAddr` with a null instance, and calls it before extension enumeration.
- It enumerates runtime extensions and requires both `XR_KHR_android_create_instance` and `XR_KHR_vulkan_enable2`.
- It creates an OpenXR instance with `XrInstanceCreateInfoAndroidKHR`, requests the head-mounted-display system, and queries the runtime's minimum/maximum Vulkan API version through `xrGetVulkanGraphicsRequirements2KHR`.
- It creates a Vulkan instance through `xrCreateVulkanInstanceKHR`, asks OpenXR for the runtime-selected physical device, selects a graphics queue family, creates the Vulkan device through `xrCreateVulkanDeviceKHR`, and creates an OpenXR session with `XrGraphicsBindingVulkan2KHR`.

The loader, Android instance creation, Vulkan instance/device creation, and OpenXR session-creation paths are implemented in the Android host. The Activity now polls OpenXR events on the Android main looper, begins the session on `READY`, ends it on `STOPPING`, and reports `VISIBLE`, `FOCUSED`, `LOSS_PENDING`, and `EXITING` states. The event pump is stopped during Activity destruction. Teardown destroys the session directly rather than calling `xrEndSession` outside the runtime's `STOPPING` transition; OpenXR permits `xrDestroySession` in any session state. If session destruction fails, teardown retains dependent Vulkan and OpenXR resources instead of invalidating handles that may still be live. Android pause/resume is currently only logged; it is not yet coordinated with the OpenXR session lifecycle. CI verifies compilation and APK packaging only; it does **not** execute these calls on a device. Physical Quest 2 runtime validation remains outstanding. The host still does **not** create a swapchain, submit frames, render the Workshop moniker, or process controller input. A successful APK build/package check is not proof of runtime initialization.

## CI evidence

On exact source SHA `d3b38c17d3856bf1d6b7ed90542743bd58f54d29`, both [Quest 2 Android workflow #41155491416](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41155491416) and [MyVR core CI #41155491402](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41155491402) passed. Android CI completed restore, Release build, signed APK publish, packaged-loader verification, and artifact upload. The [ARM64 APK artifact #13025102380](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41155491416/artifacts/13025102380) is 5,608,648 bytes and retained for seven days. The documentation-only follow-up at exact SHA `ab0435b084f149e916ee6043fa4659ecf49dbbcc` also passed [Android CI #41156189308](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41156189308) and [core CI #41156189309](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41156189309). These workflows prove build and packaging only, not runtime behavior on a headset.

## Why .NET 10 Android

The Khronos loader is distributed as an Android AAR. The `AndroidMavenLibrary` build item that resolves Maven artifacts is available in .NET 9 and later, so this host uses the current .NET 10 Android target while continuing to consume the portable .NET 8 core. The Android project does not force a framework upgrade on `MyVR.Core`.

The AAR packages `libopenxr_loader.so`. The host explicitly loads that exact library name and supplies Silk.NET with a native context backed by its exports; Silk.NET's default Android search name includes a `.so.1` suffix and does not match the AAR's packaged filename.

## Next gates

1. Install and execute the current signed APK on physical Quest 2; capture loader initialization, extension enumeration, instance creation, HMD-system selection, Vulkan device creation, session creation, and observed session-state transitions.
2. Complete Android pause/resume and Activity-destruction coordination with the OpenXR session state machine; verify teardown behavior on device.
3. Create and enumerate the projection swapchain using the session's supported view configuration and formats.
4. Implement a minimal wait/begin/acquire/wait/release/end frame loop and submit a clear frame on physical Quest 2.
5. Record device/runtime versions and actual headset evidence.
6. Add spatial rendering and semantic controller input only after the frame loop is proven.

If managed Android/OpenXR interop requires fragile runtime-specific workarounds, stop and move the native OpenXR host behind an explicit bridge rather than leaking those details into `MyVR.Core`.

## Official references

- [Meta Quest OpenXR support](https://developers.meta.com/vr/documentation/native/android/mobile-openxr/)
- [Meta OpenXR instance and session creation](https://developers.meta.com/vr/documentation/native/android/mobile-openxr-instance-session/)
- [Khronos OpenXR session lifecycle specification](https://registry.khronos.org/OpenXR/specs/1.1/html/xrspec.html#session)
- [Khronos OpenXR SDK releases and Android loader](https://github.com/KhronosGroup/OpenXR-SDK/releases)
- [Silk.NET OpenXR bindings](https://www.nuget.org/packages/Silk.NET.OpenXR/)

---

<p align="center">
  <a href="https://github.com/TrentBest/TheSingularityWorkshop" aria-label="The Singularity Workshop">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="112" />
  </a><br />
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em>
</p>
