[![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)](https://github.com/TrentBest/TheSingularityWorkshop)

# TheSingularityWorkshop.MyVR

**MyVR is the VR-device manifestation of The Singularity Workshop.**

MyVR is a client for presenting and interacting with Experiences through VR-capable devices. It does not own Experience semantics or become a second composition/runtime kernel.

## Architecture

![MyVR capability arrangement](docs/assets/myvr-capability-arrangement.svg)

**FSM_Foundry** is the shared execution/loading kernel. It owns common orchestration and execution lifecycle without knowing which host consumes it.

- **WebApp** — browser manifestation, discovery, distribution, and browser-native capabilities.
- **AnyApp** — desktop manifestation, local services, persistence, artifact caching, and local delegated computation.
- **MyVR** — VR-device manifestation, immersive presentation, spatial interaction, and device capabilities.
- **ServerApp** — optional remote computation host, discovered through explicit capability offers; it is not another client manifestation.

AnyApp, WebApp, and MyVR are peer client hosts. Each consumes shared kernel contracts and supplies its own runtime-specific composition, adapters, presentation, and lifecycle. ServerApp may provide bounded remote computation when explicitly offered and authorized. FSM_Foundry does not decide remote placement policy.

The canonical artifact identity is **BundleId + Version + ContentHash**. A MyVR session has its own session identity. Experience semantics and artifact identity remain with their canonical upstream contracts; MyVR owns only their VR manifestation and device-specific behavior.

## Companion and standalone operation

MyVR must operate independently when the Experience requirements can be satisfied locally. AnyApp and ServerApp are optional collaborators, not mandatory dependencies. Delegation must be explicit, bounded, user-authorized, and non-frame-critical. No companion boundary grants remote shell or arbitrary process authority.

## VR engineering boundary

MyVR owns device/runtime discovery, capability negotiation, Android/Quest and OpenXR lifecycle, frame timing and submission, tracking/input, spatial coordinate and play-space handling, VR presentation, haptics, and spatial audio. Those responsibilities must not migrate into FSM_Foundry or upstream portable contracts.

The architecture is engine-neutral. Runtime-specific APIs belong behind MyVR-owned adapters. Keep frame-critical work local to the headset; optimize allocations, memory traffic, scheduling, native interop, GPU synchronization, and latency without contaminating the portable computational substrate with VR assumptions.

## Evidence discipline

A roadmap item is not an implemented capability. Track source implementation, automated tests, CI/package evidence, runtime integration evidence, and physical-headset verification separately. Android packaging or a successful CI build does not prove that a Quest device can initialize or run the host.

The current host/session work and its physical-device gate are tracked in [issue #26](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/issues/26). Do not add swapchain, frame-loop, rendering, or live-tracking work ahead of the documented physical bootstrap gate.

## Theory

- [Architecture](docs/ARCHITECTURE.md)
- [Experience Manifestation](docs/EXPERIENCE_MANIFESTATION.md)
- [VR Boundary](docs/VR_BOUNDARY.md)
- [AnyApp Companion](docs/ANYAPP_COMPANION.md)
- [Security Model](docs/SECURITY_MODEL.md)
- [Roadmap](docs/ROADMAP.md)
- [Implementation Notes](docs/IMPLEMENTATION_NOTES.md)


---

<p align="center">
  <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="180" />
  <br />
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em>
</p>
