import { useQuery } from '@tanstack/react-query'
import { useAuthStore } from '../../store/authStore'
import { getOrders } from '../../api/orders'
import { getPaymentsByOrder } from '../../api/payments'
import { PaymentStatusBadge } from '../../components/ui/Badge'
import type { Payment } from '../../types'

function useAllPayments(customerId: string) {
  const { data: orders = [] } = useQuery({
    queryKey: ['orders', customerId],
    queryFn: () => getOrders(customerId),
  })

  return useQuery({
    queryKey: ['all-payments', customerId],
    queryFn: async () => {
      const results = await Promise.all(orders.map((o) => getPaymentsByOrder(o.id)))
      return results.flat() as Payment[]
    },
    enabled: orders.length > 0,
  })
}

export default function CustomerPaymentsPage() {
  const { user } = useAuthStore()
  const { data: payments = [], isLoading } = useAllPayments(user?.id ?? '')

  if (isLoading) return <div className="flex justify-center h-64 items-center"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-[#16A34A]" /></div>

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Historique des paiements</h2>
        <p className="text-gray-500 mt-1">Tous vos paiements NunyFood</p>
      </div>

      {payments.length === 0 ? (
        <div className="bg-white rounded-xl border border-gray-200 py-16 text-center text-gray-400 text-sm">
          Aucun paiement pour le moment.
        </div>
      ) : (
        <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                {['Référence', 'Montant', 'Méthode', 'Statut', 'Date'].map((h) => (
                  <th key={h} className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">{h}</th>
                ))}
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {[...payments].reverse().map((p) => (
                <tr key={p.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 text-sm font-mono text-gray-500">#{p.id.slice(0, 8)}</td>
                  <td className="px-6 py-4 text-sm font-semibold text-gray-900">{p.amount.toLocaleString()} FCFA</td>
                  <td className="px-6 py-4 text-sm text-gray-600">{p.method}</td>
                  <td className="px-6 py-4"><PaymentStatusBadge status={p.status} /></td>
                  <td className="px-6 py-4 text-sm text-gray-500">{new Date(p.createdAt).toLocaleDateString('fr-FR')}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
