import { useEffect, useState, type ChangeEvent, type FormEvent } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { confirmDelivery } from '../../api/deliveries'
import { apiErrorMessage } from '../../api/errors'
import type { Delivery } from '../../types'
import Modal from '../ui/Modal'
import Input from '../ui/Input'
import Button from '../ui/Button'
import SignaturePad from './SignaturePad'

const MAX_PHOTO_BYTES = 5 * 1024 * 1024

type Position = { latitude: number; longitude: number }
type GpsState = { status: 'pending' } | { status: 'ok'; position: Position } | { status: 'unavailable'; reason: string }

interface Props {
  delivery: Delivery
  onClose: () => void
}

export default function ConfirmDeliveryModal({ delivery, onClose }: Props) {
  const queryClient = useQueryClient()
  const [receiverName, setReceiverName] = useState(delivery.receiverName)
  const [photo, setPhoto] = useState<File | null>(null)
  const [photoPreview, setPhotoPreview] = useState<string | null>(null)
  const [signature, setSignature] = useState<Blob | null>(null)
  const [gps, setGps] = useState<GpsState>({ status: 'pending' })
  const [error, setError] = useState('')

  // La position est demandée dès l'ouverture ; elle reste facultative si le livreur la refuse.
  useEffect(() => {
    if (!navigator.geolocation) {
      setGps({ status: 'unavailable', reason: 'Géolocalisation non disponible sur cet appareil.' })
      return
    }
    navigator.geolocation.getCurrentPosition(
      (pos) => setGps({ status: 'ok', position: { latitude: pos.coords.latitude, longitude: pos.coords.longitude } }),
      () => setGps({ status: 'unavailable', reason: 'Position non autorisée ou introuvable.' }),
      { enableHighAccuracy: true, timeout: 15000 },
    )
  }, [])

  useEffect(() => () => { if (photoPreview) URL.revokeObjectURL(photoPreview) }, [photoPreview])

  const onPhotoChange = (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0] ?? null
    setError('')
    if (file && file.size > MAX_PHOTO_BYTES) {
      setError('La photo dépasse 5 Mo.')
      e.target.value = ''
      return
    }
    setPhoto(file)
    setPhotoPreview(file ? URL.createObjectURL(file) : null)
  }

  const mutation = useMutation({
    mutationFn: () =>
      confirmDelivery(delivery.id, {
        receiverName: receiverName.trim(),
        photo: photo!,
        signature: signature!,
        ...(gps.status === 'ok' ? gps.position : {}),
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['agent-deliveries'] })
      onClose()
    },
    onError: (err) => setError(apiErrorMessage(err, 'La confirmation a échoué. Réessayez.')),
  })

  const onSubmit = (e: FormEvent) => {
    e.preventDefault()
    setError('')
    if (!receiverName.trim()) return setError('Indiquez le nom de la personne qui a reçu le colis.')
    if (!photo) return setError('Ajoutez une photo de preuve.')
    if (!signature) return setError('Faites signer le receveur.')
    mutation.mutate()
  }

  return (
    <Modal open onClose={onClose} title={`Confirmer la livraison #${delivery.orderId.slice(0, 8)}`}>
      <form onSubmit={onSubmit} className="space-y-5">
        <Input label="Nom du receveur" value={receiverName} onChange={(e) => setReceiverName(e.target.value)} />

        <div className="space-y-2">
          <label className="text-sm font-medium text-gray-700 dark:text-gray-300 block">Photo de preuve</label>
          {/* capture : ouvre directement l'appareil photo sur mobile */}
          <input
            type="file"
            accept="image/jpeg,image/png,image/webp"
            capture="environment"
            onChange={onPhotoChange}
            className="block w-full text-sm text-gray-600 file:mr-3 file:rounded-lg file:border-0 file:bg-green-50 file:px-4 file:py-2 file:text-sm file:font-medium file:text-[#16A34A] hover:file:bg-green-100"
          />
          {photoPreview && <img src={photoPreview} alt="Aperçu de la photo" className="h-40 w-full rounded-lg object-cover" />}
        </div>

        <div className="space-y-2">
          <label className="text-sm font-medium text-gray-700 dark:text-gray-300 block">Signature du receveur</label>
          <SignaturePad onChange={setSignature} />
        </div>

        <p className="text-xs text-gray-500">
          {gps.status === 'pending' && '📍 Récupération de la position…'}
          {gps.status === 'ok' && `📍 Position enregistrée (${gps.position.latitude.toFixed(5)}, ${gps.position.longitude.toFixed(5)})`}
          {gps.status === 'unavailable' && `📍 ${gps.reason} La livraison sera confirmée sans position.`}
        </p>

        {error && (
          <div className="bg-red-50 border border-red-200 text-red-700 text-sm rounded-lg px-4 py-3">{error}</div>
        )}

        <div className="flex gap-3 pt-2">
          <Button type="button" variant="ghost" onClick={onClose} className="flex-1">Annuler</Button>
          <Button type="submit" loading={mutation.isPending} disabled={gps.status === 'pending'} className="flex-1">
            Confirmer la livraison
          </Button>
        </div>
      </form>
    </Modal>
  )
}
