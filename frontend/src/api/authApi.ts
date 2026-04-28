import { apiRequest } from "./apiClient";
import type { AuthResponse, LoginRequest, RegisterRequest } from "../types/auth";

export function login(request: LoginRequest) {
  return apiRequest<AuthResponse>("/auth/login", {
    method: "POST",
    body: request,
    token: null
  });
}

export function register(request: RegisterRequest) {
  return apiRequest<AuthResponse>("/auth/register", {
    method: "POST",
    body: request,
    token: null
  });
}

export function getCurrentUser() {
  return apiRequest<AuthResponse>("/auth/me");
}
