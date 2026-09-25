import { beforeEach, describe, expect, it, vi } from 'vitest'
import { screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import OrderDetailPage from './OrderDetailPage'
import { cancelOrder, confirmReception, getOrder, getOrderStatusHistory } from '../../api/orders'
import { getDeliveryByOrder } from '../../api/deliveries'
import { apiError, renderWithProviders } from '../../test/utils'
import type { Order, OrderStatus } from '../../types'

vi.mock('../../api/orders', () => ({
  getOrder: vi.fn(), getOrderStatusHistory: vi.fn(), cancelOrder: vi.fn(), confirmReception: vi.fn(),
}))
vi.mock('../../api/payments', () => ({ createPayment: vi.fn() }))
vi.mock('../../api/deliveries', () => ({ getDeliveryByOrder: vi.fn(), getDeliveryProof: vi.fn() }))

const order = (status: OrderStatus): Order => ({
  id: 'o1-0000-0000', customerId: 'c1', beneficiaryId: 'b1', packId: 'p1',
  amount: 15000, status, createdAt: '2026-09-25T10:00:00Z',
})

const renderPage = () => renderWithProviders(<OrderDetailPage />, { route: '/orders/o1', path: '/orders/:id' })

describe('OrderDetailPage', () => {
  beforeEach(() => {
    vi.mocked(getOrderStatusHistory).mockResolvedValue([])
  })

  it('commande non payée : paiement et annulation (après confirmation)', async () => {
    vi.mocked(getOrder).mockResolvedValue(order('Created'))
    vi.mocked(cancelOrder).mockResolvedValue(order('Cancelled'))
    const confirmSpy = vi.spyOn(window, 'confirm').mockReturnValue(true)
    const user = userEvent.setup()
    renderPage()

    expect(await screen.findByText('Payer cette commande')).toBeInTheDocument()
    expect(screen.queryByRole('button', { name: 'Confirmer la réception' })).not.toBeInTheDocument()

    await user.click(screen.getByRole('button', { name: 'Annuler la commande' }))

    expect(confirmSpy).toHaveBeenCalled()
    await waitFor(() => expect(cancelOrder).toHaveBeenCalledWith('o1'))
  })

  it('n’annule pas si le client renonce dans la confirmation', async () => {
    vi.mocked(getOrder).mockResolvedValue(order('Created'))
    vi.spyOn(window, 'confirm').mockReturnValue(false)
    const user = userEvent.setup()
    renderPage()

    await user.click(await screen.findByRole('button', { name: 'Annuler la commande' }))

    expect(cancelOrder).not.toHaveBeenCalled()
  })

  it('affiche le refus de l’API', async () => {
    vi.mocked(getOrder).mockResolvedValue(order('Created'))
    vi.mocked(cancelOrder).mockRejectedValue(apiError(400, { title: 'Une commande payée ne peut plus être annulée depuis votre espace. Contactez le support.' }))
    vi.spyOn(window, 'confirm').mockReturnValue(true)
    const user = userEvent.setup()
    renderPage()

    await user.click(await screen.findByRole('button', { name: 'Annuler la commande' }))

    expect(await screen.findByText(/contactez le support/i)).toBeInTheDocument()
  })

  it('commande livrée : confirmation de réception', async () => {
    vi.mocked(getOrder).mockResolvedValue(order('Delivered'))
    vi.mocked(confirmReception).mockResolvedValue(order('Confirmed'))
    const user = userEvent.setup()
    renderPage()

    await user.click(await screen.findByRole('button', { name: 'Confirmer la réception' }))

    await waitFor(() => expect(confirmReception).toHaveBeenCalledWith('o1'))
    expect(screen.queryByText('Payer cette commande')).not.toBeInTheDocument()
  })

  it('commande livrée : le client peut voir la preuve de livraison', async () => {
    vi.mocked(getOrder).mockResolvedValue(order('Confirmed'))
    vi.mocked(getDeliveryByOrder).mockResolvedValue({
      id: 'd1', orderId: 'o1-0000-0000', deliveryAgentId: 'a1', receiverName: 'Mama', photoUrl: null, signatureUrl: null,
      latitude: null, longitude: null, deliveredAt: '2026-09-25T12:00:00Z', createdAt: '2026-09-25T10:00:00Z',
    })
    const user = userEvent.setup()
    renderPage()

    await user.click(await screen.findByRole('button', { name: /voir la preuve de livraison/i }))

    expect(await screen.findByText('Mama')).toBeInTheDocument()
    expect(getDeliveryByOrder).toHaveBeenCalledWith('o1-0000-0000')
  })

  it('commande payée en cours : aucune action client', async () => {
    vi.mocked(getOrder).mockResolvedValue(order('Paid'))
    renderPage()

    expect(await screen.findByText(/commande #o1-0000-/i)).toBeInTheDocument()
    expect(screen.queryByRole('button', { name: 'Annuler la commande' })).not.toBeInTheDocument()
    expect(screen.queryByRole('button', { name: 'Confirmer la réception' })).not.toBeInTheDocument()
  })
})
