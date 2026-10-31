![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Roadmap

## Phase 0 - Theory

Define MyVR as the VR manifestation/client and lock boundaries with Experience, AnyApp, WebApp, GUI, and FSM_COS.

## Phase 1 - Device Skeleton

**Status: In progress — Quest 2 Android/OpenXR host implemented; physical-device validation outstanding.**

Implemented in the portable .NET 8 core:

- [x] Portable core project and deterministic test project.
- [x] Immutable capability snapshot with adapter-owned capability names.
- [x] Device/runtime adapter interface; fake adapter exercised by tests.
- [x] Separate immutable Experience identity and MyVR session identity.
- [x] Explicit session lifecycle with invalid-transition rejection, suspend/resume, disconnect, and reconnect tests.
- [x] Pull-request and development build/test workflow.

Implemented in the separate Quest 2 Android host:

- [x] Candidate target selected: standalone Quest 2, Android ARM64, .NET 10 Android host over the portable .NET 8 core.
- [x] Silk.NET OpenXR/Vulkan integration and packaged Khronos Android OpenXR loader.
- [x] Android loader initialization, required extension checks, HMD system selection, runtime-selected Vulkan instance/device creation, and OpenXR session creation paths.
- [x] Android-coordinated OpenXR event polling and session lifecycle handling, including READY/resume gating, STOPPING, terminal session states, and ordered teardown.
- [x] CI Release build, signed APK packaging, packaged-loader verification, and artifact upload.

Still outstanding:

- [ ] Install and run the APK on a physical Quest 2; verify loader initialization, extension discovery, HMD selection, Vulkan creation, OpenXR session creation, and Android pause/resume lifecycle.
- [ ] Record device/runtime versions and preserve the first complete device log as runtime evidence.
- [ ] Add a semantic input adapter only after the FSM_UserIO dependency-policy gate is available and approved.
- [ ] Verify the first real device lifecycle without making frame-critical work network-dependent.

**Evidence boundary:** CI proves that the Android host builds and packages the loader into an APK. It does not prove runtime initialization or lifecycle behavior on a headset. The fake adapter proves only the portable core contract; it is not hardware detection. No physical Quest validation has yet been recorded.

The host currently has no swapchain, frame loop, frame submission, spatial rendering, live tracking, or controller/hand input. Those are subsequent gates, not implied by session creation.

## Phase 2 - Creator-defined Experience requirements

Resolve an immutable published Experience identity and its declared capability requirements.

Determine whether the current VR device can satisfy the required immersive and interaction capabilities locally, and identify optional/delegable work that may be supplied by WebApp and/or AnyApp.

A requirement failure is an Experience capability mismatch, not a reason to redefine MyVR's architectural role.

## Phase 3 - First Experience

Load the permitted representation, present a minimal spatial environment, and support one bounded interaction.

The first Experience should explicitly demonstrate at least one requirement that can be satisfied by MyVR alone.

## Phase 4 - Companion

Connect MyVR to AnyApp when the Experience requires or benefits from desktop capabilities, exchange capabilities, synchronize Experience/session identity, heartbeat and reconnect, and delegate one non-frame-critical operation.

AnyApp remains independently usable without MyVR.

## Phase 5 - WebApp + MyVR

Coordinate browser and VR manifestations when an Experience requires browser capabilities, browser computation, distribution, or other WebApp-native capabilities that are not available locally on the VR device.

## Phase 6 - Co-entangled Manifestations

WebApp <-> AnyApp <-> MyVR coordinated session, manifestation handoff, shared bounded Experience events, and capability-aware behavior.

Demonstrate an Experience that intentionally uses all three manifestations simultaneously.

## Phase 7 - Spatial Experience

Observer/camera model, spatial layout, detail horizon, interaction horizon, controller/hand interaction, and appropriate audio/haptic affordances.

## Phase 8 - Device Expansion

Additional runtimes, native/OpenXR implementations where appropriate, WebXR manifestation where appropriate, and device-specific optimization behind adapters.

## Non-goals

MyVR is not Unity infrastructure, a second desktop host, a second composition OS, a repository server, a remote-control shell, a replacement for WebApp, or a requirement for every Experience.


---

<p align="center">
  <a href="https://github.com/TrentBest/TheSingularityWorkshop" aria-label="The Singularity Workshop">
    <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="112" />
  </a><br />
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em>
</p>
