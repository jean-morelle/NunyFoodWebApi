import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { getPacks } from '../../api/packs'

export default function CustomerPacksPage() {
  const { data: packs = [], isLoading } = useQuery({
    queryKey: ['packs'],
    queryFn: getPacks,
  })

  const activePacks = packs.filter((p) => p.isActive)

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-[#16A34A]" />
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Packs alimentaires</h2>
        <p className="text-gray-500 mt-1">Choisissez le pack adapté à votre famille</p>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
        {activePacks.map((pack) => (
          <div
            key={pack.id}
            className="bg-white rounded-xl border border-gray-200 overflow-hidden hover:shadow-md transition-shadow"
          >
            <div className="h-40 bg-gradient-to-br from-green-50 to-green-100 flex items-center justify-center">
              {pack.imageUrl ? (
                <img src={pack.imageUrl} alt={pack.name} className="h-full w-full object-cover" />
              ) : (
                <span className="text-6xl">🛒</span>
              )}
            </div>
            <div className="p-5">
              <h3 className="text-base font-semibold text-gray-900">{pack.name}</h3>
              <p className="text-sm text-gray-500 mt-1 line-clamp-2">{pack.description}</p>
              <div className="flex items-center justify-between mt-4">
                <span className="text-xl font-bold text-[#16A34A]">{pack.price.toLocaleString()} FCFA</span>
                <Link
                  to={`/customer/packs/${pack.id}`}
                  className="bg-[#16A34A] text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-[#15803D] transition-colors"
                >
                  Voir le pack
                </Link>
              </div>
            </div>
          </div>
        ))}
      </div>

      {activePacks.length === 0 && (
        <div className="text-center py-16 text-gray-400">Aucun pack disponible pour le moment.</div>
      )}
    </div>
  )
}
