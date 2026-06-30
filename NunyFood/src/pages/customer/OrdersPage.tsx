import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { useAuthStore } from '../../store/authStore'
import { getOrders } from '../../api/orders'
import { OrderStatusBadge } from '../../components/ui/Badge'

export default function CustomerOrdersPage() {
  const { user } = useAuthStore()
  const { data: orders = [], isLoading } = useQuery({
    queryKey: ['orders', user?.id],
    queryFn: () => getOrders(user!.id),
    enabled: !!user,
  })

  if (isLoading) {
    return <div className="flex justify-center h-64 items-center"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-[#16A34A]" /></div>
  }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Mes commandes</h2>
        <p className="text-gray-500 mt-1">Suivez l'état de vos commandes</p>
      </div>

      {orders.length === 0 ? (
        <div className="bg-white rounded-xl border border-gray-200 py-16 text-center">
          <p className="text-gray-400 text-sm mb-4">Aucune commande pour le moment.</p>
          <Link to="/customer/packs" className="text-[#16A34A] font-medium hover:underline text-sm">
            Commander un pack
          </Link>
        </div>
      ) : (
        <div className="space-y-3">
          {[...orders].reverse().map((order) => (
            <Link
              key={order.id}
              to={`/customer/orders/${order.id}`}
              className="block bg-white rounded-xl border border-gray-200 p-5 hover:shadow-sm transition-shadow"
            >
              <div className="flex items-center justify-between">
                <div>
                  <p className="font-medium text-gray-900">Commande #{order.id.slice(0, 8)}</p>
                  <p className="text-sm text-gray-400 mt-0.5">
                    {new Date(order.createdAt).toLocaleDateString('fr-FR', { day: 'numeric', month: 'long', year: 'numeric' })}
                  </p>
                </div>
                <div className="flex items-center gap-4">
                  <span className="font-semibold text-gray-700">{order.amount.toLocaleString()} FCFA</span>
                  <OrderStatusBadge status={order.status} />
                </div>
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  )
}
