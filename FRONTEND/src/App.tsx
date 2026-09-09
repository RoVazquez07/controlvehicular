import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { RequireAuth, RequireGuest } from './auth'
import { BlankLayout } from './layouts/BlankLayout'
import { InspiniaBody } from './layouts/InspiniaBody'
import { MainLayout } from './layouts/MainLayout'
import { ForgotPasswordPage } from './pages/ForgotPasswordPage'
import { LoginPage } from './pages/LoginPage'
import { MainView } from './pages/MainView'
import { MinorView } from './pages/MinorView'
import { RegisterPage } from './pages/RegisterPage'

export default function App() {
  return (
    <BrowserRouter>
      <InspiniaBody />
      <Routes>
        <Route element={<BlankLayout />}>
          <Route element={<RequireGuest />}>
            <Route path="/" element={<LoginPage />} />
            <Route path="/login" element={<Navigate to="/" replace />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/forgot-password" element={<ForgotPasswordPage />} />
          </Route>
        </Route>
        <Route element={<RequireAuth />}>
          <Route element={<MainLayout />}>
            <Route path="/app" element={<MainView />} />
            <Route path="/minor" element={<MinorView />} />
          </Route>
        </Route>
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  )
}
