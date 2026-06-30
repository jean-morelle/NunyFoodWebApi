import { useQuery } from '@tanstack/react-query'
import { useAuthStore } from '../../store/authStore'
import { getDeliveriesByAgent } from '../../api/deliveries'

export default function AgentDashboardPage() {
  const { user } = useAuthStore()
  const { data: deliveries = [], isLoading } = useQuery({
    queryKey: ['agent-deliveries', user?.id],
    queryFn: () => getDeliveriesByAgent(user!.id),
    enabled: !!user,
  })

  const enCours = deliveries.filter((d) => !d.deliveredAt)
  const terminees = deliveries.filter((d) => !!d.deliveredAt)

  if (isLoading) {
    return <div className="flex justify-center h-64 items-center"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-[#F97316]" /></div>
  }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Tableau de bord</h2>
        <p className="text-gray-500 mt-1">Bonjour, {user?.email}</p>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <StatCard label="Total livraisons" value={deliveries.length} color="orange" />
        <StatCard label="En cours" value={enCours.length} color="blue" />
        <StatCard label="Terminées" value={terminees.length} color="green" />
      </div>

      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-100">
          <h3 className="font-semibold text-gray-900">Livraisons en cours</h3>
        </div>
        {enCours.length === 0 ? (
          <div className="py-10 text-center text-gray-400 text-sm">Aucune livraison en cours</div>
        ) : (
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                {['Commande', 'Destinataire', 'Assigné le'].map((h) => (
                  <th key={h} className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">{h}</th>
                ))}
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {enCours.map((d) => (
                <tr key={d.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 text-sm font-medium text-gray-900">#{d.orderId.slice(0, 8)}</td>
                  <td className="px-6 py-4 text-sm text-gray-500">{d.receiverName}</td>
                  <td className="px-6 py-4 text-sm text-gray-500">
                    {new Date(d.createdAt).toLocaleDateString('fr-FR')}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  )
}

function StatCard({ label, value, color }: { label: string; value: number; color: 'orange' | 'blue' | 'green' }) {
  const colors = {
    orange: 'bg-orange-50 text-orange-600',
    blue: 'bg-blue-50 text-blue-600',
    green: 'bg-green-50 text-green-600',
  }
  return (
    <div className="bg-white rounded-xl border border-gray-200 p-5">
      <p className="text-sm text-gray-500">{label}</p>
      <p className={`text-3xl font-bold mt-1 ${colors[color]}`}>{value}</p>
    </div>
  )
}
