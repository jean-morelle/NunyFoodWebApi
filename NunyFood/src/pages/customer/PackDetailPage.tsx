import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useParams, useNavigate } from 'react-router-dom'
import { useState } from 'react'
import { getPack, getPackProducts } from '../../api/packs'
import { getBeneficiaries } from '../../api/beneficiaries'
import { createOrder } from '../../api/orders'
import { useAuthStore } from '../../store/authStore'
import Button from '../../components/ui/Button'

export default function PackDetailPage() {
  const { id } = useParams<{ id: string }>()
  const { user } = useAuthStore()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [selectedBeneficiary, setSelectedBeneficiary] = useState('')
  const [error, setError] = useState('')

  const { data: pack, isLoading: packLoading } = useQuery({
    queryKey: ['packs', id],
    queryFn: () => getPack(id!),
    enabled: !!id,
  })

  const { data: packProducts = [] } = useQuery({
    queryKey: ['pack-products', id],
    queryFn: () => getPackProducts(id!),
    enabled: !!id,
  })

  const { data: beneficiaries = [] } = useQuery({
    queryKey: ['beneficiaries', user?.id],
    queryFn: () => getBeneficiaries(user!.id),
    enabled: !!user,
  })

  const orderMutation = useMutation({
    mutationFn: () =>
      createOrder({ customerId: user!.id, beneficiaryId: selectedBeneficiary, packId: id! }),
    onSuccess: (order) => {
      queryClient.invalidateQueries({ queryKey: ['orders'] })
      navigate(`/customer/orders/${order.id}`)
    },
    onError: () => setError("Erreur lors de la commande. Veuillez réessayer."),
  })

  const handleOrder = () => {
    if (!selectedBeneficiary) { setError("Sélectionnez un bénéficiaire."); return }
    setError('')
    orderMutation.mutate()
  }

  if (packLoading || !pack) {
    return <div className="flex justify-center items-center h-64"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-[#16A34A]" /></div>
  }

  return (
    <div className="max-w-2xl space-y-6">
      <button onClick={() => navigate(-1)} className="text-sm text-[#16A34A] hover:underline flex items-center gap-1">
        ← Retour aux packs
      </button>

      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        <div className="h-48 bg-gradient-to-br from-green-50 to-green-100 flex items-center justify-center">
          {pack.imageUrl
            ? <img src={pack.imageUrl} alt={pack.name} className="h-full w-full object-cover" />
            : <span className="text-7xl">🛒</span>
          }
        </div>
        <div className="p-6">
          <h2 className="text-2xl font-bold text-gray-900">{pack.name}</h2>
          <p className="text-gray-500 mt-2">{pack.description}</p>
          <p className="text-3xl font-bold text-[#16A34A] mt-4">{pack.price.toLocaleString()} FCFA</p>
        </div>
      </div>

      {packProducts.length > 0 && (
        <div className="bg-white rounded-xl border border-gray-200 p-6">
          <h3 className="font-semibold text-gray-900 mb-4">Contenu du pack</h3>
          <ul className="space-y-2">
            {packProducts.map((pp) => (
              <li key={pp.productId} className="flex items-center gap-2 text-sm text-gray-700">
                <span className="text-[#16A34A] font-medium">✓</span>
                Quantité : {pp.quantity}
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Commander */}
      <div className="bg-white rounded-xl border border-gray-200 p-6 space-y-4">
        <h3 className="font-semibold text-gray-900">Passer la commande</h3>

        {beneficiaries.length === 0 ? (
          <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4 text-sm text-yellow-800">
            Vous n'avez pas encore de bénéficiaire.{' '}
            <a href="/customer/beneficiaries" className="font-medium underline">En ajouter un</a>
          </div>
        ) : (
          <>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Bénéficiaire</label>
              <select
                value={selectedBeneficiary}
                onChange={(e) => setSelectedBeneficiary(e.target.value)}
                className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[#16A34A]"
              >
                <option value="">-- Choisir un bénéficiaire --</option>
                {beneficiaries.map((b) => (
                  <option key={b.id} value={b.id}>{b.fullName} — {b.city}</option>
                ))}
              </select>
            </div>

            {error && <p className="text-sm text-red-600">{error}</p>}

            <Button
              onClick={handleOrder}
              loading={orderMutation.isPending}
              size="lg"
              className="w-full"
            >
              Commander — {pack.price.toLocaleString()} FCFA
            </Button>
          </>
        )}
      </div>
    </div>
  )
}
