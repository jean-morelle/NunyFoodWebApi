import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { getCustomers } from '../../api/customers'
import { getOrders } from '../../api/orders'
import { getPacks } from '../../api/packs'
import { getDeliveryAgents } from '../../api/deliveries'
import { OrderStatusBadge } from '../../components/ui/Badge'

export default function AdminDashboardPage() {
  const { data: customers = [] } = useQuery({ queryKey: ['admin-customers'], queryFn: getCustomers })
  const { data: orders = [] } = useQuery({ queryKey: ['admin-orders'], queryFn: () => getOrders() })
  const { data: packs = [] } = useQuery({ queryKey: ['admin-packs'], queryFn: getPacks })
  const { data: agents = [] } = useQuery({ queryKey: ['admin-agents'], queryFn: getDeliveryAgents })

  const stats = [
    { label: 'Clients', value: customers.length, href: '/admin/customers', color: 'bg-blue-50 text-blue-700' },
    { label: 'Commandes', value: orders.length, href: '/admin/orders', color: 'bg-orange-50 text-orange-700' },
    { label: 'Packs actifs', value: packs.filter((p) => p.isActive).length, href: '/admin/packs', color: 'bg-green-50 text-[#16A34A]' },
    { label: 'Livreurs', value: agents.filter((a) => a.isActive).length, href: '/admin/delivery-agents', color: 'bg-purple-50 text-purple-700' },
  ]

  const recentOrders = [...orders].reverse().slice(0, 5)

  return (
    <div className="space-y-8">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Tableau de bord</h2>
        <p className="text-gray-500 mt-1">Vue d'ensemble de la plateforme NunyFood</p>
      </div>

      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        {stats.map(({ label, value, href, color }) => (
          <Link key={label} to={href} className={`${color} rounded-xl p-6 hover:opacity-90 transition-opacity`}>
            <p className="text-3xl font-bold">{value}</p>
            <p className="text-sm font-medium mt-1 opacity-80">{label}</p>
          </Link>
        ))}
      </div>

      <div className="bg-white rounded-xl border border-gray-200">
        <div className="flex items-center justify-between p-6 border-b">
          <h3 className="font-semibold text-gray-900">Dernières commandes</h3>
          <Link to="/admin/orders" className="text-sm text-[#16A34A] hover:underline">Voir tout</Link>
        </div>
        {recentOrders.length === 0 ? (
          <div className="p-10 text-center text-gray-400 text-sm">Aucune commande</div>
        ) : (
          <ul className="divide-y divide-gray-100">
            {recentOrders.map((o) => (
              <li key={o.id} className="flex items-center justify-between px-6 py-4">
                <div>
                  <p className="text-sm font-medium text-gray-900">#{o.id.slice(0, 8)}</p>
                  <p className="text-xs text-gray-400">{new Date(o.createdAt).toLocaleDateString('fr-FR')}</p>
                </div>
                <div className="flex items-center gap-4">
                  <span className="text-sm font-semibold text-gray-700">{o.amount.toLocaleString()} FCFA</span>
                  <OrderStatusBadge status={o.status} />
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  )
}
