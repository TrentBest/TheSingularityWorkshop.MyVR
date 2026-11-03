![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# TheSingularityWorkshop.MyVR

[![Build and test](https://img.shields.io/github/actions/workflow/status/TrentBest/TheSingularityWorkshop.MyVR/ci.yml?branch=development&style=flat-square&logo=github)](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 01 What

**MyVR is the VR-device manifestation of The Singularity Workshop.**

MyVR is a client for presenting and interacting with Workshop Experiences through VR-capable devices. It provides immersive device capabilities without becoming the Experience runtime, composition engine, repository, or desktop process authority.

## 02 Why

An Experience should retain its identity and meaning when its presentation changes. MyVR provides the VR-device manifestation: immersive presentation, spatial interaction, and device capabilities behind a client-owned boundary.

The canonical artifact identity is **BundleId + Version + ContentHash**. A MyVR session has its own session identity. Device sessions may come and go without redefining the Experience artifact.

## 03 How

The Workshop has complementary manifestations:

- **WebApp** — browser reach, discovery, distribution, and browser APIs.
- **AnyApp** — desktop execution, local services, persistence, artifact caching, and explicitly delegated computation.
- **MyVR** — immersive VR presentation, spatial interaction, and device capabilities.

These clients may participate in the same Experience simultaneously. There is no universal VR → WebApp → AnyApp ladder; the Experience's declared requirements and available capabilities determine the arrangement.

MyVR can operate without AnyApp. When a companion is available, MyVR may delegate explicitly bounded, non-frame-critical work. Frame-critical presentation and immediate interaction must remain locally viable.

The architecture remains engine-neutral at the portable-core boundary. The current Quest 2 candidate is a separate Android ARM64 host using OpenXR and Vulkan; those platform details do not enter MyVR.Core.

## 04 See it in a minute

With the .NET 8 SDK installed, run the hardware-free core tests:

```bash
dotnet test tests/MyVR.Core.Tests/MyVR.Core.Tests.csproj --configuration Release
```

The portable core covers immutable Experience identity, a separate session identifier, capability snapshots, an adapter boundary, and validated lifecycle transitions.

A separate standalone Quest 2 Android host is implemented in [draft PR #16](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/pull/16). Its current candidate head `8c0dd80f9a51ab90fac034c435ed15134cdce949` passed both the [core CI workflow](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41227205130) and [Quest Android build/package workflow](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41227205152). This proves the reported source/build/test and APK packaging checks only. **No physical Quest 2 runtime verification has been recorded.**

The Android host currently creates the OpenXR instance, selects the HMD system, creates the runtime-selected Vulkan device, creates an OpenXR session, and coordinates its lifecycle with Android. It does not yet create a swapchain, submit frames, render spatial content, sample live tracking, or process controller/hand input.

## 05 Documentation and theory

- [MyVR theory learning path](docs/THEORY/README.md) — textbook-style chapters with source mapping, exercises, evidence, and limitations.
- [Architecture](docs/ARCHITECTURE.md) — system position and ownership.
- [Experience Manifestation](docs/EXPERIENCE_MANIFESTATION.md) — immutable identity and creator-owned requirements.
- [VR Boundary](docs/VR_BOUNDARY.md) — semantic-to-device translation and frame-critical work.
- [AnyApp Companion](docs/ANYAPP_COMPANION.md) — bounded desktop companion responsibilities.
- [Security Model](docs/SECURITY_MODEL.md) — identity, authority, messages, and privacy.
- [Implementation Notes](docs/IMPLEMENTATION_NOTES.md) — implemented core and Quest host, validation limits, and gaps.
- [Roadmap](docs/ROADMAP.md) — staged device work and the physical-headset gate.
- [Quest 2 Android host bootstrap guide](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/quest2-android-host-bootstrap/docs/QUEST2_ANDROID_HOST_BOOTSTRAP.md) — developer setup and first-device verification checklist.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
