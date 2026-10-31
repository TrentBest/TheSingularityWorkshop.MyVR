![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)

# TheSingularityWorkshop.MyVR

**MyVR is the VR-device manifestation of The Singularity Workshop.**

MyVR is a client specifically for VR-capable devices. It presents and interacts with Experiences without becoming the Experience runtime itself.

## Explore the business vision

The [MyVR GitHub Pages site](https://trentbest.github.io/TheSingularityWorkshop.MyVR/) presents the broader vision: how the Workshop's technology could scale from an individual venue into a partner-led distribution network.

It also includes our [competitive landscape research](https://trentbest.github.io/TheSingularityWorkshop.MyVR/competition.html) and the developing business and pricing model. These pages are an invitation to examine the opportunity and help us test the assumptions—not a claim that the proposed distributor portal or commercial capabilities are already implemented.

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

---

<p align="center">
  <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="180" /><br />
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em><br />
  <a href="https://github.com/TrentBest">GitHub</a> · <a href="https://www.patreon.com/TheSingularityWorkshop">Support the Workshop</a>
</p>
