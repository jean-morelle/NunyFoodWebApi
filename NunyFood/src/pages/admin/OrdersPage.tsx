import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { getOrders, updateOrderStatus } from '../../api/orders'
import { OrderStatusBadge } from '../../components/ui/Badge'
import Button from '../../components/ui/Button'
import Modal from '../../components/ui/Modal'
import type { Order, OrderStatus } from '../../types'

const statuses: OrderStatus[] = ['Created','PendingPayment','Paid','Preparing','Assigned','InDelivery','Delivered','Confirmed','Cancelled']
const statusLabels: Record<OrderStatus, string> = {
  Created: 'Créée', PendingPayment: 'En attente de paiement', Paid: 'Payée',
  Preparing: 'En préparation', Assigned: 'Assignée', InDelivery: 'En livraison',
  Delivered: 'Livrée', Confirmed: 'Confirmée', Cancelled: 'Annulée',
}

export default function AdminOrdersPage() {
  const queryClient = useQueryClient()
  const [selected, setSelected] = useState<Order | null>(null)
  const [newStatus, setNewStatus] = useState<OrderStatus>('Created')

  const { data: orders = [] } = useQuery({ queryKey: ['admin-orders'], queryFn: () => getOrders() })

  const updateMut = useMutation({
    mutationFn: () => updateOrderStatus(selected!.id, newStatus),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['admin-orders'] }); setSelected(null) },
  })

  const openModal = (order: Order) => { setSelected(order); setNewStatus(order.status) }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Commandes</h2>
        <p className="text-gray-500 mt-1">{orders.length} commande(s) au total</p>
      </div>

      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              {['Référence', 'Montant', 'Statut', 'Date', 'Actions'].map((h) => (
                <th key={h} className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">{h}</th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {[...orders].reverse().map((o) => (
              <tr key={o.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 text-sm font-mono text-gray-500">#{o.id.slice(0, 8)}</td>
                <td className="px-6 py-4 text-sm font-semibold text-gray-900">{o.amount.toLocaleString()} FCFA</td>
                <td className="px-6 py-4"><OrderStatusBadge status={o.status} /></td>
                <td className="px-6 py-4 text-sm text-gray-500">{new Date(o.createdAt).toLocaleDateString('fr-FR')}</td>
                <td className="px-6 py-4">
                  <button onClick={() => openModal(o)} className="text-sm text-[#16A34A] hover:underline">
                    Changer statut
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {orders.length === 0 && <div className="py-10 text-center text-gray-400 text-sm">Aucune commande</div>}
      </div>

      <Modal open={!!selected} onClose={() => setSelected(null)} title="Changer le statut">
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Nouveau statut</label>
            <select
              value={newStatus}
              onChange={(e) => setNewStatus(e.target.value as OrderStatus)}
              className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[#16A34A]"
            >
              {statuses.map((s) => <option key={s} value={s}>{statusLabels[s]}</option>)}
            </select>
          </div>
          <div className="flex gap-3 pt-2">
            <Button variant="ghost" onClick={() => setSelected(null)} className="flex-1">Annuler</Button>
            <Button onClick={() => updateMut.mutate()} loading={updateMut.isPending} className="flex-1">Confirmer</Button>
          </div>
        </div>
      </Modal>
    </div>
  )
}
