import type { AuthSession, AuthUser, LoginCredentials, LoginResponse } from '../types/auth'
import { apiRequest } from './http'

/** Rutas del módulo de autenticación — alinear con el backend cuando exista. */
export const AUTH_ENDPOINTS = {
  login: '/api/auth/login',
  logout: '/api/auth/logout',
  me: '/api/auth/me',
} as const

export async function login(credentials: LoginCredentials): Promise<AuthSession> {
  const data = await apiRequest<LoginResponse>(AUTH_ENDPOINTS.login, {
    method: 'POST',
    body: credentials,
  })

  return {
    accessToken: data.accessToken,
    refreshToken: data.refreshToken,
    user: data.user,
  }
}

export async function logout(token: string): Promise<void> {
  try {
    await apiRequest<void>(AUTH_ENDPOINTS.logout, {
      method: 'POST',
      token,
    })
  } catch {
    // El logout local siempre debe completarse aunque el servidor falle.
  }
}

export async function fetchCurrentUser(token: string): Promise<AuthUser> {
  return apiRequest<AuthUser>(AUTH_ENDPOINTS.me, {
    method: 'GET',
    token,
  })
}
