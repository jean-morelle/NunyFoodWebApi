import { Outlet, Link } from 'react-router-dom'
import ThemeToggle from '../components/ui/ThemeToggle'

export default function PublicLayout() {
  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-900 transition-colors">
      <header className="bg-white dark:bg-gray-800 shadow-sm transition-colors">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex items-center justify-between h-16">
          <Link to="/" className="flex items-center gap-2">
            <span className="text-2xl font-bold text-[#16A34A]">Nuny</span>
            <span className="text-2xl font-bold text-[#F97316]">Food</span>
          </Link>
          <nav className="flex items-center gap-4">
            <ThemeToggle />
            <Link to="/login" className="text-gray-600 dark:text-gray-300 hover:text-[#16A34A] font-medium transition-colors">
              Connexion
            </Link>
            <Link
              to="/register"
              className="bg-[#16A34A] text-white px-4 py-2 rounded-lg font-medium hover:bg-[#15803D] transition-colors"
            >
              S'inscrire
            </Link>
          </nav>
        </div>
      </header>
      <main>
        <Outlet />
      </main>
    </div>
  )
}
