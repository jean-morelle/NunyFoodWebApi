import { Outlet, Link, useNavigate, NavLink } from 'react-router-dom'
import { useAuthStore } from '../store/authStore'
import ThemeToggle from '../components/ui/ThemeToggle'

const navItems = [
  { to: '/admin/dashboard', label: 'Tableau de bord' },
  { to: '/admin/customers', label: 'Clients' },
  { to: '/admin/products', label: 'Produits' },
  { to: '/admin/packs', label: 'Packs' },
  { to: '/admin/orders', label: 'Commandes' },
  { to: '/admin/payments', label: 'Paiements' },
  { to: '/admin/delivery-agents', label: 'Livreurs' },
  { to: '/admin/deliveries', label: 'Livraisons' },
]

export default function AdminLayout() {
  const { user, logout } = useAuthStore()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/')
  }

  return (
    <div className="min-h-screen bg-gray-50 flex">
      <aside className="w-64 bg-[#16A34A] text-white flex flex-col">
        <div className="h-16 flex items-center px-6 border-b border-green-700">
          <Link to="/" className="flex items-center gap-1">
            <span className="text-xl font-bold text-white">Nuny</span>
            <span className="text-xl font-bold text-[#F97316]">Food</span>
            <span className="text-xs ml-2 bg-green-800 px-2 py-0.5 rounded-full">Admin</span>
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
                    ? 'bg-green-800 text-white'
                    : 'text-green-100 hover:bg-green-700 hover:text-white'
                }`
              }
            >
              {item.label}
            </NavLink>
          ))}
        </nav>
        <div className="p-4 border-t border-green-700">
          <p className="text-xs text-green-200 mb-1">Administrateur</p>
          <p className="text-sm font-medium text-white truncate">{user?.email}</p>
          <button
            onClick={handleLogout}
            className="mt-3 w-full text-sm text-red-300 hover:text-red-100 text-left transition-colors"
          >
            Se déconnecter
          </button>
        </div>
      </aside>

      <div className="flex-1 flex flex-col min-w-0">
        <header className="h-16 bg-white dark:bg-gray-800 shadow-sm flex items-center justify-between px-8 transition-colors">
          <h1 className="text-lg font-semibold text-gray-800 dark:text-gray-100">Administration NunyFood</h1>
          <ThemeToggle />
        </header>
        <main className="flex-1 p-8 overflow-auto bg-gray-50 dark:bg-gray-900 transition-colors">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
