import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { getProducts, createProduct, updateProduct, deleteProduct } from '../../api/products'
import type { Product } from '../../types'
import Button from '../../components/ui/Button'
import Input from '../../components/ui/Input'
import Modal from '../../components/ui/Modal'
import { ActiveBadge } from '../../components/ui/Badge'

const schema = z.object({
  name: z.string().min(1, 'Nom requis'),
  description: z.string().min(1, 'Description requise'),
})
type FormData = z.infer<typeof schema>

export default function AdminProductsPage() {
  const queryClient = useQueryClient()
  const [modal, setModal] = useState<'create' | 'edit' | null>(null)
  const [editing, setEditing] = useState<Product | null>(null)

  const { data: products = [] } = useQuery({ queryKey: ['products'], queryFn: getProducts })
  const inv = () => queryClient.invalidateQueries({ queryKey: ['products'] })

  const createMut = useMutation({ mutationFn: createProduct, onSuccess: () => { inv(); setModal(null) } })
  const updateMut = useMutation({
    mutationFn: (data: FormData) => updateProduct(editing!.id, data),
    onSuccess: () => { inv(); setModal(null) },
  })
  const toggleMut = useMutation({
    mutationFn: (p: Product) => updateProduct(p.id, { isActive: !p.isActive }),
    onSuccess: inv,
  })
  const deleteMut = useMutation({ mutationFn: deleteProduct, onSuccess: inv })

  const { register, handleSubmit, reset, formState: { errors, isSubmitting } } = useForm<FormData>({ resolver: zodResolver(schema) })

  const openCreate = () => { reset({}); setEditing(null); setModal('create') }
  const openEdit = (p: Product) => { reset({ name: p.name, description: p.description }); setEditing(p); setModal('edit') }
  const onSubmit = (data: FormData) => modal === 'create' ? createMut.mutate(data) : updateMut.mutate(data)

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Produits</h2>
          <p className="text-gray-500 mt-1">{products.length} produit(s)</p>
        </div>
        <Button onClick={openCreate}>+ Nouveau produit</Button>
      </div>

      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              {['Nom', 'Description', 'Statut', 'Actions'].map((h) => (
                <th key={h} className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">{h}</th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {products.map((p) => (
              <tr key={p.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 text-sm font-medium text-gray-900">{p.name}</td>
                <td className="px-6 py-4 text-sm text-gray-500 max-w-xs truncate">{p.description}</td>
                <td className="px-6 py-4"><ActiveBadge isActive={p.isActive} /></td>
                <td className="px-6 py-4">
                  <div className="flex gap-3">
                    <button onClick={() => openEdit(p)} className="text-sm text-[#16A34A] hover:underline">Modifier</button>
                    <button onClick={() => toggleMut.mutate(p)} className="text-sm text-gray-500 hover:underline">
                      {p.isActive ? 'Désactiver' : 'Activer'}
                    </button>
                    <button onClick={() => { if (confirm('Supprimer ?')) deleteMut.mutate(p.id) }} className="text-sm text-red-500 hover:underline">Suppr.</button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {products.length === 0 && <div className="py-10 text-center text-gray-400 text-sm">Aucun produit</div>}
      </div>

      <Modal open={modal !== null} onClose={() => setModal(null)} title={modal === 'create' ? 'Nouveau produit' : 'Modifier le produit'}>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <Input label="Nom" {...register('name')} error={errors.name?.message} />
          <Input label="Description" {...register('description')} error={errors.description?.message} />
          <div className="flex gap-3 pt-2">
            <Button type="button" variant="ghost" onClick={() => setModal(null)} className="flex-1">Annuler</Button>
            <Button type="submit" loading={isSubmitting} className="flex-1">{modal === 'create' ? 'Créer' : 'Enregistrer'}</Button>
          </div>
        </form>
      </Modal>
    </div>
  )
}
