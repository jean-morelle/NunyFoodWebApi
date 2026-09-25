import { describe, expect, it } from 'vitest'
import { manualStatusOptions } from './orderStatus'

describe('manualStatusOptions (statuts que l’admin fixe à la main)', () => {
  it('propose la préparation seulement pour une commande payée', () => {
    expect(manualStatusOptions('Paid')).toEqual(['Preparing', 'Cancelled'])
  })

  it('propose l’annulation tant que la commande n’est pas en livraison', () => {
    for (const status of ['Created', 'PendingPayment', 'Preparing', 'Assigned'] as const)
      expect(manualStatusOptions(status)).toEqual(['Cancelled'])
  })

  it('ne propose rien une fois la livraison commencée ou la commande terminée', () => {
    for (const status of ['InDelivery', 'Delivered', 'Confirmed', 'Cancelled'] as const)
      expect(manualStatusOptions(status)).toEqual([])
  })
})
