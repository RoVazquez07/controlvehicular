import { useEffect, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { signIn } from '../auth'

export function LoginPage() {
  const navigate = useNavigate()

  useEffect(() => {
    document.title = 'INSPINIA | Login'
  }, [])

  const onSubmit = (event: FormEvent) => {
    event.preventDefault()
    signIn()
    navigate('/app')
  }

  return (
    <div className="middle-box text-center loginscreen animated fadeInDown">
      <div>
        <div>
          <h1 className="logo-name">IN+</h1>
        </div>
        <h3>Welcome to IN+</h3>
        <p>
          Perfectly designed and precisely prepared admin theme with over 50
          pages with extra new web app views.
        </p>
        <p>Login in. To see it in action.</p>
        <form className="m-t" role="form" onSubmit={onSubmit}>
          <div className="form-group">
            <input
              type="email"
              className="form-control"
              placeholder="Username"
              required
            />
          </div>
          <div className="form-group">
            <input
              type="password"
              className="form-control"
              placeholder="Password"
              required
            />
          </div>
          <button type="submit" className="btn btn-primary block full-width m-b">
            Login
          </button>

          <Link to="/forgot-password">
            <small>Forgot password?</small>
          </Link>
          <p className="text-muted text-center">
            <small>Do not have an account?</small>
          </p>
          <Link className="btn btn-sm btn-white btn-block" to="/register">
            Create an account
          </Link>
        </form>
        <p className="m-t">
          {' '}
          <small>Inspinia we app framework base on Bootstrap 3 &copy; 2014</small>
        </p>
      </div>
    </div>
  )
}
