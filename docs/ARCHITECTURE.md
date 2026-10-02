# MyVR Architecture

## Purpose

**MyVR is the VR-device manifestation of The Singularity Workshop.**

It is a client for presenting and interacting with Workshop Experiences through VR-capable devices. It is not a second FSM_COS, a replacement for AnyApp, or the canonical owner of Experience state.

## System position

```text
Experience
    |
immutable identity
    |
composition / runtime
    |
+-----------+-----------+
|                       |
AnyApp                  MyVR
Desktop companion       VR client
local computation       VR presentation
persistence             spatial input
artifact cache          immersive session
```

The WebApp is another manifestation of the same Experience.

## Responsibilities

MyVR owns VR-device discovery, capability reporting, immersive-session lifecycle, headset/controller/hand input, VR-specific spatial presentation, VR interaction, device pose/input state, bounded Experience interactions, permitted companion communication, and graceful degradation when services are unavailable.

MyVR does not own Experience composition, MicroBundle arbitration, authoritative artifact identity, arbitrary assembly loading, desktop process control, the repository, browser UI, a duplicate GUI platform abstraction, or Unity-specific infrastructure.

## Device-first, Experience-first

The client begins with device capability because VR hardware determines what can actually be presented. The Experience remains the semantic source.

```text
device capability
      |
MyVR manifestation capability
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

This is a companion relationship, not a requirement that MyVR become a thin remote display. Frame-critical work must remain locally viable on the VR device.

## Relationship to WebApp

WebApp provides browser reach, discovery, distribution, and browser APIs. MyVR provides direct VR-device interaction. The same Experience may be entered through WebApp, AnyApp, MyVR, or a coordinated combination.

## Relationship to GUI

MyVR may consume semantic GUI/interaction concepts where useful, but GUI Core must not become a VR SDK. Platform-specific VR types belong behind MyVR's device boundary.

## Guiding principle

> **MyVR is a window into an Experience, not the Experience itself.**
