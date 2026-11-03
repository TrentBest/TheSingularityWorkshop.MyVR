<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# MyVR Theory: A Learning Path

MyVR is a VR-device host and manifestation of a Workshop Experience. This learning path starts with the identity of an Experience, derives the device/client boundary, explains session lifecycle, then follows the portable core, the Quest 2 host, and the shared execution architecture.

These chapters explain **MyVR-owned theory**. They do not replace the theory or contracts owned by neighboring Workshop repositories.

## 01 What — the problem MyVR solves

Start with [An Experience, Many Manifestations](01-experience-and-manifestation.md). Learn why the same Experience must retain its identity and meaning when its presentation moves between a browser, desktop, and VR device.

## 02 Why — a client boundary, not another runtime

Continue with [The VR Client and Its Device Boundary](02-the-vr-client-and-device-boundary.md). Understand why device capabilities constrain a manifestation, why the headset is not the Experience, and why frame-critical work stays local.

## 03 How — the session model

Read [Sessions, Identity, and Lifecycle](03-sessions-identity-and-lifecycle.md). Follow the difference between immutable artifact identity and one live device session, then reason about discovery, connection, synchronization, immersion, suspension, and recovery.

## 04 See it in a minute — one bounded flow

Use [A Headless Session Walkthrough](04-headless-session-walkthrough.md) to run and test the portable .NET core. It is deliberately hardware-free and does not claim to prove a device runtime.

## 05 Device host — from Android Activity to OpenXR session

Read [The Quest 2 Android Host Bootstrap](05-quest2-android-host-bootstrap.md). Trace loader initialization, extension checks, HMD system selection, runtime-selected Vulkan device creation, OpenXR session creation, Android lifecycle coordination, and the physical-device evidence gate.

## 06 Ecosystem integration — a peer host of Foundry

Read [MyVR as a Peer Host of the Shared Runtime](06-myvr-as-a-peer-host.md). Understand the intended manifest → FSM_COS → RuntimeAssembly → FSM_Foundry → MyVR presentation flow, and why MyVR must not duplicate composition or workflow execution.

## Reuse and further reading

- [Architecture](../ARCHITECTURE.md) — the Workshop-wide manifestation arrangement and ownership boundaries.
- [Experience Manifestation](../EXPERIENCE_MANIFESTATION.md) — creator-owned requirements and simultaneous clients.
- [VR Boundary](../VR_BOUNDARY.md) — semantic-to-device translation and the frame boundary.
- [AnyApp Companion](../ANYAPP_COMPANION.md) — bounded desktop companion responsibilities.
- [Security Model](../SECURITY_MODEL.md) — identity, authority, message, and privacy boundaries.
- [Implementation Notes](../IMPLEMENTATION_NOTES.md) — current implementation inventory and gaps.
- [Roadmap](../ROADMAP.md) — staged device work.

## How to read and reuse these chapters

The chapters are modular. An instructor can reorder them, assign one chapter independently, or use the review questions and exercises as seminar prompts. Each chapter distinguishes conceptual architecture from implemented behavior and links to the source, tests, CI evidence, and open work that support its claims.

## Evidence and status

The current repository has two separate candidate layers:

- **Portable core:** .NET 8 identity, capability snapshots, adapter boundary, and deterministic session lifecycle tests.
- **Quest host:** a separate .NET 10 Android ARM64 application with OpenXR/Vulkan session bootstrap and Android lifecycle handling.

The Quest host candidate is [draft PR #16](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/pull/16), head `8c0dd80f9a51ab90fac034c435ed15134cdce949`. Its [core CI run](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41227205130) and [Android build/package run](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41227205152) succeeded for that exact head. The PR remains draft and unmerged; its branch ancestry still requires reconciliation with the declared OpenXR-spike base before it can be treated as integration-ready.

- **Source-implemented:** portable core contracts and the Android/OpenXR/Vulkan bootstrap described in Chapter 5.
- **CI-verified:** build/test and signed ARM64 APK packaging checks for the exact PR #16 head.
- **Conceptual / not integrated:** the shared manifest → FSM_COS → RuntimeAssembly → FSM_Foundry → MyVR host handoff described in Chapter 6.
- **Not hardware-verified:** no physical Quest 2 runtime initialization, lifecycle, rendering, tracking, or input evidence has been recorded.

A diagram, passing headless test, or successful APK package does not upgrade a conceptual or hardware-unverified capability to implemented or verified.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
