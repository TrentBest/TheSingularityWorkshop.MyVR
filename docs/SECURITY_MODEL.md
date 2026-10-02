# Security Model

VR hardware is a client boundary. MyVR operates with explicit authority.

## Rules

1. Verify Experience identity.
2. Verify artifact content through immutable content identity.
3. Device capability does not grant authority.
4. Companion connections are authenticated and session-bound.
5. Messages are versioned and bounded.
6. Commands are explicit and allowlisted.
7. No arbitrary code execution is exposed.
8. No arbitrary desktop filesystem or process access is exposed.
9. Sessions can expire and reconnect.
10. Device disconnect is a normal state transition.

## Threats

Initial threats include forged identities, stale artifacts, content-hash mismatch, replayed launch/session tokens, malformed or oversized messages, duplicate events, unauthorized companion connections, sleep/reconnect, network loss, compromised companion software, and false capability claims.

## Privacy

Minimize telemetry. Device information is shared only when required by an explicit capability or session contract. Pose, room-scale information, microphone state, camera state, and other device-sensitive data are not globally available merely because the device can provide them.

```text
device capability != client authority != Experience authority != desktop authority
```