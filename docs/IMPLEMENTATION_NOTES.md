# Implementation Notes

The first executable MyVR core now exists. It is a portable, headless .NET 8 library; it is not yet a deployable headset application.

## Implemented core boundary

The core currently contains:

- immutable Experience artifact identity: BundleId + Version + ContentHash;
- a separate MyVR session GUID and explicit immersive-session lifecycle;
- immutable device capability snapshots with adapter-owned capability names;
- IDeviceRuntimeAdapter, the narrow boundary for future device/runtime integrations;
- deterministic tests using a fake adapter, without a headset or companion.

The core has no compile-time dependency on FSM_COS, FSM_UserIO, GUI, AnyApp, a VR SDK, or a repository client.

## Dependency direction

```text
MyVR.Core
  |-- Experience artifact identity (local immutable value contract)
  |-- MyVR session lifecycle (local host concern)
  `-- IDeviceRuntimeAdapter
          |
          v
    future platform/runtime adapter
          |
          v
    headset / browser / native VR runtime
```

A future semantic-input adapter may translate device observations into FSM_UserIO semantic values. Keep that integration outside the device-independent core until the exact package/version/source and dependency direction pass the required CiAi dependency-policy gate (MyVR issue #6 / CiAi issue #21). A semantic intent is not authorization or execution permission.

## First executable slice

Implemented:

1. A MyVR session can be created for a validated immutable Experience identity.
2. Device capabilities can be represented without importing a vendor/runtime type.
3. A runtime adapter can report capabilities through a narrow asynchronous interface.
4. The session follows an explicit lifecycle and rejects invalid transitions.
5. Suspension/resume, disconnect, and reconnect preserve Experience and session identity.
6. Deterministic tests exercise the core and a fake adapter without hardware.

Still outstanding:

1. A concrete device/runtime adapter and an actual deployment target.
2. Runtime capability detection on a real headset.
3. Resolution and verification of a published Experience artifact by its immutable identity.
4. A minimal permitted Experience representation.
5. One bounded semantic interaction and Experience event.
6. Companion/network lifecycle tests against a real bounded protocol.

## Testing

The test project covers identity validation/equality, capability reporting, the fake runtime adapter, valid/invalid session transitions, suspend/resume, disconnect, and reconnect.

Platform/device tests must remain separate from deterministic core tests. A green headless test run is not headset verification.

## Technology rule

The portable core targets .NET 8, matching the Workshop's current cross-platform library baseline. This does not select a headset deployment target. Do not select a VR engine or runtime until device targets, deployment constraints, licensing, and required APIs are understood.
