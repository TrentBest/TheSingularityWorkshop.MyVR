# TheSingularityWorkshop.MyVR

**MyVR is the VR-device manifestation of The Singularity Workshop.**

MyVR is a client specifically for VR-capable devices. It presents and interacts with Experiences without becoming the Experience runtime itself.

## Architecture

![MyVR capability arrangement](docs/assets/myvr-capability-arrangement.svg)

MyVR contributes immersive capabilities; the Experience decides whether those capabilities participate alone or alongside WebApp and AnyApp.

## Architecture

The Workshop separates Experience identity from manifestation:

- **WebApp** — browser reach, discovery, distribution, and browser APIs
- **AnyApp** — desktop execution, local services, persistence, artifact caching, and delegated computation
- **MyVR** — immersive VR presentation, spatial interaction, and device capabilities

All three may participate in the same Experience without sharing the same renderer or runtime implementation.

The canonical artifact identity is **BundleId + Version + ContentHash**. A VR session has its own session identity.

## Companion model

MyVR can operate without AnyApp. When a companion is available, MyVR may delegate explicitly bounded, non-frame-critical work to it. The companion boundary is not a remote shell.

## Technology boundary

The architecture is intentionally engine-neutral. WebXR, native VR APIs, OpenXR, or device-specific adapters may be used behind the MyVR boundary as implementation choices.

## Guiding principle

> **MyVR is a window into an Experience, not the Experience itself.**

## Theory

- [Architecture](docs/ARCHITECTURE.md)
- [Experience Manifestation](docs/EXPERIENCE_MANIFESTATION.md)
- [VR Boundary](docs/VR_BOUNDARY.md)
- [AnyApp Companion](docs/ANYAPP_COMPANION.md)
- [Security Model](docs/SECURITY_MODEL.md)
- [Roadmap](docs/ROADMAP.md)
- [Implementation Notes](docs/IMPLEMENTATION_NOTES.md)

Implementation begins after these boundaries are reviewed against the existing Workshop architecture.
