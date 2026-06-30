import type { OrderStatus, PaymentStatus } from '../../types'

const orderColors: Record<OrderStatus, string> = {
  Created: 'bg-gray-100 text-gray-700 dark:bg-gray-700 dark:text-gray-200',
  PendingPayment: 'bg-yellow-100 text-yellow-700 dark:bg-yellow-900 dark:text-yellow-200',
  Paid: 'bg-blue-100 text-blue-700 dark:bg-blue-900 dark:text-blue-200',
  Preparing: 'bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-200',
  Assigned: 'bg-indigo-100 text-indigo-700 dark:bg-indigo-900 dark:text-indigo-200',
  InDelivery: 'bg-orange-100 text-orange-700 dark:bg-orange-900 dark:text-orange-200',
  Delivered: 'bg-teal-100 text-teal-700 dark:bg-teal-900 dark:text-teal-200',
  Confirmed: 'bg-green-100 text-green-700 dark:bg-green-900 dark:text-green-200',
  Cancelled: 'bg-red-100 text-red-700 dark:bg-red-900 dark:text-red-200',
}

const orderLabels: Record<OrderStatus, string> = {
  Created: 'Créée',
  PendingPayment: 'En attente de paiement',
  Paid: 'Payée',
  Preparing: 'En préparation',
  Assigned: 'Assignée',
  InDelivery: 'En livraison',
  Delivered: 'Livrée',
  Confirmed: 'Confirmée',
  Cancelled: 'Annulée',
}

const paymentColors: Record<PaymentStatus, string> = {
  Pending: 'bg-yellow-100 text-yellow-700 dark:bg-yellow-900 dark:text-yellow-200',
  Succeeded: 'bg-green-100 text-green-700 dark:bg-green-900 dark:text-green-200',
  Failed: 'bg-red-100 text-red-700 dark:bg-red-900 dark:text-red-200',
}

export function OrderStatusBadge({ status }: { status: OrderStatus }) {
  return (
    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${orderColors[status]}`}>
      {orderLabels[status]}
    </span>
  )
}

export function PaymentStatusBadge({ status }: { status: PaymentStatus }) {
  const labels = { Pending: 'En attente', Succeeded: 'Réussi', Failed: 'Échoué' }
  return (
    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${paymentColors[status]}`}>
      {labels[status]}
    </span>
  )
}

export function ActiveBadge({ isActive }: { isActive: boolean }) {
  return (
    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${isActive ? 'bg-green-100 text-green-700 dark:bg-green-900 dark:text-green-200' : 'bg-gray-100 text-gray-500 dark:bg-gray-700 dark:text-gray-400'}`}>
      {isActive ? 'Actif' : 'Inactif'}
    </span>
  )
}
