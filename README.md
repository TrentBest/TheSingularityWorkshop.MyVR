![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# TheSingularityWorkshop.MyVR

**MyVR is the VR-device manifestation of The Singularity Workshop.**

MyVR is a client for presenting and interacting with Workshop Experiences through VR-capable devices. It provides immersive device capabilities without becoming the Experience runtime, composition engine, repository, or desktop process authority.

## 01 What

An Experience should retain its identity and meaning when its presentation changes. MyVR provides the VR-device manifestation: immersive presentation, spatial interaction, and device capabilities behind a client-owned boundary.

![MyVR capability arrangement](docs/assets/myvr-capability-arrangement.svg)

## 02 Why

A headset knows about its display, tracking, and input devices. It does not own the meaning of the Experience or gain authority over the desktop merely because it is connected. MyVR keeps device-specific concerns separate from portable Experience identity and semantics.

The canonical artifact identity is **BundleId + Version + ContentHash**. A MyVR session has its own session identity.

## 03 How

The Workshop has complementary manifestations:

- **WebApp** — browser reach, discovery, distribution, and browser APIs.
- **AnyApp** — desktop execution, local services, persistence, artifact caching, and explicitly delegated computation.
- **MyVR** — immersive VR presentation, spatial interaction, and device capabilities.

These clients may participate in the same Experience simultaneously. There is no universal VR → WebApp → AnyApp ladder; the Experience's declared requirements and available capabilities determine the arrangement.

MyVR can operate without AnyApp. When a companion is available, MyVR may delegate explicitly bounded, non-frame-critical work. Frame-critical presentation and immediate interaction must remain locally viable.

The architecture remains engine-neutral. WebXR, native VR APIs, OpenXR, and device-specific adapters are possible implementation routes, not selected dependencies.

## 04 See it in a minute

The current headless core candidate demonstrates immutable Experience identity, a separate session identifier, adapter-reported capability snapshots, and validated lifecycle transitions.

With the .NET 8 SDK installed, run:

```bash
dotnet test tests/MyVR.Core.Tests/MyVR.Core.Tests.csproj --configuration Release
```

The implementation and its 13 deterministic tests are proposed in [draft PR #12](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/pull/12). The tests do not verify a headset, concrete runtime, spatial rendering, or live device input.

## 05 Choose your next step

- [MyVR theory learning path](docs/THEORY/README.md) — modular textbook-style chapters with source mapping, exercises, evidence, and limitations.
- [Architecture](docs/ARCHITECTURE.md) — system position and ownership.
- [Experience Manifestation](docs/EXPERIENCE_MANIFESTATION.md) — immutable identity and creator-owned requirements.
- [VR Boundary](docs/VR_BOUNDARY.md) — semantic-to-device translation and frame-critical work.
- [AnyApp Companion](docs/ANYAPP_COMPANION.md) — bounded desktop companion responsibilities.
- [Security Model](docs/SECURITY_MODEL.md) — identity, authority, messages, and privacy.
- [Roadmap](docs/ROADMAP.md) — implementation stages.
- [Implementation Notes](docs/IMPLEMENTATION_NOTES.md) — implemented core and remaining device work.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
