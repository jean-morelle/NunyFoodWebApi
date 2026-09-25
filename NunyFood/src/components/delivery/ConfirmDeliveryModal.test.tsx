import { beforeEach, describe, expect, it, vi } from 'vitest'
import { fireEvent, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import ConfirmDeliveryModal from './ConfirmDeliveryModal'
import { confirmDelivery } from '../../api/deliveries'
import { renderWithProviders } from '../../test/utils'
import type { Delivery } from '../../types'

vi.mock('../../api/deliveries', () => ({ confirmDelivery: vi.fn() }))

const delivery: Delivery = {
  id: 'd1', orderId: 'o1-0000-0000', deliveryAgentId: 'a1', receiverName: 'Mama',
  photoUrl: null, signatureUrl: null, latitude: null, longitude: null, deliveredAt: null, createdAt: '2026-09-25T10:00:00Z',
}

function mockGeolocation(result: 'ok' | 'denied') {
  Object.defineProperty(navigator, 'geolocation', {
    configurable: true,
    value: {
      getCurrentPosition: (success: PositionCallback, error: PositionErrorCallback) =>
        result === 'ok'
          ? success({ coords: { latitude: 6.1319, longitude: 1.2228 } } as GeolocationPosition)
          : error({ code: 1 } as GeolocationPositionError),
    },
  })
}

const photo = new File(['jpeg'], 'preuve.jpg', { type: 'image/jpeg' })
const sign = () => {
  const canvas = document.querySelector('canvas')!
  fireEvent.pointerDown(canvas, { clientX: 10, clientY: 10 })
  fireEvent.pointerUp(canvas)
}

describe('ConfirmDeliveryModal', () => {
  beforeEach(() => {
    mockGeolocation('ok')
    vi.mocked(confirmDelivery).mockResolvedValue({ ...delivery, deliveredAt: '2026-09-25T12:00:00Z' })
  })

  it('exige la photo puis la signature avant d’envoyer', async () => {
    const user = userEvent.setup()
    renderWithProviders(<ConfirmDeliveryModal delivery={delivery} onClose={vi.fn()} />)
    const submit = () => user.click(screen.getByRole('button', { name: 'Confirmer la livraison' }))

    await submit()
    expect(screen.getByText('Ajoutez une photo de preuve.')).toBeInTheDocument()

    await user.upload(document.querySelector<HTMLInputElement>('input[type="file"]')!, photo)
    await submit()
    expect(screen.getByText('Faites signer le receveur.')).toBeInTheDocument()

    expect(confirmDelivery).not.toHaveBeenCalled()
  })

  it('envoie photo, signature et position GPS puis ferme la fenêtre', async () => {
    const onClose = vi.fn()
    const user = userEvent.setup()
    renderWithProviders(<ConfirmDeliveryModal delivery={delivery} onClose={onClose} />)

    expect(screen.getByText(/position enregistrée \(6\.13190, 1\.22280\)/i)).toBeInTheDocument()
    await user.upload(document.querySelector<HTMLInputElement>('input[type="file"]')!, photo)
    sign()
    await user.click(screen.getByRole('button', { name: 'Confirmer la livraison' }))

    await waitFor(() => expect(onClose).toHaveBeenCalled())
    expect(confirmDelivery).toHaveBeenCalledWith('d1', expect.objectContaining({
      receiverName: 'Mama', photo, latitude: 6.1319, longitude: 1.2228,
    }))
    expect(vi.mocked(confirmDelivery).mock.calls[0][1].signature).toBeInstanceOf(Blob)
  })

  it('confirme sans position si le livreur refuse la géolocalisation', async () => {
    mockGeolocation('denied')
    const user = userEvent.setup()
    renderWithProviders(<ConfirmDeliveryModal delivery={delivery} onClose={vi.fn()} />)

    expect(screen.getByText(/sera confirmée sans position/i)).toBeInTheDocument()
    await user.upload(document.querySelector<HTMLInputElement>('input[type="file"]')!, photo)
    sign()
    await user.click(screen.getByRole('button', { name: 'Confirmer la livraison' }))

    await waitFor(() => expect(confirmDelivery).toHaveBeenCalled())
    expect(vi.mocked(confirmDelivery).mock.calls[0][1]).not.toHaveProperty('latitude')
  })

  it('refuse une photo de plus de 5 Mo', async () => {
    const user = userEvent.setup()
    renderWithProviders(<ConfirmDeliveryModal delivery={delivery} onClose={vi.fn()} />)
    const big = new File([new Uint8Array(5 * 1024 * 1024 + 1)], 'grosse.jpg', { type: 'image/jpeg' })

    await user.upload(document.querySelector<HTMLInputElement>('input[type="file"]')!, big)

    expect(screen.getByText('La photo dépasse 5 Mo.')).toBeInTheDocument()
  })
})
