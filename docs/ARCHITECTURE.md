# MyVR Architecture

## Purpose

**MyVR is the VR-device manifestation of The Singularity Workshop.**

It is a client for presenting and interacting with Workshop Experiences through VR-capable devices. It is not a second FSM_COS, a replacement for AnyApp, or the canonical owner of Experience state.

## System position

```
Experience
    |
creator-declared requirements
    |
composition / runtime
    |
+-----------+-----------+-----------+
|           |           |           |
WebApp      AnyApp      MyVR
browser     desktop     VR client
compute     local       immersive
APIs        compute     presentation
```

The WebApp, AnyApp, and MyVR manifestations have different capabilities. The Experience creator decides which capabilities are required, preferred, optional, or delegable.

## Responsibilities

MyVR owns VR-device discovery, capability reporting, immersive-session lifecycle, headset/controller/hand input, VR-specific spatial presentation, VR interaction, device pose/input state, bounded Experience interactions, permitted companion communication, and graceful degradation when services are unavailable.

MyVR does not own Experience composition, MicroBundle arbitration, authoritative artifact identity, arbitrary assembly loading, desktop process control, the repository, browser UI, a duplicate GUI platform abstraction, or Unity-specific infrastructure.

## Device capability is a constraint, not an Experience definition

An Experience can exceed the capabilities of the VR device.

That does not make MyVR a failed runtime. It means the Experience's declared requirements must be evaluated against the capabilities available across its manifestations.

For example, an Experience may require:

- MyVR for immersive presentation and immediate interaction;
- WebApp for browser computation or browser-native APIs;
- AnyApp for desktop CPU/GPU computation, storage, persistence, or artifact services;
- or all three simultaneously.

There is therefore no universal "VR -> WebApp -> AnyApp" upgrade ladder. The execution arrangement is selected from the creator's declared requirements and the capabilities actually available.

## Device-first, Experience-first

The client begins with device capability because VR hardware determines what can actually be presented. The Experience remains the semantic source.

```
device capability
      |
MyVR manifestation capability
      |
Experience requirements
      |
compatible execution arrangement
      |
Experience semantic surface
      |
VR representation
      |
user interaction
      |
bounded Experience event
```

A VR device should never need to understand FSM_COS or MicroBundleRepository implementation details.

## Relationship to AnyApp

AnyApp and MyVR are complementary manifestations. AnyApp can provide local services and computation that are inappropriate for a headset. MyVR provides immersive representation and device interaction.

AnyApp is **not dependent on MyVR**. It can run an Experience independently, work with WebApp without VR, or participate with MyVR when an Experience explicitly benefits from the combination.

Likewise, MyVR can operate without AnyApp when the Experience requirements and device capabilities permit it.

This is a companion relationship, not a requirement that MyVR become a thin remote display. Frame-critical work must remain locally viable on the VR device.

## Relationship to WebApp

WebApp provides browser reach, discovery, distribution, browser APIs, and browser-side computation where available. MyVR provides direct VR-device interaction.

An Experience may use WebApp + MyVR when the browser supplies capabilities the VR device lacks, or it may use WebApp + AnyApp + MyVR when the Experience requires the full coordinated environment.

## Relationship to GUI

MyVR may consume semantic GUI/interaction concepts where useful, but GUI Core must not become a VR SDK. Platform-specific VR types belong behind MyVR's device boundary.

## Guiding principle

> **MyVR is a window into an Experience, not the Experience itself.**
