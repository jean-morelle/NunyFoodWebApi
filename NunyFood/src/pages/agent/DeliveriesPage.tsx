import { useQuery } from '@tanstack/react-query'
import { useAuthStore } from '../../store/authStore'
import { getDeliveriesByAgent } from '../../api/deliveries'

export default function AgentDeliveriesPage() {
  const { user } = useAuthStore()
  const { data: deliveries = [], isLoading } = useQuery({
    queryKey: ['agent-deliveries', user?.id],
    queryFn: () => getDeliveriesByAgent(user!.id),
    enabled: !!user,
  })

  if (isLoading) {
    return <div className="flex justify-center h-64 items-center"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-[#F97316]" /></div>
  }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Mes livraisons</h2>
        <p className="text-gray-500 mt-1">{deliveries.length} livraison(s) assignée(s)</p>
      </div>

      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              {['Commande', 'Destinataire', 'Livré le', 'Statut'].map((h) => (
                <th key={h} className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">{h}</th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {deliveries.map((d) => (
              <tr key={d.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 text-sm font-medium text-gray-900">#{d.orderId.slice(0, 8)}</td>
                <td className="px-6 py-4 text-sm text-gray-500">{d.receiverName}</td>
                <td className="px-6 py-4 text-sm text-gray-500">
                  {d.deliveredAt ? new Date(d.deliveredAt).toLocaleDateString('fr-FR') : '—'}
                </td>
                <td className="px-6 py-4">
                  <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                    d.deliveredAt ? 'bg-green-100 text-green-800' : 'bg-orange-100 text-orange-800'
                  }`}>
                    {d.deliveredAt ? 'Livré' : 'En cours'}
                  </span>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {deliveries.length === 0 && (
          <div className="py-10 text-center text-gray-400 text-sm">Aucune livraison assignée</div>
        )}
      </div>
    </div>
  )
}
