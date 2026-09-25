import { describe, expect, it, vi } from 'vitest'
import { screen, waitFor } from '@testing-library/react'
import DeliveryProofModal from './DeliveryProofModal'
import { getDeliveryByOrder, getDeliveryProof } from '../../api/deliveries'
import { apiError, renderWithProviders } from '../../test/utils'
import type { Delivery } from '../../types'

vi.mock('../../api/deliveries', () => ({ getDeliveryByOrder: vi.fn(), getDeliveryProof: vi.fn() }))

const delivered: Delivery = {
  id: 'd1', orderId: 'o1-0000-0000', deliveryAgentId: 'a1', receiverName: 'Mama',
  photoUrl: '/api/deliveries/d1/photo', signatureUrl: '/api/deliveries/d1/signature',
  latitude: 6.1319, longitude: 1.2228, deliveredAt: '2026-09-25T12:00:00Z', createdAt: '2026-09-25T10:00:00Z',
}

describe('DeliveryProofModal', () => {
  it('charge photo et signature via l’API authentifiée', async () => {
    vi.mocked(getDeliveryByOrder).mockResolvedValue(delivered)
    vi.mocked(getDeliveryProof).mockResolvedValue(new Blob(['img'], { type: 'image/png' }))

    renderWithProviders(<DeliveryProofModal orderId="o1-0000-0000" onClose={vi.fn()} />)

    expect(await screen.findByAltText('Photo de preuve')).toHaveAttribute('src', 'blob:apercu')
    expect(await screen.findByAltText('Signature du receveur')).toHaveAttribute('src', 'blob:apercu')
    expect(getDeliveryProof).toHaveBeenCalledWith('d1', 'photo')
    expect(getDeliveryProof).toHaveBeenCalledWith('d1', 'signature')
    expect(screen.getByText('Mama')).toBeInTheDocument()
    expect(screen.getByRole('link', { name: 'Voir sur la carte' })).toHaveAttribute('href', expect.stringContaining('mlat=6.1319'))
  })

  it('reste lisible si un fichier est inaccessible', async () => {
    vi.mocked(getDeliveryByOrder).mockResolvedValue(delivered)
    vi.mocked(getDeliveryProof).mockRejectedValue(apiError(404))

    renderWithProviders(<DeliveryProofModal orderId="o1-0000-0000" onClose={vi.fn()} />)

    // Les deux requêtes échouent indépendamment : on attend que les deux images soient remplacées.
    await waitFor(() => expect(screen.getAllByText('Image indisponible')).toHaveLength(2))
  })

  it('n’interroge pas l’API pour une preuve absente', async () => {
    vi.mocked(getDeliveryByOrder).mockResolvedValue({ ...delivered, photoUrl: null, signatureUrl: null, latitude: null, longitude: null })

    renderWithProviders(<DeliveryProofModal orderId="o1-0000-0000" onClose={vi.fn()} />)

    expect(await screen.findByText('Aucune photo')).toBeInTheDocument()
    expect(screen.getByText('Aucune signature')).toBeInTheDocument()
    expect(screen.getByText('Non enregistrée')).toBeInTheDocument()
    expect(getDeliveryProof).not.toHaveBeenCalled()
  })
})
