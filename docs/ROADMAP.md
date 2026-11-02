![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Roadmap

## Phase 0 - Theory

Define MyVR as the VR manifestation/client and lock boundaries with Experience, AnyApp, WebApp, GUI, and FSM_COS.

## Phase 1 - Device Skeleton

Establish the client project, detect VR/runtime capabilities, expose a minimal capability model, create a testable session state machine, and establish a semantic-to-VR boundary.

## Phase 2 - Creator-defined Experience requirements

Resolve an immutable published Experience identity and its declared capability requirements.

Determine whether the current VR device can satisfy the required immersive and interaction capabilities locally, and identify optional/delegable work that may be supplied by WebApp and/or AnyApp.

A requirement failure is an Experience capability mismatch, not a reason to redefine MyVR's architectural role.

## Phase 3 - First Experience

Load the permitted representation, present a minimal spatial environment, and support one bounded interaction.

The first Experience should explicitly demonstrate at least one requirement that can be satisfied by MyVR alone.

## Phase 4 - Companion

Connect MyVR to AnyApp when the Experience requires or benefits from desktop capabilities, exchange capabilities, synchronize Experience/session identity, heartbeat and reconnect, and delegate one non-frame-critical operation.

AnyApp remains independently usable without MyVR.

## Phase 5 - WebApp + MyVR

Coordinate browser and VR manifestations when an Experience requires browser capabilities, browser computation, distribution, or other WebApp-native capabilities that are not available locally on the VR device.

## Phase 6 - Co-entangled Manifestations

WebApp <-> AnyApp <-> MyVR coordinated session, manifestation handoff, shared bounded Experience events, and capability-aware behavior.

Demonstrate an Experience that intentionally uses all three manifestations simultaneously.

## Phase 7 - Spatial Experience

Observer/camera model, spatial layout, detail horizon, interaction horizon, controller/hand interaction, and appropriate audio/haptic affordances.

## Phase 8 - Device Expansion

Additional runtimes, native/OpenXR implementations where appropriate, WebXR manifestation where appropriate, and device-specific optimization behind adapters.

## Non-goals

MyVR is not Unity infrastructure, a second desktop host, a second composition OS, a repository server, a remote-control shell, a replacement for WebApp, or a requirement for every Experience.

## Play-space boundary direction

The default Experience boundary is an automatically generated sphere scaled from the Experience's recommended clearance radius. MyVR owns its device-local placement and presentation; the Experience owns the recommendation through the canonical Experiences contract. A player-drawn boundary is not part of the intended workflow.

A later optional custom mode may use prescanned play-area geometry so an Experience can adapt its world to the available space. This does not replace the headset platform's system Guardian. See [issue #17](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/issues/17) for staged acceptance criteria.

This is architectural direction only. The current host has no spatial frame pipeline or live play-space tracking, so neither sphere visualization nor scanned-area capture is implemented.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
