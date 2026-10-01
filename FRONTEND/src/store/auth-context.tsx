import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { Navigate, Outlet } from 'react-router-dom'
import * as authService from '../services/auth.service'
import { ApiError } from '../services/http'
import type { AuthSession, AuthUser, LoginCredentials } from '../types/auth'

const SESSION_KEY = 'cv_auth_session'

type AuthContextValue = {
  user: AuthUser | null
  token: string | null
  isAuthenticated: boolean
  isAuthenticating: boolean
  login: (credentials: LoginCredentials) => Promise<void>
  logout: () => Promise<void>
}

const AuthContext = createContext<AuthContextValue | null>(null)

function readStoredSession(): AuthSession | null {
  const raw = sessionStorage.getItem(SESSION_KEY)
  if (!raw) return null
  try {
    const parsed = JSON.parse(raw) as AuthSession
    if (!parsed?.accessToken || !parsed?.user?.email) return null
    return parsed
  } catch {
    return null
  }
}

function persistSession(session: AuthSession | null) {
  if (!session) {
    sessionStorage.removeItem(SESSION_KEY)
    return
  }
  sessionStorage.setItem(SESSION_KEY, JSON.stringify(session))
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<AuthSession | null>(() => readStoredSession())
  const [isAuthenticating, setIsAuthenticating] = useState(false)

  const login = useCallback(async (credentials: LoginCredentials) => {
    setIsAuthenticating(true)
    try {
      const nextSession = await authService.login(credentials)
      persistSession(nextSession)
      setSession(nextSession)
    } finally {
      setIsAuthenticating(false)
    }
  }, [])

  const logout = useCallback(async () => {
    const token = session?.accessToken
    persistSession(null)
    setSession(null)
    if (token) {
      await authService.logout(token)
    }
  }, [session?.accessToken])

  const value = useMemo<AuthContextValue>(
    () => ({
      user: session?.user ?? null,
      token: session?.accessToken ?? null,
      isAuthenticated: Boolean(session?.accessToken),
      isAuthenticating,
      login,
      logout,
    }),
    [session, isAuthenticating, login, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth debe usarse dentro de AuthProvider')
  }
  return context
}

export function RequireAuth() {
  const { isAuthenticated } = useAuth()
  if (!isAuthenticated) {
    return <Navigate to="/" replace />
  }
  return <Outlet />
}

export function RequireGuest() {
  const { isAuthenticated } = useAuth()
  if (isAuthenticated) {
    return <Navigate to="/app" replace />
  }
  return <Outlet />
}

export function getErrorMessage(error: unknown): string {
  if (error instanceof ApiError) return error.message
  if (error instanceof Error) return error.message
  return 'No se pudo iniciar sesión'
}
