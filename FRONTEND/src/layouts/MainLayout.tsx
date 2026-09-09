import { Outlet } from 'react-router-dom'
import { Footer } from '../components/Footer'
import { Navbar } from '../components/Navbar'
import { Sidebar } from '../components/Sidebar'
import { LayoutProvider } from '../store/layout-context'

export function MainLayout() {
  return (
    <LayoutProvider>
      <div id="wrapper">
        <Sidebar />
        <div id="page-wrapper" className="gray-bg">
          <Navbar />
          <Outlet />
          <Footer />
        </div>
      </div>
    </LayoutProvider>
  )
}
