import type { FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { signOut } from '../auth'
import { useLayout } from '../store/layout-context'

export function Navbar() {
  const { toggleMiniNavbar } = useLayout()

  const onSearch = (event: FormEvent) => {
    event.preventDefault()
  }

  return (
    <div className="row border-bottom">
      <nav
        className="navbar navbar-static-top white-bg"
        role="navigation"
        style={{ marginBottom: 0 }}
      >
        <div className="navbar-header">
          <a
            className="navbar-minimalize minimalize-styl-2 btn btn-primary"
            href="#"
            onClick={(event) => {
              event.preventDefault()
              toggleMiniNavbar()
            }}
          >
            <i className="fa fa-bars"></i>
          </a>
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
            <Link to="/" onClick={signOut}>
              <i className="fa fa-sign-out"></i> Log out
            </Link>
          </li>
        </ul>
      </nav>
    </div>
  )
}
