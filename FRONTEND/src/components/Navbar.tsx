import type { FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../store/auth-context'
import { useLayout } from '../store/layout-context'

export function Navbar() {
  const navigate = useNavigate()
  const { logout } = useAuth()
  const { toggleMiniNavbar } = useLayout()

  const onSearch = (event: FormEvent) => {
    event.preventDefault()
  }

  const onLogout = async () => {
    await logout()
    navigate('/', { replace: true })
  }

  return (
    <div className="row border-bottom">
      <nav
        className="navbar navbar-static-top white-bg"
        role="navigation"
        style={{ marginBottom: 0 }}
      >
        <div className="navbar-header">
          <button
            type="button"
            className="navbar-minimalize minimalize-styl-2 btn btn-primary"
            onClick={toggleMiniNavbar}
          >
            <i className="fa fa-bars"></i>
          </button>
          <form
            role="search"
            className="navbar-form-custom"
            onSubmit={onSearch}
          >
            <div className="form-group">
              <input
                type="text"
                placeholder="Search for something..."
                className="form-control"
                name="top-search"
                id="top-search"
              />
            </div>
          </form>
        </div>
        <ul className="nav navbar-top-links navbar-right">
          <li>
            <button type="button" className="btn btn-link" onClick={onLogout}>
              <i className="fa fa-sign-out"></i> Log out
            </button>
          </li>
        </ul>
      </nav>
    </div>
  )
}
