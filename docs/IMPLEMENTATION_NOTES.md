# Implementation Notes

MyVR has two deliberately separate executable layers: a portable, headless .NET 8 core and a candidate standalone Quest 2 Android/OpenXR host. The host is implemented and CI-packaged; physical Quest runtime validation remains outstanding.

## Implemented core boundary

The core currently contains:

- immutable Experience artifact identity: BundleId + Version + ContentHash;
- a separate MyVR session GUID and explicit immersive-session lifecycle;
- immutable device capability snapshots with adapter-owned capability names;
- IDeviceRuntimeAdapter, the narrow boundary for device/runtime integrations;
- deterministic tests using a fake runtime adapter, without a headset or companion.

The core has no compile-time dependency on FSM_COS, FSM_UserIO, GUI, AnyApp, a VR SDK, or a repository client.

## Quest 2 Android host

The current device-host candidate is a separate .NET 10 Android ARM64 application targeting standalone Quest 2. It references the portable .NET 8 core; Android, OpenXR, and Vulkan implementation details remain outside that core.

Implemented host paths include:

- Khronos Android OpenXR loader AAR packaging and Android loader initialization using the JavaVM and Activity context;
- required OpenXR extension checks for Android instance creation and Vulkan enable2;
- Android OpenXR instance creation and HMD system selection;
- runtime Vulkan API requirement query and runtime-selected Vulkan physical device;
- OpenXR-mediated Vulkan instance/device creation and Vulkan2 graphics binding for session creation;
- Android-main-looper event polling and session lifecycle coordination;
- READY/resume gating, STOPPING-driven xrEndSession, visibility/focus state handling, terminal session loss/exit handling, and ordered teardown.

The host has no swapchain, frame loop, frame submission, rendering, live tracking, or controller/hand input. Session recreation is not implemented; a fresh Activity initialization is required after session loss.

## Validation status and limits

The current CI evidence is hardware-free:

- the portable core workflow restores, builds, and tests the .NET 8 core;
- the Quest Android workflow restores and builds the .NET 10 Android host in Release, publishes a signed ARM64 APK, verifies the packaged libopenxr_loader.so, and uploads the artifact.

These checks establish source/build/package integrity only. They do not prove that a physical Quest runtime loads the native library, accepts the requested extensions, selects an HMD system, creates Vulkan resources, creates an OpenXR session, or follows the expected lifecycle.

The first physical-device pass must capture the headset model, OS/runtime version, install and launch results, and complete MyVR.Quest2 log. Verify loader initialization, extension enumeration, instance creation, HMD selection, Vulkan device creation, session creation, pause/resume, and teardown before expanding into swapchain/frame-loop work.

A green headless core test run is not headset verification. A successful APK build/package check is not runtime initialization evidence.

## Dependency direction

MyVR.Core (.NET 8, portable)
  -> Experience artifact identity (local immutable value contract)
  -> MyVR session lifecycle (local host concern)
  -> IDeviceRuntimeAdapter
       -> platform/runtime host adapter
            -> Quest 2 Android host (.NET 10): Android lifecycle, OpenXR loader/session, Vulkan graphics binding
            -> future browser/native runtime adapters

A future semantic-input adapter may translate device observations into FSM_UserIO semantic values. Keep that integration outside the device-independent core until the exact package/version/source and dependency direction pass the required CiAi dependency-policy gate (MyVR issue #6 / CiAi issue #21). A semantic intent is not authorization or execution permission.

## First executable core slice

Implemented:

1. A MyVR session can be created for a validated immutable Experience identity.
2. Device capabilities can be represented without importing a vendor/runtime type.
3. A runtime adapter can report capabilities through a narrow asynchronous interface.
4. The session follows an explicit lifecycle and rejects invalid transitions.
5. Suspension/resume, disconnect, and reconnect preserve Experience and session identity.
6. Deterministic tests exercise the core and a fake adapter without hardware.

Still outstanding:

1. Physical Quest 2 runtime verification of the implemented Android/OpenXR host.
2. Runtime capability detection backed by a real headset.
3. Resolution and verification of a published Experience artifact by its immutable identity.
4. A minimal permitted Experience representation.
5. One bounded semantic interaction and Experience event.
6. Companion/network lifecycle tests against a real bounded protocol.
7. Swapchain creation, frame submission, spatial presentation, and live device input.

## Testing

The core test project covers identity validation/equality, capability reporting, the fake runtime adapter, valid/invalid session transitions, suspend/resume, disconnect, and reconnect.

Platform/device tests must remain separate from deterministic core tests. Record actual device/runtime versions and logs for headset claims.

## Technology rule

The portable core targets .NET 8, matching the Workshop's cross-platform library baseline. The current Quest host uses .NET 10 Android because its Android packaging/build requirements are separate from the core target. Do not move Android/OpenXR/Vulkan dependencies into MyVR.Core. Additional device/runtime choices remain behind the adapter boundary and should be selected only when target requirements, deployment constraints, licensing, and required APIs are understood.
