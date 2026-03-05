export interface FileDropInfo {
  path: string;
  mimeType: string | null;
  size: number | null;
}

export interface DragDropPayload {
  files: FileDropInfo[] | null;
  text: string | null;
  html: string | null;
  uri: string | null;
}

export type DragDropEffects = 'none' | 'copy' | 'move' | 'link';

export interface DragEvent {
  payload: DragDropPayload;
  allowedEffects: DragDropEffects[];
  effect: DragDropEffects;
  x: number;
  y: number;
}

export interface DropEvent {
  payload: DragDropPayload;
  effect: DragDropEffects;
  x: number;
  y: number;
}

export interface IDragDropBridgeService {
  getLastDropPayloadAsync(signal?: AbortSignal): Promise<DragDropPayload | null>;
  isDragDropSupportedAsync(signal?: AbortSignal): Promise<boolean>;
}
