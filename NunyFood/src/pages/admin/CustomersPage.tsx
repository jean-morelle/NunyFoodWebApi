import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { getCustomers, deleteCustomer } from '../../api/customers'

export default function AdminCustomersPage() {
  const queryClient = useQueryClient()
  const { data: customers = [] } = useQuery({ queryKey: ['admin-customers'], queryFn: getCustomers })

  const deleteMut = useMutation({
    mutationFn: deleteCustomer,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['admin-customers'] }),
  })

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Clients</h2>
        <p className="text-gray-500 mt-1">{customers.length} client(s) inscrits</p>
      </div>

      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              {['Nom', 'Email', 'Téléphone', 'Inscrit le', 'Actions'].map((h) => (
                <th key={h} className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">{h}</th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {customers.map((c) => (
              <tr key={c.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 text-sm font-medium text-gray-900">{c.firstName} {c.lastName}</td>
                <td className="px-6 py-4 text-sm text-gray-500">{c.email}</td>
                <td className="px-6 py-4 text-sm text-gray-500">{c.phoneNumber}</td>
                <td className="px-6 py-4 text-sm text-gray-500">{new Date(c.createdAt).toLocaleDateString('fr-FR')}</td>
                <td className="px-6 py-4">
                  <button
                    onClick={() => { if (confirm('Supprimer ce client ?')) deleteMut.mutate(c.id) }}
                    className="text-sm text-red-500 hover:underline"
                  >
                    Supprimer
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {customers.length === 0 && <div className="py-10 text-center text-gray-400 text-sm">Aucun client</div>}
      </div>
    </div>
  )
}
