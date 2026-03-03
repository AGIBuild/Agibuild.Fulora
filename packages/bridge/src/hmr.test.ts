import { describe, it } from "node:test";
import assert from "node:assert/strict";
import {
  serializeBridgeState,
  restoreBridgeState,
  type BridgeState,
} from "./hmr.ts";

describe("serializeBridgeState", () => {
  it("returns empty pendingCalls and eventSubscriptions", () => {
    const bridge = {};
    const state = serializeBridgeState(bridge);
    assert.deepEqual(state.pendingCalls, []);
    assert.deepEqual(state.eventSubscriptions, []);
  });

  it("returns a plain object suitable for JSON serialization", () => {
    const state = serializeBridgeState({});
    const json = JSON.stringify(state);
    const parsed = JSON.parse(json) as BridgeState;
    assert.deepEqual(parsed.pendingCalls, []);
    assert.deepEqual(parsed.eventSubscriptions, []);
  });
});

describe("restoreBridgeState", () => {
  it("accepts bridge and state without throwing", () => {
    const bridge = {};
    const state: BridgeState = { pendingCalls: [], eventSubscriptions: [] };
    assert.doesNotThrow(() => restoreBridgeState(bridge, state));
  });

  it("accepts state with eventSubscriptions", () => {
    const bridge = {};
    const state: BridgeState = {
      pendingCalls: [],
      eventSubscriptions: ["event1", "event2"],
    };
    assert.doesNotThrow(() => restoreBridgeState(bridge, state));
  });
});
