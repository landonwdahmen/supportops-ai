import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from "react";
import { getCurrentUser, login, register } from "../api/authApi";
import { clearStoredToken, getStoredToken, storeToken } from "./authStorage";
import type { AuthResponse, LoginRequest, RegisterRequest } from "../types/auth";

type AuthContextValue = {
  user: AuthResponse | null;
  token: string | null;
  isBootstrapping: boolean;
  loginUser: (request: LoginRequest) => Promise<void>;
  registerUser: (request: RegisterRequest) => Promise<void>;
  logout: () => void;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthResponse | null>(null);
  const [token, setToken] = useState<string | null>(() => getStoredToken());
  const [isBootstrapping, setIsBootstrapping] = useState(true);

  useEffect(() => {
    let isMounted = true;

    async function bootstrap() {
      if (!token) {
        setIsBootstrapping(false);
        return;
      }

      try {
        const currentUser = await getCurrentUser();
        if (isMounted) {
          setUser(currentUser);
        }
      } catch {
        clearStoredToken();
        if (isMounted) {
          setToken(null);
          setUser(null);
        }
      } finally {
        if (isMounted) {
          setIsBootstrapping(false);
        }
      }
    }

    bootstrap();
    return () => {
      isMounted = false;
    };
  }, [token]);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      token,
      isBootstrapping,
      async loginUser(request) {
        const response = await login(request);
        storeToken(response.accessToken);
        setToken(response.accessToken);
        setUser(response);
      },
      async registerUser(request) {
        const response = await register(request);
        storeToken(response.accessToken);
        setToken(response.accessToken);
        setUser(response);
      },
      logout() {
        clearStoredToken();
        setToken(null);
        setUser(null);
      }
    }),
    [isBootstrapping, token, user]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider.");
  }

  return context;
}
