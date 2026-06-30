import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useParams, useNavigate } from 'react-router-dom'
import { useState } from 'react'
import { getOrder, getOrderStatusHistory } from '../../api/orders'
import { createPayment } from '../../api/payments'
import { OrderStatusBadge } from '../../components/ui/Badge'
import Button from '../../components/ui/Button'
import type { PaymentMethod } from '../../types'

const paymentMethods: PaymentMethod[] = ['PayPal', 'TMoney', 'Flooz']

export default function OrderDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [method, setMethod] = useState<PaymentMethod>('PayPal')

  const { data: order } = useQuery({
    queryKey: ['orders', id],
    queryFn: () => getOrder(id!),
    enabled: !!id,
  })

  const { data: history = [] } = useQuery({
    queryKey: ['order-history', id],
    queryFn: () => getOrderStatusHistory(id!),
    enabled: !!id,
  })

  const payMutation = useMutation({
    mutationFn: () => createPayment({ orderId: id!, amount: order!.amount, method }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] })
    },
  })

  if (!order) return <div className="flex justify-center h-64 items-center"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-[#16A34A]" /></div>

  const canPay = order.status === 'Created' || order.status === 'PendingPayment'

  return (
    <div className="max-w-2xl space-y-6">
      <button onClick={() => navigate(-1)} className="text-sm text-[#16A34A] hover:underline">← Retour</button>

      {/* Order info */}
      <div className="bg-white rounded-xl border border-gray-200 p-6">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-bold text-gray-900">Commande #{order.id.slice(0, 8)}</h2>
          <OrderStatusBadge status={order.status} />
        </div>
        <div className="grid grid-cols-2 gap-4 text-sm">
          <div>
            <p className="text-gray-500">Montant</p>
            <p className="font-semibold text-gray-900 text-lg">{order.amount.toLocaleString()} FCFA</p>
          </div>
          <div>
            <p className="text-gray-500">Date</p>
            <p className="font-medium text-gray-700">{new Date(order.createdAt).toLocaleDateString('fr-FR')}</p>
          </div>
        </div>
      </div>

      {/* Payment */}
      {canPay && (
        <div className="bg-white rounded-xl border border-gray-200 p-6 space-y-4">
          <h3 className="font-semibold text-gray-900">Payer cette commande</h3>
          <div className="flex gap-3">
            {paymentMethods.map((m) => (
              <button
                key={m}
                onClick={() => setMethod(m)}
                className={`flex-1 py-2.5 rounded-lg border text-sm font-medium transition-colors ${method === m ? 'border-[#16A34A] bg-green-50 text-[#16A34A]' : 'border-gray-300 text-gray-600 hover:bg-gray-50'}`}
              >
                {m}
              </button>
            ))}
          </div>
          <Button onClick={() => payMutation.mutate()} loading={payMutation.isPending} className="w-full" size="lg">
            Payer {order.amount.toLocaleString()} FCFA via {method}
          </Button>
          {payMutation.isSuccess && (
            <p className="text-sm text-center text-[#16A34A] font-medium">Paiement effectué !</p>
          )}
          {payMutation.isError && (
            <p className="text-sm text-center text-red-600">Erreur lors du paiement.</p>
          )}
        </div>
      )}

      {/* Status history */}
      <div className="bg-white rounded-xl border border-gray-200 p-6">
        <h3 className="font-semibold text-gray-900 mb-4">Historique des statuts</h3>
        <ol className="relative border-l border-gray-200 space-y-4 ml-3">
          {history.map((h) => (
            <li key={h.id} className="ml-4">
              <div className="absolute w-3 h-3 bg-[#16A34A] rounded-full -left-1.5 mt-1" />
              <div className="flex items-center gap-3">
                <OrderStatusBadge status={h.status} />
                <span className="text-xs text-gray-400">
                  {new Date(h.changedAt).toLocaleDateString('fr-FR', { day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit' })}
                </span>
              </div>
            </li>
          ))}
        </ol>
      </div>
    </div>
  )
}
