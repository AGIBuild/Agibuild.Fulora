/**
 * Typed Bridge service proxies.
 * Each proxy maps to a C# [JsExport] service exposed via the Agibuild WebView Bridge.
 */
import { bridgeClient } from '@agibuild/bridge';
const appShellRpc = bridgeClient.getService('AppShellService');
const systemInfoRpc = bridgeClient.getService('SystemInfoService');
const chatRpc = bridgeClient.getService('ChatService');
const fileRpc = bridgeClient.getService('FileService');
const settingsRpc = bridgeClient.getService('SettingsService');
// ─── Service proxies ────────────────────────────────────────────────────────
export const appShellService = {
    getPages: () => appShellRpc.getPages(),
    getAppInfo: () => appShellRpc.getAppInfo(),
};
export const systemInfoService = {
    getSystemInfo: () => systemInfoRpc.getSystemInfo(),
    getRuntimeMetrics: () => systemInfoRpc.getRuntimeMetrics(),
};
export const chatService = {
    sendMessage: (request) => chatRpc.sendMessage({ request }),
    getHistory: () => chatRpc.getHistory(),
    clearHistory: () => chatRpc.clearHistory(),
};
export const fileService = {
    listFiles: (path) => fileRpc.listFiles({ path }),
    readTextFile: (path) => fileRpc.readTextFile({ path }),
    getUserDocumentsPath: () => fileRpc.getUserDocumentsPath(),
};
export const settingsService = {
    getSettings: () => settingsRpc.getSettings(),
    updateSettings: (settings) => settingsRpc.updateSettings({ settings }),
};
