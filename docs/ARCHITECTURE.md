[![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)](https://github.com/TrentBest/TheSingularityWorkshop)

# MyVR Architecture

## Purpose

![MyVR capability arrangement](assets/myvr-capability-arrangement.svg)

The visual emphasizes that VR is one manifestation in a capability arrangement, not a mandatory rung in a platform ladder.

**MyVR is the VR-device manifestation of The Singularity Workshop.** It presents and interacts with Workshop Experiences through VR-capable devices. It is not a second FSM_COS, a replacement for AnyApp, or the canonical owner of Experience state.

## Shared kernel and host responsibilities

**FSM_Foundry is the shared execution/loading kernel.** It owns common execution lifecycle and orchestration. It must remain consumer-agnostic: no references, special cases, or host-specific policy for AnyApp, WebApp, ServerApp, or MyVR.

AnyApp, WebApp, and MyVR are peer client/application hosts. Each consumes the shared kernel and provides its own runtime-specific host composition, adapters, presentation, and lifecycle facilities. **ServerApp is different:** it is an optional remote computation host, not a fourth client manifestation. Clients may discover it through explicit capability/offer contracts and delegate only bounded, non-frame-critical workloads with user consent. FSM_Foundry does not own remote placement policy.

```text
                    FSM_Foundry
                shared host kernel
                       |
          +------------+------------+
          |            |            |
        AnyApp       WebApp        MyVR
        desktop      browser       VR device
        client       client        client
          \            |            /
           \           |           /
            +---- optional ServerApp
                  remote computation
```

The diagram is conceptual. Exact project/package/API references must be source-verified before an integration is described as implemented.

## Experience and artifact ownership

Experience semantics, published artifact identity, and canonical contracts remain owned by their upstream domains. The canonical artifact identity is **BundleId + Version + ContentHash**. MyVR has a separate session identity and owns the VR manifestation and device-specific behavior.

A VR device should not need to understand FSM_COS, MicroBundleRepository, or Foundry implementation details. MyVR consumes the appropriate shared host contract; it does not reproduce artifact resolution, composition, arbitration, scheduling, or the common execution lifecycle.

## MyVR-owned VR domain

MyVR owns device/runtime discovery and capability negotiation; Android/Quest application lifecycle; OpenXR session lifecycle; frame timing, swapchain, submission, and recovery; headset pose and controller/hand input; tracking validity and input mapping; spatial coordinate frames and play-space boundaries; VR scene/presentation and interaction affordances; haptics and spatial audio; and device-specific recovery.

These responsibilities remain behind MyVR-owned adapters. Portable Experience and shared-kernel contracts must not acquire VR SDK types.

## Capability arrangement

The Experience creator decides which capabilities are required, preferred, optional, or delegable. The WebApp, AnyApp, and MyVR manifestations have different capabilities; ServerApp may offer bounded remote computation where appropriate.

An Experience may require MyVR for immersive presentation, WebApp for browser-native APIs, AnyApp for desktop computation or persistence, or a combination. There is no universal "VR -> WebApp -> AnyApp" upgrade ladder. The arrangement is selected from declared Experience requirements and capabilities actually available.

A requirement failure is a capability mismatch, not a reason to redefine MyVR's architectural role.

## Standalone and companion operation

MyVR must run without AnyApp when the Experience requirements and local device capabilities permit it. AnyApp is an optional peer host and companion, not a prerequisite. ServerApp is also optional. Delegated work must be explicit, bounded, user-authorized, and non-frame-critical; headset frame-critical work must remain locally viable.

## Relationship to GUI

MyVR may consume semantic GUI/interaction concepts where useful, but GUI Core must not become a VR SDK. Platform-specific VR types belong behind MyVR's device boundary.

## Performance principle

The portable computational substrate and the headset hot path have different constraints. Preserve broad framework/platform interoperability in shared foundations; optimize the VR path independently for frame-time predictability, allocation pressure, memory traffic, native interop, GPU synchronization, and latency. Do not move VR-specific assumptions into FSM_API or FSM_Foundry.

## Guiding principle

> **MyVR is a window into an Experience, not the Experience itself.**

---

<p align="center">
  <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="180" />
  <br />
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em>
</p>
