<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# 05 — The Quest 2 Android Host Bootstrap

## 01 What — a deployable host is not yet an immersive application

The portable MyVR core can be built and tested without a headset. A standalone VR device still needs an operating-system application that can load the native VR runtime, discover the headset system, create graphics resources accepted by that runtime, and coordinate the runtime session with Android.

The Quest 2 implementation is a separate Android ARM64 application. It is not a new MyVR core and it is not a general-purpose VR engine. It is the first device-host adapter that attempts to bring the portable MyVR boundary to a real standalone headset.

The implementation is a bootstrap, not a finished immersive experience. It currently reaches OpenXR session creation and lifecycle coordination. It does not yet render a scene or accept controller/hand input.

## 02 Why — native platform concerns stay outside the portable core

Android lifecycle callbacks, JavaVM/Activity handles, OpenXR native handles, Vulkan devices, and vendor/runtime extension availability are platform concerns. Putting those types into MyVR.Core would make portable identity and session rules depend on a specific device stack.

The intended boundary is:

```text
MyVR.Core (.NET 8)
  |  portable identity, capability, session contracts
  |
MyVR.Quest2.Android (.NET 10 Android / ARM64)
  |  Android Activity + OpenXR + Vulkan host
  |
Quest 2 OpenXR runtime and device
```

The Android project references MyVR.Core. The core does not reference Android, OpenXR, Vulkan, or the headset SDK.

## 03 How — the bootstrap sequence

The current host candidate performs the following sequence:

1. **Android application startup.** The Quest launcher starts the Android Activity. The application targets the ARM64 ABI.
2. **Loader initialization.** The host obtains the JavaVM and Activity JNI context, resolves `xrInitializeLoaderKHR`, and initializes the Android OpenXR loader before runtime extension enumeration.
3. **Extension discovery.** The host enumerates available OpenXR extensions and requires `XR_KHR_android_create_instance` and `XR_KHR_vulkan_enable2`.
4. **OpenXR instance creation.** It creates an Android-aware OpenXR instance and requests the head-mounted-display system.
5. **Runtime graphics requirements.** It asks the active OpenXR runtime for its minimum and maximum supported Vulkan API versions.
6. **Runtime-selected Vulkan device.** It asks OpenXR to create the Vulkan instance, identifies the physical device selected by the runtime, selects a compatible graphics queue family, and asks OpenXR to create the Vulkan device.
7. **OpenXR session creation.** It creates the session using the Vulkan 2 graphics binding.
8. **Runtime event processing.** The host polls OpenXR events on the Android main looper and coordinates the session with Activity resume state.
9. **Ordered teardown.** The Activity stops event processing and retires native resources in dependency order.

The exact source and dependency versions are authoritative. This chapter explains the candidate path; it does not imply that the path has been exercised on a physical headset.

## 04 See it in a minute — understand the lifecycle gate

The current host coordinates these OpenXR events:

| Runtime state/event | Host behavior |
|---|---|
| `READY` | Begin the OpenXR session only when the Android Activity is resumed |
| `VISIBLE` | Record that the session is visible |
| `FOCUSED` | Record that the session has input focus |
| `STOPPING` | Call `xrEndSession` |
| `LOSS_PENDING` | Retire the terminal session and stop polling |
| `EXITING` | Retire the terminal session and stop polling |
| `INSTANCE_LOSS_PENDING` | Stop polling and finish the Activity for ordered teardown |

A runtime may report `READY` before Android has resumed the Activity. The host therefore waits until both conditions are true before beginning the session. Conversely, Android pause is not itself permission to call `xrEndSession`; the host waits for the runtime's `STOPPING` event.

Session recreation after loss is not implemented. A fresh Activity initialization is currently required.

## Implementation mapping

The current implementation candidate is [draft PR #16](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/pull/16):

- [Android host project](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/tree/feature/quest2-android-host-bootstrap/src/MyVR.Quest2.Android)
- [MainActivity.cs](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/quest2-android-host-bootstrap/src/MyVR.Quest2.Android/MainActivity.cs)
- [Android project file](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/quest2-android-host-bootstrap/src/MyVR.Quest2.Android/MyVR.Quest2.Android.csproj)
- [First-device bootstrap guide](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/quest2-android-host-bootstrap/docs/QUEST2_ANDROID_HOST_BOOTSTRAP.md)

The PR head at the time of this theory update is `8c0dd80f9a51ab90fac034c435ed15134cdce949`. Its [core CI run](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41227205130) and [Android host build/package run](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41227205152) succeeded for that exact head.

## Evidence, limits, and the physical-device gate

CI currently establishes that:
- the .NET 8 core restores, builds, and passes its deterministic tests;
- the .NET 10 Android host restores and builds in Release;
- the host publishes a signed ARM64 APK;
- the APK contains `lib/arm64-v8a/libopenxr_loader.so`.

CI does **not** establish that:
- the headset loads the native library successfully;
- the runtime exposes the required extensions;
- the HMD system is selected successfully;
- Vulkan instance/device creation succeeds on the actual device;
- the OpenXR session begins and responds correctly to Android lifecycle changes;
- teardown is safe on the actual runtime.

The first physical test is deliberately narrow. Install and launch the APK, capture the complete `MyVR.Quest2` log, record headset model and OS/runtime version, and verify loader initialization, extension enumeration, instance creation, HMD selection, Vulkan creation, session creation, pause/resume, and teardown.

Do not add swapchain/frame-loop work until this bootstrap gate passes on the physical headset. The host currently has no swapchain, frame submission, spatial rendering, live tracking, controller/hand input, spatial audio, or haptics.

## Practice

1. Why must OpenXR initialize its Android loader before extension enumeration?
2. Why does the host ask OpenXR to select the Vulkan physical device instead of choosing an arbitrary GPU?
3. Why should `OnPause` not independently call `xrEndSession`?
4. Which exact log events would prove that the runtime progressed from loader initialization to session creation?
5. Which CI checks can be repeated without a headset, and which claims require physical-device evidence?

**Exercise:** design a failure table for each bootstrap stage. For each failure, specify the native resources already acquired, the cleanup order, the log evidence, and whether retry requires Activity restart or a new process. Do not invent a retry path that the implementation does not provide.

## Further reading

- [Theory index](README.md)
- [Quest 2 Android host bootstrap guide](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/quest2-android-host-bootstrap/docs/QUEST2_ANDROID_HOST_BOOTSTRAP.md)
- [Implementation Notes](../IMPLEMENTATION_NOTES.md)
- [Roadmap](../ROADMAP.md)
- [OpenXR host feasibility PR #15](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/pull/15)
- [Android host PR #16](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/pull/16)

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
