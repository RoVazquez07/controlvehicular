import { useEffect, useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { ICheck } from '../components/ICheck'

export function RegisterPage() {
  const navigate = useNavigate()
  const [agreed, setAgreed] = useState(false)

  useEffect(() => {
    document.title = 'INSPINIA | Register'
  }, [])

  const onSubmit = (event: FormEvent) => {
    event.preventDefault()
    if (!agreed) {
      return
    }
    navigate('/')
  }

  return (
    <div className="middle-box text-center loginscreen animated fadeInDown">
      <div>
        <div>
          <h1 className="logo-name">IN+</h1>
        </div>
        <h3>Register to IN+</h3>
        <p>Create account to see it in action.</p>
        <form className="m-t" role="form" onSubmit={onSubmit}>
          <div className="form-group">
            <input
              type="text"
              className="form-control"
              placeholder="Name"
              required
            />
          </div>
          <div className="form-group">
            <input
              type="email"
              className="form-control"
              placeholder="Email"
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
          <div className="form-group">
            <ICheck checked={agreed} onChange={setAgreed} required>
              {' '}
              Agree the terms and policy{' '}
            </ICheck>
          </div>
          <button
            type="submit"
            className="btn btn-primary block full-width m-b"
          >
            Register
          </button>
          <p className="text-muted text-center">
            <small>Already have an account?</small>
          </p>
          <Link className="btn btn-sm btn-white btn-block" to="/">
            Login
          </Link>
        </form>
        <p className="m-t">
          <small>Inspinia we app framework base on Bootstrap 3 &copy; 2014</small>
        </p>
      </div>
    </div>
  )
}
