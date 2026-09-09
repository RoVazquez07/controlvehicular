import { Navigate, Outlet } from 'react-router-dom'

const AUTH_KEY = 'cv_auth'

export function isLoggedIn(): boolean {
  return sessionStorage.getItem(AUTH_KEY) === '1'
}

export function signIn(): void {
  sessionStorage.setItem(AUTH_KEY, '1')
}

export function signOut(): void {
  sessionStorage.removeItem(AUTH_KEY)
}

export function RequireAuth() {
  if (!isLoggedIn()) {
    return <Navigate to="/" replace />
  }
  return <Outlet />
}

export function RequireGuest() {
  if (isLoggedIn()) {
    return <Navigate to="/app" replace />
  }
  return <Outlet />
}
