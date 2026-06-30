import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Link } from 'react-router-dom'
import { getPacks, createPack, updatePack, deletePack } from '../../api/packs'
import type { Pack } from '../../types'
import Button from '../../components/ui/Button'
import Input from '../../components/ui/Input'
import Modal from '../../components/ui/Modal'
import { ActiveBadge } from '../../components/ui/Badge'

const schema = z.object({
  name: z.string().min(1, 'Nom requis'),
  description: z.string().min(1, 'Description requise'),
  price: z.coerce.number().positive('Prix invalide'),
  imageUrl: z.string().url('URL invalide').optional().or(z.literal('')),
})
type FormData = z.infer<typeof schema>

export default function AdminPacksPage() {
  const queryClient = useQueryClient()
  const [modal, setModal] = useState<'create' | 'edit' | null>(null)
  const [editing, setEditing] = useState<Pack | null>(null)

  const { data: packs = [] } = useQuery({ queryKey: ['packs'], queryFn: getPacks })
  const inv = () => queryClient.invalidateQueries({ queryKey: ['packs'] })

  const createMut = useMutation({ mutationFn: createPack, onSuccess: () => { inv(); setModal(null) } })
  const updateMut = useMutation({
    mutationFn: (data: FormData) => updatePack(editing!.id, { ...data, imageUrl: data.imageUrl || undefined }),
    onSuccess: () => { inv(); setModal(null) },
  })
  const toggleMut = useMutation({
    mutationFn: (p: Pack) => updatePack(p.id, { isActive: !p.isActive }),
    onSuccess: inv,
  })
  const deleteMut = useMutation({ mutationFn: deletePack, onSuccess: inv })

  const { register, handleSubmit, reset, formState: { errors, isSubmitting } } = useForm<FormData>({ resolver: zodResolver(schema) })

  const openCreate = () => { reset({ name: '', description: '', price: 0, imageUrl: '' }); setEditing(null); setModal('create') }
  const openEdit = (p: Pack) => {
    reset({ name: p.name, description: p.description, price: p.price, imageUrl: p.imageUrl ?? '' })
    setEditing(p); setModal('edit')
  }
  const onSubmit = (data: FormData) =>
    modal === 'create'
      ? createMut.mutate({ ...data, imageUrl: data.imageUrl || undefined })
      : updateMut.mutate(data)

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Packs alimentaires</h2>
          <p className="text-gray-500 mt-1">{packs.length} pack(s)</p>
        </div>
        <Button onClick={openCreate}>+ Nouveau pack</Button>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {packs.map((p) => (
          <div key={p.id} className="bg-white rounded-xl border border-gray-200 overflow-hidden">
            <div className="h-32 bg-green-50 flex items-center justify-center">
              {p.imageUrl ? <img src={p.imageUrl} alt={p.name} className="h-full w-full object-cover" /> : <span className="text-5xl">🛒</span>}
            </div>
            <div className="p-4">
              <div className="flex items-center justify-between mb-2">
                <h3 className="font-semibold text-gray-900">{p.name}</h3>
                <ActiveBadge isActive={p.isActive} />
              </div>
              <p className="text-sm text-gray-500 line-clamp-2 mb-3">{p.description}</p>
              <p className="text-[#16A34A] font-bold mb-4">{p.price.toLocaleString()} FCFA</p>
              <div className="flex gap-2 flex-wrap">
                <Link to={`/admin/packs/${p.id}`} className="text-xs text-[#16A34A] hover:underline">Produits</Link>
                <button onClick={() => openEdit(p)} className="text-xs text-gray-500 hover:underline">Modifier</button>
                <button onClick={() => toggleMut.mutate(p)} className="text-xs text-gray-500 hover:underline">{p.isActive ? 'Désactiver' : 'Activer'}</button>
                <button onClick={() => { if (confirm('Supprimer ?')) deleteMut.mutate(p.id) }} className="text-xs text-red-500 hover:underline">Supprimer</button>
              </div>
            </div>
          </div>
        ))}
      </div>
      {packs.length === 0 && <div className="text-center py-16 text-gray-400 text-sm">Aucun pack</div>}

      <Modal open={modal !== null} onClose={() => setModal(null)} title={modal === 'create' ? 'Nouveau pack' : 'Modifier le pack'}>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <Input label="Nom" {...register('name')} error={errors.name?.message} />
          <Input label="Description" {...register('description')} error={errors.description?.message} />
          <Input label="Prix (FCFA)" type="number" {...register('price')} error={errors.price?.message} />
          <Input label="Image URL (optionnel)" {...register('imageUrl')} error={errors.imageUrl?.message} />
          <div className="flex gap-3 pt-2">
            <Button type="button" variant="ghost" onClick={() => setModal(null)} className="flex-1">Annuler</Button>
            <Button type="submit" loading={isSubmitting} className="flex-1">{modal === 'create' ? 'Créer' : 'Enregistrer'}</Button>
          </div>
        </form>
      </Modal>
    </div>
  )
}
