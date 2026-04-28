import type { UserRole } from "./common";

export type AuthResponse = {
  userId: string;
  email: string;
  displayName: string;
  role: UserRole;
  accessToken: string;
  expiresAt: string;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type RegisterRequest = {
  email: string;
  displayName: string;
  password: string;
  role: UserRole;
};
