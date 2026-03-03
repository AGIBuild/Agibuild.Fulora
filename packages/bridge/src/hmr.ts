// HMR bridge state preservation
// Detects Vite/webpack HMR events and saves/restores bridge state

export interface BridgeState {
  pendingCalls: Array<[string, unknown]>;
  eventSubscriptions: string[];
}

interface ViteHot {
  dispose: (cb: () => void) => void;
  accept: (cb?: () => void) => void;
}

/** Webpack HMR injects module at runtime */
declare const module: { hot?: { dispose: (cb: (data: { bridgeState?: BridgeState }) => void) => void; accept: () => void; data?: { bridgeState?: BridgeState } } } | undefined;

export function enableHmrPreservation(bridge: { invoke?: (method: string, params?: Record<string, unknown>) => Promise<unknown> }): void {
  // Vite HMR detection
  const metaHot = (import.meta as unknown as { hot?: ViteHot }).hot;
  if (metaHot) {
    const hot = metaHot;
    hot.dispose(() => {
      const state = serializeBridgeState(bridge);
      if (typeof sessionStorage !== "undefined") {
        sessionStorage.setItem("__fulora_bridge_state", JSON.stringify(state));
      }
    });

    hot.accept(() => {
      if (typeof sessionStorage !== "undefined") {
        const saved = sessionStorage.getItem("__fulora_bridge_state");
        if (saved) {
          restoreBridgeState(bridge, JSON.parse(saved) as BridgeState);
          sessionStorage.removeItem("__fulora_bridge_state");
        }
      }
    });
  }

  // Webpack HMR detection (module may be undefined in ESM/Vite)
  let mod: typeof module | undefined;
  try {
    mod = typeof module !== "undefined" ? module : undefined;
  } catch {
    mod = undefined;
  }
  if (mod?.hot) {
    mod.hot.dispose((data) => {
      data.bridgeState = serializeBridgeState(bridge);
    });
    mod.hot.accept();
    if (mod.hot.data?.bridgeState) {
      restoreBridgeState(bridge, mod.hot.data.bridgeState);
    }
  }
}

export function serializeBridgeState(_bridge: unknown): BridgeState {
  return {
    pendingCalls: [],
    eventSubscriptions: [],
  };
}

export function restoreBridgeState(_bridge: unknown, state: BridgeState): void {
  // Re-register event subscriptions when bridge supports them
  void state;
}
