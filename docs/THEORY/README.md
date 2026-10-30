<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# MyVR Theory: A Learning Path

MyVR is a client that gives an Experience an immersive manifestation. This learning path teaches the problem from the user's point of view, develops the device/client model, follows a session through its lifecycle, and then traces the current implementation.

These chapters explain **MyVR-owned theory**. They do not replace the theory or contracts owned by neighboring Workshop repositories.

## 01 What — the problem MyVR solves

Start with [An Experience, Many Manifestations](01-experience-and-manifestation.md). Learn why the same Experience must retain its identity and meaning when its presentation moves between a browser, desktop, and VR device.

## 02 Why — a client boundary, not another runtime

Continue with [The VR Client and Its Device Boundary](02-the-vr-client-and-device-boundary.md). Understand why device capabilities are constraints on a manifestation, why the headset is not the Experience, and why frame-critical work stays local.

## 03 How — the session model

Read [Sessions, Identity, and Lifecycle](03-sessions-identity-and-lifecycle.md). Follow the difference between immutable artifact identity and one live device session, then reason about discovery, connection, synchronization, immersion, suspension, and recovery.

## 04 See it in a minute — one bounded flow

Use [A Headless Session Walkthrough](04-headless-session-walkthrough.md) for a small end-to-end example grounded in the current headless core candidate. It is source-checked against the candidate implementation and tests; it is not a headset demo.

## 05 Choose your next step

- [Architecture](../ARCHITECTURE.md) — the Workshop-wide manifestation arrangement and ownership boundaries.
- [Experience Manifestation](../EXPERIENCE_MANIFESTATION.md) — creator-owned requirements and simultaneous clients.
- [VR Boundary](../VR_BOUNDARY.md) — semantic-to-device translation and the frame boundary.
- [AnyApp Companion](../ANYAPP_COMPANION.md) — bounded desktop companion responsibilities.
- [Security Model](../SECURITY_MODEL.md) — identity, authority, message, and privacy boundaries.
- [Implementation Notes](../IMPLEMENTATION_NOTES.md) — current implementation inventory and gaps.
- [Roadmap](../ROADMAP.md) — staged device work.

## How to read and reuse these chapters

The chapters are modular. An instructor can reorder them, assign one chapter independently, or use the review questions and exercises as seminar prompts. Each chapter distinguishes conceptual architecture from implemented behavior and links to the exact source/tests used for implementation claims.

## Evidence and status

The executable core described here is proposed in [PR #12](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/pull/12), candidate head `d0f365f74b9e567151a6d5570d99fb62d2391b75`. That candidate contains the `.NET 8` headless core and deterministic tests. Its GitHub Actions runs report a successful Release build and 13 passing tests. The PR is still a draft and is not merged into `development`.

- **Source-checked:** public types and tests linked from the candidate branch.
- **CI-verified:** the candidate SHA above was built and tested by GitHub Actions.
- **Conceptual:** multi-manifestation coordination, semantic input translation, real device discovery, spatial presentation, and runtime integration remain architectural direction unless explicitly implemented elsewhere.
- **Not verified:** no headset, controller, hand-tracking system, WebXR session, OpenXR runtime, or vendor runtime was exercised.

This theory path must be updated when the implementation changes. A claim is not upgraded from conceptual to implemented merely because it appears in a diagram or roadmap.

---

<p align="center">
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em><br />
  <a href="https://github.com/TrentBest">GitHub</a> · <a href="https://www.patreon.com/TheSingularityWorkshop">Support the Workshop</a>
</p>
