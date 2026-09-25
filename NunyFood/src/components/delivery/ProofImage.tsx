import { useEffect, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { getDeliveryProof } from '../../api/deliveries'

interface Props {
  deliveryId: string
  kind: 'photo' | 'signature'
  alt: string
  className?: string
  /** Rend l'image cliquable pour l'ouvrir en grand dans un nouvel onglet. */
  openable?: boolean
}

/** Image de preuve chargée avec le token de connexion, puis affichée depuis une URL locale (blob:). */
export default function ProofImage({ deliveryId, kind, alt, className, openable }: Props) {
  const { data: blob, isLoading, isError } = useQuery({
    queryKey: ['delivery-proof', deliveryId, kind],
    queryFn: () => getDeliveryProof(deliveryId, kind),
    staleTime: Infinity,
  })
  const [src, setSrc] = useState<string | null>(null)

  useEffect(() => {
    if (!blob) return
    const url = URL.createObjectURL(blob)
    setSrc(url)
    return () => URL.revokeObjectURL(url)
  }, [blob])

  if (isLoading) return <div className={`animate-pulse rounded-lg bg-gray-100 ${className ?? ''}`} aria-label={`Chargement : ${alt}`} />
  if (isError || !src) return <p className="text-sm text-gray-400">Image indisponible</p>

  const img = <img src={src} alt={alt} className={className} />
  return openable ? <a href={src} target="_blank" rel="noreferrer">{img}</a> : img
}
