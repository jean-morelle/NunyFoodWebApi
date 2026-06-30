import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { getOrders } from '../../api/orders'
import { getDeliveryAgents, createDelivery } from '../../api/deliveries'
import { OrderStatusBadge } from '../../components/ui/Badge'
import Button from '../../components/ui/Button'
import Input from '../../components/ui/Input'
import Modal from '../../components/ui/Modal'

const schema = z.object({
  orderId: z.string().min(1, 'Commande requise'),
  deliveryAgentId: z.string().min(1, 'Livreur requis'),
  receiverName: z.string().min(1, 'Nom du receveur requis'),
})
type FormData = z.infer<typeof schema>

export default function AdminDeliveriesPage() {
  const queryClient = useQueryClient()
  const [modal, setModal] = useState(false)

  const { data: orders = [] } = useQuery({ queryKey: ['admin-orders'], queryFn: () => getOrders() })
  const { data: agents = [] } = useQuery({ queryKey: ['admin-agents'], queryFn: getDeliveryAgents })

  const assignableOrders = orders.filter((o) => o.status === 'Paid' || o.status === 'Preparing')

  const createMut = useMutation({
    mutationFn: createDelivery,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-orders'] })
      setModal(false)
    },
  })

  const { register, handleSubmit, reset, formState: { errors, isSubmitting } } = useForm<FormData>({ resolver: zodResolver(schema) })
  const onSubmit = (data: FormData) => createMut.mutate(data)

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Livraisons</h2>
          <p className="text-gray-500 mt-1">Gérez les affectations de livraison</p>
        </div>
        <Button onClick={() => { reset({}); setModal(true) }}>+ Affecter une livraison</Button>
      </div>

      {/* Orders in delivery */}
      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        <div className="px-6 py-4 border-b bg-gray-50">
          <p className="text-sm font-medium text-gray-700">Commandes en cours de livraison</p>
        </div>
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              {['Référence', 'Montant', 'Statut', 'Date'].map((h) => (
                <th key={h} className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">{h}</th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {orders.filter((o) => ['Assigned','InDelivery','Delivered'].includes(o.status)).map((o) => (
              <tr key={o.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 text-sm font-mono text-gray-500">#{o.id.slice(0, 8)}</td>
                <td className="px-6 py-4 text-sm font-semibold text-gray-900">{o.amount.toLocaleString()} FCFA</td>
                <td className="px-6 py-4"><OrderStatusBadge status={o.status} /></td>
                <td className="px-6 py-4 text-sm text-gray-500">{new Date(o.createdAt).toLocaleDateString('fr-FR')}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <Modal open={modal} onClose={() => setModal(false)} title="Affecter une livraison">
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Commande</label>
            <select {...register('orderId')} className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[#16A34A]">
              <option value="">-- Sélectionner --</option>
              {assignableOrders.map((o) => (
                <option key={o.id} value={o.id}>#{o.id.slice(0, 8)} — {o.amount.toLocaleString()} FCFA</option>
              ))}
            </select>
            {errors.orderId && <p className="text-xs text-red-600 mt-1">{errors.orderId.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Livreur</label>
            <select {...register('deliveryAgentId')} className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[#16A34A]">
              <option value="">-- Sélectionner --</option>
              {agents.filter((a) => a.isActive).map((a) => (
                <option key={a.id} value={a.id}>{a.fullName} — {a.zone}</option>
              ))}
            </select>
            {errors.deliveryAgentId && <p className="text-xs text-red-600 mt-1">{errors.deliveryAgentId.message}</p>}
          </div>
          <Input label="Nom du receveur" {...register('receiverName')} error={errors.receiverName?.message} />
          <div className="flex gap-3 pt-2">
            <Button type="button" variant="ghost" onClick={() => setModal(false)} className="flex-1">Annuler</Button>
            <Button type="submit" loading={isSubmitting} className="flex-1">Affecter</Button>
          </div>
        </form>
      </Modal>
    </div>
  )
}
