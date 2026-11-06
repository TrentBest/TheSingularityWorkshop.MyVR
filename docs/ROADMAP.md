[![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)](https://github.com/TrentBest/TheSingularityWorkshop)

# Roadmap

This roadmap describes ordered work, not proof of implementation. Use [issue #26](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/issues/26) as the active shared-kernel and VR-domain coordination record.

## Phase 0 — Architecture and shared host contract

Define MyVR as a peer client host consuming FSM_Foundry. Confirm the concrete host contract with FSM_Foundry #20 and AnyApp #45. Keep FSM_Foundry consumer-agnostic; keep Experience semantics and artifact identity upstream; keep optional ServerApp remote-compute policy outside Foundry.

## Phase 1 — Portable device/session boundary

Establish the device-independent MyVR core, capability model, immutable Experience/session identity boundaries, and deterministic session lifecycle tests. Keep the core free of headset SDK dependencies and shared-kernel implementation details.

## Phase 2 — Quest/Android host bootstrap

Establish Android application lifecycle, OpenXR loader/instance/system initialization, runtime-selected graphics-device integration, and OpenXR session lifecycle. Record CI/package evidence separately from runtime and physical-device evidence.

**Physical-device gate:** verify the bootstrap on the target Quest headset before proceeding to swapchain, frame loop, frame submission, or rendering work. A successful build/package is not a substitute for this gate.

## Phase 3 — First standalone Experience

The first installed product is the Workshop Moniker Experience. Resolve its canonical published identity and requirements, negotiate the local VR capabilities, enter its permitted representation, and support one bounded interaction. It must work locally without AnyApp or ServerApp when its requirements permit.

## Phase 4 — Frame and spatial pipeline

After the physical bootstrap gate passes, implement frame timing, swapchain lifecycle, submission/recovery, tracking validity, coordinate frames, play-space boundaries, spatial presentation, and controller/hand interaction. Keep all headset-specific APIs behind MyVR adapters.

## Phase 5 — Optional collaborators

Integrate AnyApp as a peer desktop host/optional companion and ServerApp as an optional remote computation host. Exchange explicit capability offers and delegate only bounded, user-authorized, non-frame-critical work. Add WebApp coordination when an Experience declares browser-native requirements.

## Phase 6 — Co-entangled manifestations

Demonstrate an Experience coordinating WebApp, AnyApp, and MyVR, with optional ServerApp assistance where appropriate. Preserve a single canonical Experience identity and bounded semantic events across manifestations.

## Phase 7 — Device expansion and optimization

Add additional runtimes and devices behind adapters. Optimize the headset hot path for predictable frame times, allocation pressure, memory traffic, native interop, GPU synchronization, and latency without weakening the portability of shared computational foundations.

## Non-goals

MyVR is not Unity infrastructure, a second desktop host, a second composition OS, a repository server, a remote-control shell, a replacement for WebApp, or a requirement for every Experience.

---

<p align="center">
  <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="180" />
  <br />
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em>
</p>
