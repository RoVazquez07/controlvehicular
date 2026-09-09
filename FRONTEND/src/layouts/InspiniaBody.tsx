import { useLayoutEffect } from 'react'
import { useLocation } from 'react-router-dom'

const AUTH_PATHS = new Set(['/', '/login', '/register', '/forgot-password'])

export function InspiniaBody() {
  const { pathname } = useLocation()
  const isAuthScreen = AUTH_PATHS.has(pathname)

  useLayoutEffect(() => {
    document.body.classList.toggle('gray-bg', isAuthScreen)
  }, [isAuthScreen])

  return null
}
