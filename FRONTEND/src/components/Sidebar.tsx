import { useEffect, useRef, useState } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../store/auth-context'

const menu = [
  { label: 'Main view', icon: 'fa fa-th-large', to: '/app' },
  { label: 'Minor view', icon: 'fa fa-th-large', to: '/minor' },
] as const

export function Sidebar() {
  const location = useLocation()
  const navigate = useNavigate()
  const { user, logout } = useAuth()
  const [profileOpen, setProfileOpen] = useState(false)
  const profileRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    const onClick = (event: MouseEvent) => {
      if (!profileRef.current?.contains(event.target as Node)) {
        setProfileOpen(false)
      }
    }
    document.addEventListener('mousedown', onClick)
    return () => document.removeEventListener('mousedown', onClick)
  }, [])

  const onLogout = async () => {
    setProfileOpen(false)
    await logout()
    navigate('/', { replace: true })
  }

  return (
    <nav className="navbar-default navbar-static-side" role="navigation">
      <div className="sidebar-collapse">
        <ul className="nav metismenu" id="side-menu">
          <li className="nav-header">
            <div
              ref={profileRef}
              className={`dropdown profile-element${profileOpen ? ' show' : ''}`}
            >
              <button
                type="button"
                className="dropdown-toggle btn btn-link"
                onClick={() => setProfileOpen((open) => !open)}
              >
                <span className="block m-t-xs font-bold">
                  {user?.name ?? user?.email ?? 'Usuario'}
                </span>
                <span className="text-muted text-xs block">
                  menu <b className="caret"></b>
                </span>
              </button>
              <ul
                className={`dropdown-menu animated fadeInRight m-t-xs${profileOpen ? ' show' : ''}`}
              >
                <li>
                  <button
                    type="button"
                    className="dropdown-item btn btn-link"
                    onClick={onLogout}
                  >
                    Logout
                  </button>
                </li>
              </ul>
            </div>
            <div className="logo-element">IN+</div>
          </li>
          {menu.map((item) => (
            <li
              key={item.to}
              className={location.pathname === item.to ? 'active' : undefined}
            >
              <Link to={item.to}>
                <i className={item.icon}></i>{' '}
                <span className="nav-label">{item.label}</span>
              </Link>
            </li>
          ))}
        </ul>
      </div>
    </nav>
  )
}
