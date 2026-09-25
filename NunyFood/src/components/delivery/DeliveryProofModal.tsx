import { useQuery } from '@tanstack/react-query'
import { getDeliveryByOrder } from '../../api/deliveries'
import Modal from '../ui/Modal'

interface Props {
  orderId: string
  onClose: () => void
}

export default function DeliveryProofModal({ orderId, onClose }: Props) {
  const { data: delivery, isLoading, isError } = useQuery({
    queryKey: ['delivery-by-order', orderId],
    queryFn: () => getDeliveryByOrder(orderId),
  })

  return (
    <Modal open onClose={onClose} title={`Preuve de livraison #${orderId.slice(0, 8)}`}>
      {isLoading && <p className="text-sm text-gray-500">Chargement…</p>}
      {isError && <p className="text-sm text-red-600">Impossible de charger la livraison.</p>}
      {delivery && (
        <div className="space-y-5 text-sm">
          <dl className="grid grid-cols-2 gap-x-4 gap-y-2">
            <dt className="text-gray-500">Receveur</dt>
            <dd className="font-medium text-gray-900 dark:text-gray-100">{delivery.receiverName}</dd>
            <dt className="text-gray-500">Livré le</dt>
            <dd className="font-medium text-gray-900 dark:text-gray-100">
              {delivery.deliveredAt ? new Date(delivery.deliveredAt).toLocaleString('fr-FR') : '—'}
            </dd>
            <dt className="text-gray-500">Position</dt>
            <dd className="font-medium text-gray-900 dark:text-gray-100">
              {delivery.latitude != null && delivery.longitude != null ? (
                <a
                  href={`https://www.openstreetmap.org/?mlat=${delivery.latitude}&mlon=${delivery.longitude}#map=17/${delivery.latitude}/${delivery.longitude}`}
                  target="_blank"
                  rel="noreferrer"
                  className="text-[#16A34A] hover:underline"
                >
                  Voir sur la carte
                </a>
              ) : (
                'Non enregistrée'
              )}
            </dd>
          </dl>

          <div className="space-y-1">
            <p className="text-gray-500">Photo</p>
            {delivery.photoUrl ? (
              <a href={delivery.photoUrl} target="_blank" rel="noreferrer">
                <img src={delivery.photoUrl} alt="Photo de preuve" className="max-h-64 w-full rounded-lg object-contain bg-gray-50" />
              </a>
            ) : (
              <p className="text-gray-400">Aucune photo</p>
            )}
          </div>

          <div className="space-y-1">
            <p className="text-gray-500">Signature</p>
            {delivery.signatureUrl ? (
              <img src={delivery.signatureUrl} alt="Signature du receveur" className="h-32 w-full rounded-lg border border-gray-200 object-contain bg-white" />
            ) : (
              <p className="text-gray-400">Aucune signature</p>
            )}
          </div>
        </div>
      )}
    </Modal>
  )
}
