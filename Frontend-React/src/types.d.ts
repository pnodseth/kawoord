interface Navigator {
  locks: LockManager;
}

interface LockManager {
  request<T>(name: string, callback: (lock?: Lock) => Promise<T> | T): Promise<T>;

  request<T>(name: string, options: LockOptions, callback: (lock?: Lock) => Promise<T> | T): Promise<T>;

  query(): Promise<LockManagerSnapshot>;
}

type LockMode = "shared" | "exclusive";

interface LockOptions {
  mode?: LockMode;
  ifAvailable?: boolean;
  steal?: boolean;
  signal?: AbortSignal;
}

interface LockManagerSnapshot {
  held: LockInfo[];
  pending: LockInfo[];
}

interface LockInfo {
  name: string;
  mode: LockMode;
  clientId: string;
}

interface Lock {
  name: string;
  mode: LockMode;
}
