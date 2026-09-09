import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useLayoutEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'

type LayoutContextValue = {
  miniNavbar: boolean
  bodySmall: boolean
  toggleMiniNavbar: () => void
}

const LayoutContext = createContext<LayoutContextValue | null>(null)

export function LayoutProvider({ children }: { children: ReactNode }) {
  const [miniNavbar, setMiniNavbar] = useState(
    () => window.localStorage.getItem('collapse_menu') === 'on',
  )
  const [bodySmall, setBodySmall] = useState(() => window.innerWidth < 769)

  useEffect(() => {
    const onResize = () => setBodySmall(window.innerWidth < 769)
    window.addEventListener('resize', onResize)
    return () => window.removeEventListener('resize', onResize)
  }, [])

  useLayoutEffect(() => {
    document.body.classList.toggle('mini-navbar', miniNavbar)
    document.body.classList.toggle('body-small', bodySmall)
    window.localStorage.setItem('collapse_menu', miniNavbar ? 'on' : 'off')
    return () => {
      document.body.classList.remove('mini-navbar', 'body-small')
    }
  }, [miniNavbar, bodySmall])

  const toggleMiniNavbar = useCallback(() => {
    setMiniNavbar((value) => !value)
  }, [])

  const value = useMemo(
    () => ({ miniNavbar, bodySmall, toggleMiniNavbar }),
    [miniNavbar, bodySmall, toggleMiniNavbar],
  )

  return <LayoutContext.Provider value={value}>{children}</LayoutContext.Provider>
}

export function useLayout() {
  const context = useContext(LayoutContext)
  if (!context) {
    throw new Error('useLayout debe usarse dentro de LayoutProvider')
  }
  return context
}
