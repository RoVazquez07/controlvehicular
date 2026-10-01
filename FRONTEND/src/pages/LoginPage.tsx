import { useEffect, useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { getErrorMessage, useAuth } from '../store/auth-context'

export function LoginPage() {
  const navigate = useNavigate()
  const { login, isAuthenticating } = useAuth()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    document.title = 'INSPINIA | Login'
  }, [])

  const onSubmit = async (event: FormEvent) => {
    event.preventDefault()
    setError(null)

    try {
      await login({ email: email.trim(), password })
      navigate('/app')
    } catch (err) {
      setError(getErrorMessage(err))
    }
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
              name="email"
              autoComplete="username"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              required
              disabled={isAuthenticating}
            />
          </div>
          <div className="form-group">
            <input
              type="password"
              className="form-control"
              placeholder="Password"
              name="password"
              autoComplete="current-password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              required
              disabled={isAuthenticating}
            />
          </div>
          {error ? (
            <p className="text-danger" role="alert">
              {error}
            </p>
          ) : null}
          <button
            type="submit"
            className="btn btn-primary block full-width m-b"
            disabled={isAuthenticating}
          >
            {isAuthenticating ? 'Ingresando…' : 'Login'}
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
