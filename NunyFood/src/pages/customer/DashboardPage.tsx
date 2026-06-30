import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { useAuthStore } from '../../store/authStore'
import { getOrders } from '../../api/orders'
import { OrderStatusBadge } from '../../components/ui/Badge'

export default function CustomerDashboardPage() {
  const { user } = useAuthStore()
  const { data: orders = [] } = useQuery({
    queryKey: ['orders', user?.id],
    queryFn: () => getOrders(user!.id),
    enabled: !!user,
  })

  const stats = {
    total: orders.length,
    active: orders.filter((o) => !['Delivered', 'Confirmed', 'Cancelled'].includes(o.status)).length,
    confirmed: orders.filter((o) => o.status === 'Confirmed').length,
  }

  const recent = orders.slice(-5).reverse()

  return (
    <div className="space-y-8">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Bonjour 👋</h2>
        <p className="text-gray-500 mt-1">Voici un aperçu de vos commandes</p>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        {[
          { label: 'Total commandes', value: stats.total, color: 'text-[#16A34A]' },
          { label: 'En cours', value: stats.active, color: 'text-[#F97316]' },
          { label: 'Confirmées', value: stats.confirmed, color: 'text-blue-600' },
        ].map(({ label, value, color }) => (
          <div key={label} className="bg-white rounded-xl border border-gray-200 p-6">
            <p className="text-sm text-gray-500">{label}</p>
            <p className={`text-3xl font-bold mt-1 ${color}`}>{value}</p>
          </div>
        ))}
      </div>

      {/* Quick links */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <Link
          to="/customer/packs"
          className="bg-[#16A34A] text-white rounded-xl p-6 hover:bg-[#15803D] transition-colors"
        >
          <p className="text-xl font-bold mb-1">Commander un pack</p>
          <p className="text-green-100 text-sm">Choisissez parmi nos packs alimentaires</p>
        </Link>
        <Link
          to="/customer/beneficiaries"
          className="bg-white border border-gray-200 rounded-xl p-6 hover:bg-gray-50 transition-colors"
        >
          <p className="text-xl font-bold text-gray-900 mb-1">Mes bénéficiaires</p>
          <p className="text-gray-500 text-sm">Gérez les personnes qui reçoivent vos packs</p>
        </Link>
      </div>

      {/* Recent orders */}
      <div className="bg-white rounded-xl border border-gray-200">
        <div className="flex items-center justify-between p-6 border-b">
          <h3 className="font-semibold text-gray-900">Commandes récentes</h3>
          <Link to="/customer/orders" className="text-sm text-[#16A34A] hover:underline">
            Voir tout
          </Link>
        </div>
        {recent.length === 0 ? (
          <div className="p-10 text-center text-gray-400 text-sm">Aucune commande pour le moment</div>
        ) : (
          <ul className="divide-y divide-gray-100">
            {recent.map((order) => (
              <li key={order.id} className="flex items-center justify-between px-6 py-4">
                <div>
                  <p className="text-sm font-medium text-gray-900">Commande #{order.id.slice(0, 8)}</p>
                  <p className="text-xs text-gray-400 mt-0.5">{new Date(order.createdAt).toLocaleDateString('fr-FR')}</p>
                </div>
                <div className="flex items-center gap-4">
                  <span className="text-sm font-semibold text-gray-700">{order.amount.toLocaleString()} FCFA</span>
                  <OrderStatusBadge status={order.status} />
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  )
}
