export type AuthUser = {
  id: string
  email: string
  name?: string
}

export type LoginCredentials = {
  email: string
  password: string
}

/**
 * Contrato esperado del endpoint POST /api/auth/login.
 * Ajusta los nombres de campos si el backend usa otro shape.
 */
export type LoginResponse = {
  accessToken: string
  refreshToken?: string
  user: AuthUser
}

export type AuthSession = {
  accessToken: string
  refreshToken?: string
  user: AuthUser
}
