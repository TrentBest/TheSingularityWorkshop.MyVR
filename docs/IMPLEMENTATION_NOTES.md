# Implementation Notes

Implementation begins after the boundaries in the theory are stable.

## Dependency direction

```text
MyVR
 |-- Experience identity / contracts
 |-- GUI semantic concepts
 `-- VR platform adapter
          |
          v
     device/runtime API
```

MyVR must not make upstream packages depend on a specific VR SDK.

## First vertical slice

1. MyVR starts.
2. The device/runtime capability boundary is detected.
3. A published Experience identity can be resolved.
4. A minimal Experience representation can be entered.
5. One VR interaction produces one bounded Experience event.
6. The client survives temporary companion/network loss.

Only then should sophisticated spatial rendering be added.

## Testing

Test identity parsing, artifact identity validation, capability detection, session transitions, malformed messages, replay/expiry behavior, companion lifecycle, bounded event handling, graceful disconnect, and device-independent semantic logic.

Platform/device tests should remain separate from deterministic core tests.

## Technology rule

Do not select a VR engine merely because it is familiar. Keep the architecture engine-neutral until device targets, deployment constraints, licensing, and required APIs are understood.