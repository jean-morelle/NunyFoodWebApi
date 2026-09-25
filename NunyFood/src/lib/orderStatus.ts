import type { OrderStatus } from '../types'

export const orderStatusLabels: Record<OrderStatus, string> = {
  Created: 'Créée', PendingPayment: 'En attente de paiement', Paid: 'Payée',
  Preparing: 'En préparation', Assigned: 'Assignée', InDelivery: 'En livraison',
  Delivered: 'Livrée', Confirmed: 'Confirmée', Cancelled: 'Annulée',
}

const cancellableByAdmin: OrderStatus[] = ['Created', 'PendingPayment', 'Paid', 'Preparing', 'Assigned']

/**
 * Statuts que l'admin peut fixer à la main depuis le statut actuel. Miroir des règles de l'API
 * (UpdateOrderStatusCommandValidator.ManualStatuses + OrderStatusTransitions) : les autres statuts
 * suivent le paiement, l'affectation d'un livreur et la livraison.
 */
export function manualStatusOptions(status: OrderStatus): OrderStatus[] {
  return [
    ...(status === 'Paid' ? (['Preparing'] as OrderStatus[]) : []),
    ...(cancellableByAdmin.includes(status) ? (['Cancelled'] as OrderStatus[]) : []),
  ]
}
