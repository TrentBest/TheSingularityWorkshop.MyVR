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

The loader, Android instance creation, Vulkan instance/device creation, and OpenXR session-creation paths are implemented in the Android host. CI verifies compilation and APK packaging only; it does **not** execute these calls on a device. The OpenXR session is created but is not yet driven through session events or begun. Physical Quest 2 runtime validation remains outstanding. The host still does **not** create a swapchain, submit frames, render the Workshop moniker, or process controller input. A successful APK build/package check is not proof of runtime initialization.

## CI evidence

The Android Release build, signed APK publish, packaged-loader verification, and artifact upload passed on exact source SHA `877df3c7884c92c1ca544c4960cfeab9a3014e69`: [Quest 2 Android workflow #41154240252](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41154240252). The portable core CI also passed on that SHA: [MyVR CI #41154240278](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41154240278). The APK artifact is [myvr-quest2-android-arm64-apk](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41154240252/artifacts/13024666748) (SHA-256 `a34ca6782b71993c46ea9e13c77f3bef3189750b2ee4870104af32fb26080a34`).

## Why .NET 10 Android

The Khronos loader is distributed as an Android AAR. The `AndroidMavenLibrary` build item that resolves Maven artifacts is available in .NET 9 and later, so this host uses the current .NET 10 Android target while continuing to consume the portable .NET 8 core. The Android project does not force a framework upgrade on `MyVR.Core`.

The AAR packages `libopenxr_loader.so`. The host explicitly loads that exact library name and supplies Silk.NET with a native context backed by its exports; Silk.NET's default Android search name includes a `.so.1` suffix and does not match the AAR's packaged filename.

## Next gates

1. Install and execute the current signed APK on physical Quest 2; capture loader initialization, extension enumeration, instance creation, HMD-system selection, Vulkan device creation, and session creation.
2. Implement OpenXR session event polling and READY/FOCUSED lifecycle handling, including Android pause/resume.
3. Create and enumerate the projection swapchain using the session's supported view configuration and formats.
4. Implement a minimal wait/begin/acquire/wait/release/end frame loop and submit a clear frame on physical Quest 2.
5. Record device/runtime versions and actual headset evidence.
6. Add spatial rendering and semantic controller input only after the frame loop is proven.

If managed Android/OpenXR interop requires fragile runtime-specific workarounds, stop and move the native OpenXR host behind an explicit bridge rather than leaking those details into `MyVR.Core`.

## Official references

- [Meta Quest OpenXR support](https://developers.meta.com/vr/documentation/native/android/mobile-openxr/)
- [Meta OpenXR instance and session creation](https://developers.meta.com/vr/documentation/native/android/mobile-openxr-instance-session/)
- [Khronos OpenXR SDK releases and Android loader](https://github.com/KhronosGroup/OpenXR-SDK/releases)
- [Silk.NET OpenXR bindings](https://www.nuget.org/packages/Silk.NET.OpenXR/)

---

<p align="center">
  <a href="https://github.com/TrentBest/TheSingularityWorkshop" aria-label="The Singularity Workshop living avatar">
    <img src="https://raw.githubusercontent.com/TrentBest/WebPage/development/TheSingularityWorkshop/wwwroot/Images/TheSingularityWorkshopLivingAvatar.svg" alt="The Singularity Workshop living avatar" width="112" />
  </a><br />
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em>
</p>
