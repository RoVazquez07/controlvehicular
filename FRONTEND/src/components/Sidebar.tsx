import { useEffect, useRef, useState } from 'react'
import { Link, useLocation } from 'react-router-dom'
import { signOut } from '../auth'

const menu = [
  { label: 'Main view', icon: 'fa fa-th-large', to: '/app' },
  { label: 'Minor view', icon: 'fa fa-th-large', to: '/minor' },
] as const

export function Sidebar() {
  const location = useLocation()
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

  return (
    <nav className="navbar-default navbar-static-side" role="navigation">
      <div className="sidebar-collapse">
        <ul className="nav metismenu" id="side-menu">
          <li className="nav-header">
            <div
              ref={profileRef}
              className={`dropdown profile-element${profileOpen ? ' show' : ''}`}
            >
              <a
                className="dropdown-toggle"
                href="#"
                onClick={(event) => {
                  event.preventDefault()
                  setProfileOpen((open) => !open)
                }}
              >
                <span className="block m-t-xs font-bold">Example user</span>
                <span className="text-muted text-xs block">
                  menu <b className="caret"></b>
                </span>
              </a>
              <ul
                className={`dropdown-menu animated fadeInRight m-t-xs${profileOpen ? ' show' : ''}`}
              >
                <li>
                  <Link className="dropdown-item" to="/" onClick={signOut}>
                    Logout
                  </Link>
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
