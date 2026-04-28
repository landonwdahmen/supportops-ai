import { getStoredToken } from "../auth/authStorage";

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL || "/api";

type RequestOptions = {
  method?: string;
  body?: unknown;
  token?: string | null;
};

export class ApiError extends Error {
  constructor(
    message: string,
    public readonly status: number
  ) {
    super(message);
  }
}

export async function apiRequest<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const token = options.token ?? getStoredToken();
  const headers = new Headers();

  if (options.body !== undefined) {
    headers.set("Content-Type", "application/json");
  }

  if (token) {
    headers.set("Authorization", `Bearer ${token}`);
  }

  const response = await fetch(`${apiBaseUrl}${path}`, {
    method: options.method ?? "GET",
    headers,
    body: options.body === undefined ? undefined : JSON.stringify(options.body)
  });

  if (!response.ok) {
    throw new ApiError(await readError(response), response.status);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

async function readError(response: Response) {
  try {
    const payload = await response.json();
    if (Array.isArray(payload.error)) {
      return payload.error.join(" ");
    }

    if (typeof payload.error === "string") {
      return payload.error;
    }
  } catch {
    // Fall through to a generic message.
  }

  return `Request failed with status ${response.status}.`;
}
