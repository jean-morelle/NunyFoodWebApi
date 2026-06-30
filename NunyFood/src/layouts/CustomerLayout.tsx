import { Outlet, Link, useNavigate, NavLink } from 'react-router-dom'
import { useAuthStore } from '../store/authStore'
import ThemeToggle from '../components/ui/ThemeToggle'

const navItems = [
  { to: '/customer/dashboard', label: 'Tableau de bord' },
  { to: '/customer/packs', label: 'Packs' },
  { to: '/customer/beneficiaries', label: 'Bénéficiaires' },
  { to: '/customer/orders', label: 'Commandes' },
  { to: '/customer/payments', label: 'Paiements' },
  { to: '/customer/profile', label: 'Profil' },
]

export default function CustomerLayout() {
  const { user, logout } = useAuthStore()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/')
  }

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-900 flex transition-colors">
      {/* Sidebar */}
      <aside className="w-64 bg-white dark:bg-gray-800 shadow-md flex flex-col transition-colors">
        <div className="h-16 flex items-center px-6 border-b dark:border-gray-700">
          <Link to="/" className="flex items-center gap-1">
            <span className="text-xl font-bold text-[#16A34A]">Nuny</span>
            <span className="text-xl font-bold text-[#F97316]">Food</span>
          </Link>
        </div>
        <nav className="flex-1 py-4 px-3 space-y-1">
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                `block px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
                  isActive
                    ? 'bg-[#16A34A] text-white'
                    : 'text-gray-600 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 hover:text-gray-900 dark:hover:text-white'
                }`
              }
            >
              {item.label}
            </NavLink>
          ))}
        </nav>
        <div className="p-4 border-t dark:border-gray-700">
          <p className="text-xs text-gray-500 dark:text-gray-400 mb-1">Connecté en tant que</p>
          <p className="text-sm font-medium text-gray-800 dark:text-gray-100 truncate">{user?.email}</p>
          <button
            onClick={handleLogout}
            className="mt-3 w-full text-sm text-red-600 dark:text-red-400 hover:text-red-800 dark:hover:text-red-300 text-left transition-colors"
          >
            Se déconnecter
          </button>
        </div>
      </aside>

      {/* Main */}
      <div className="flex-1 flex flex-col min-w-0">
        <header className="h-16 bg-white dark:bg-gray-800 shadow-sm flex items-center justify-between px-8 transition-colors">
          <h1 className="text-lg font-semibold text-gray-800 dark:text-gray-100">Espace Client</h1>
          <ThemeToggle />
        </header>
        <main className="flex-1 p-8 overflow-auto">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
