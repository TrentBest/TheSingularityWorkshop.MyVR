![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)

# Quest 2 Android host bootstrap

## Purpose

This project is the first deployable Android application experiment for the standalone Quest 2 target. It is deliberately separate from `MyVR.Core`: Android lifecycle, OpenXR bindings, and native loader packaging belong to the device host, not the portable session contract.

## Current slice

- Targets `net10.0-android` and packages only the `arm64-v8a` ABI.
- References the existing `net8.0` `MyVR.Core` project without changing its target or dependencies.
- Uses Silk.NET OpenXR 2.23.0 bindings and the KHR extension bindings.
- Includes the Khronos `openxr_loader_for_android` 1.1.62 AAR from Maven Central.
- Declares Meta's immersive-HMD Android launcher intent filter.
- Loads the packaged native loader library and reports success or failure in the app.

The host currently does **not** initialize the OpenXR loader with Android VM/activity context, create an OpenXR instance/system/session, create a graphics binding or swapchain, submit frames, render the Workshop moniker, or process controller input. Loading the shared library is a packaging smoke check only; it is not an OpenXR runtime or headset test.

## Why .NET 10 Android

The Khronos loader is distributed as an Android AAR. The `AndroidMavenLibrary` build item that resolves Maven artifacts is available in .NET 9 and later, so this host uses the current .NET 10 Android target while continuing to consume the portable .NET 8 core. The Android project does not force a framework upgrade on `MyVR.Core`.

## Next gates

1. Build and package this Android application in CI.
2. Obtain the JavaVM and Activity references safely from the Android host and call `xrInitializeLoaderKHR` before OpenXR enumeration or instance creation.
3. Enable `XR_KHR_android_create_instance` and pass the Android instance-create structure.
4. Create an HMD system and Android-compatible graphics session/swapchain.
5. Handle OpenXR and Android pause/resume lifecycle correctly.
6. Install and run on a physical Quest 2, then record device/runtime versions and actual evidence.
7. Add spatial rendering and semantic controller input only after the runtime/session path is proven.

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
