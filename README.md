![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# TheSingularityWorkshop.MyVR

[![Build and test](https://img.shields.io/github/actions/workflow/status/TrentBest/TheSingularityWorkshop.MyVR/ci.yml?branch=development&style=flat-square&logo=github)](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 01 What

**MyVR is the VR-device manifestation of The Singularity Workshop.**

MyVR is a client specifically for VR-capable devices. It presents and interacts with Experiences without becoming the Experience runtime itself.

## 02 Why

An Experience should keep its identity and meaning when its presentation changes. MyVR gives an Experience an immersive manifestation without turning the headset, renderer, or device runtime into the owner of Experience semantics or composition.

## 03 How

![MyVR capability arrangement](docs/assets/myvr-capability-arrangement.svg)

MyVR contributes immersive capabilities; the Experience decides whether those capabilities participate alone or alongside WebApp and AnyApp.

### Manifestations and identity

The Workshop separates Experience identity from manifestation:

- **WebApp** — browser reach, discovery, distribution, and browser APIs
- **AnyApp** — desktop execution, local services, persistence, artifact caching, and delegated computation
- **MyVR** — immersive VR presentation, spatial interaction, and device capabilities

All three may participate in the same Experience without sharing the same renderer or runtime implementation.

The canonical artifact identity is **BundleId + Version + ContentHash**. A VR session has its own session identity.

### Companion model

MyVR can operate without AnyApp. When a companion is available, MyVR may delegate explicitly bounded, non-frame-critical work to it. The companion boundary is not a remote shell.

### Technology boundary

The architecture is intentionally engine-neutral. WebXR, native VR APIs, OpenXR, or device-specific adapters may be used behind the MyVR boundary as implementation choices.

### Guiding principle

> **MyVR is a window into an Experience, not the Experience itself.**

### Implementation status

The first executable slice is a **headless, engine-neutral .NET 8 core**:

- implemented: immutable Experience identity, a separate MyVR session identity, immutable capability snapshots, a device/runtime adapter boundary, and validated session lifecycle transitions;
- simulated: capability reporting through a deterministic fake adapter in the test suite;
- implemented but not yet verified on physical hardware: a standalone Quest 2 Android/OpenXR host bootstrap, including loader initialization, HMD discovery, runtime-selected Vulkan device creation, OpenXR session creation, and Android/OpenXR lifecycle handling;
- not yet implemented: swapchain creation, frame submission, spatial rendering, live tracking, or controller/hand input;
- not yet verified: runtime initialization and lifecycle behavior on a physical Quest headset.

The core has no FSM_COS, FSM_UserIO, GUI, AnyApp, or VR SDK dependency. MyVR does not compose Experiences or own workflow execution.

## 04 See it in a minute

Install the .NET 8 SDK, then run the deterministic, hardware-free tests:

```bash
dotnet test tests/MyVR.Core.Tests/MyVR.Core.Tests.csproj --configuration Release
```

This verifies the portable lifecycle and adapter contract; it does not verify a headset or VR runtime.

## 05 Documentation and theory

- [Architecture](docs/ARCHITECTURE.md)
- [Quest 2 Android host bootstrap and first-device test](docs/QUEST2_ANDROID_HOST_BOOTSTRAP.md)
- [Experience Manifestation](docs/EXPERIENCE_MANIFESTATION.md)
- [VR Boundary](docs/VR_BOUNDARY.md)
- [AnyApp Companion](docs/ANYAPP_COMPANION.md)
- [Security Model](docs/SECURITY_MODEL.md)
- [Roadmap](docs/ROADMAP.md)
- [Implementation Notes](docs/IMPLEMENTATION_NOTES.md)

The implementation follows the boundaries above. See [the roadmap](docs/ROADMAP.md) for the implemented core and remaining device work.

---

<p align="center">
  <a href="https://github.com/TrentBest/TheSingularityWorkshop" aria-label="The Singularity Workshop living avatar">
    <img src="https://raw.githubusercontent.com/TrentBest/WebPage/development/TheSingularityWorkshop/wwwroot/Images/TheSingularityWorkshopLivingAvatar.svg" alt="The Singularity Workshop living avatar — a digital vortex converging on its central eye" width="112" />
  </a><br />
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em><br />
  <a href="https://github.com/TrentBest">GitHub</a> · <a href="https://www.patreon.com/TheSingularityWorkshop">Support the Workshop</a>
</p>
