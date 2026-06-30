import { useQuery } from '@tanstack/react-query'
import { getOrders } from '../../api/orders'
import { getPaymentsByOrder } from '../../api/payments'
import { PaymentStatusBadge } from '../../components/ui/Badge'
import type { Payment } from '../../types'

function useAllPayments() {
  const { data: orders = [] } = useQuery({ queryKey: ['admin-orders'], queryFn: () => getOrders() })
  return useQuery({
    queryKey: ['admin-all-payments'],
    queryFn: async () => {
      const results = await Promise.all(orders.map((o) => getPaymentsByOrder(o.id)))
      return results.flat() as Payment[]
    },
    enabled: orders.length > 0,
  })
}

export default function AdminPaymentsPage() {
  const { data: payments = [] } = useAllPayments()
  const total = payments.filter((p) => p.status === 'Succeeded').reduce((sum, p) => sum + p.amount, 0)

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Paiements</h2>
        <p className="text-gray-500 mt-1">{payments.length} paiement(s) — Total encaissé : <span className="text-[#16A34A] font-semibold">{total.toLocaleString()} FCFA</span></p>
      </div>

      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              {['Référence', 'Commande', 'Montant', 'Méthode', 'Transaction', 'Statut', 'Date'].map((h) => (
                <th key={h} className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">{h}</th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {[...payments].reverse().map((p) => (
              <tr key={p.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 text-sm font-mono text-gray-400">#{p.id.slice(0, 8)}</td>
                <td className="px-6 py-4 text-sm font-mono text-gray-400">#{p.orderId.slice(0, 8)}</td>
                <td className="px-6 py-4 text-sm font-semibold text-gray-900">{p.amount.toLocaleString()} FCFA</td>
                <td className="px-6 py-4 text-sm text-gray-600">{p.method}</td>
                <td className="px-6 py-4 text-sm text-gray-400 font-mono">{p.transactionId ?? '—'}</td>
                <td className="px-6 py-4"><PaymentStatusBadge status={p.status} /></td>
                <td className="px-6 py-4 text-sm text-gray-500">{new Date(p.createdAt).toLocaleDateString('fr-FR')}</td>
              </tr>
            ))}
          </tbody>
        </table>
        {payments.length === 0 && <div className="py-10 text-center text-gray-400 text-sm">Aucun paiement</div>}
      </div>
    </div>
  )
}
