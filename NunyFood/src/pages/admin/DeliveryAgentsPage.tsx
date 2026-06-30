import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { getDeliveryAgents, createDeliveryAgent, updateDeliveryAgent, deleteDeliveryAgent } from '../../api/deliveries'
import type { DeliveryAgent } from '../../types'
import Button from '../../components/ui/Button'
import Input from '../../components/ui/Input'
import Modal from '../../components/ui/Modal'
import { ActiveBadge } from '../../components/ui/Badge'

const schema = z.object({
  fullName: z.string().min(1, 'Nom requis'),
  email: z.string().email('Email invalide'),
  phoneNumber: z.string().min(1, 'Téléphone requis'),
  zone: z.string().min(1, 'Zone requise'),
  password: z.string().min(8, 'Au moins 8 caractères'),
})
type FormData = z.infer<typeof schema>

export default function AdminDeliveryAgentsPage() {
  const queryClient = useQueryClient()
  const [modal, setModal] = useState<'create' | 'edit' | null>(null)
  const [editing, setEditing] = useState<DeliveryAgent | null>(null)

  const { data: agents = [] } = useQuery({ queryKey: ['admin-agents'], queryFn: getDeliveryAgents })
  const inv = () => queryClient.invalidateQueries({ queryKey: ['admin-agents'] })

  const createMut = useMutation({ mutationFn: createDeliveryAgent, onSuccess: () => { inv(); setModal(null) } })
  const updateMut = useMutation({
    mutationFn: (data: Partial<FormData>) => updateDeliveryAgent(editing!.id, data),
    onSuccess: () => { inv(); setModal(null) },
  })
  const toggleMut = useMutation({
    mutationFn: (a: DeliveryAgent) => updateDeliveryAgent(a.id, { isActive: !a.isActive }),
    onSuccess: inv,
  })
  const deleteMut = useMutation({ mutationFn: deleteDeliveryAgent, onSuccess: inv })

  const { register, handleSubmit, reset, formState: { errors, isSubmitting } } = useForm<FormData>({ resolver: zodResolver(schema) })

  const openCreate = () => { reset({}); setEditing(null); setModal('create') }
  const openEdit = (a: DeliveryAgent) => {
    reset({ fullName: a.fullName, email: a.email, phoneNumber: a.phoneNumber, zone: a.zone, password: '' })
    setEditing(a); setModal('edit')
  }
  const onSubmit = (data: FormData) =>
    modal === 'create' ? createMut.mutate(data) : updateMut.mutate({ fullName: data.fullName, phoneNumber: data.phoneNumber, zone: data.zone })

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Livreurs</h2>
          <p className="text-gray-500 mt-1">{agents.length} livreur(s)</p>
        </div>
        <Button onClick={openCreate}>+ Ajouter un livreur</Button>
      </div>

      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              {['Nom', 'Email', 'Téléphone', 'Zone', 'Statut', 'Actions'].map((h) => (
                <th key={h} className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">{h}</th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {agents.map((a) => (
              <tr key={a.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 text-sm font-medium text-gray-900">{a.fullName}</td>
                <td className="px-6 py-4 text-sm text-gray-500">{a.email}</td>
                <td className="px-6 py-4 text-sm text-gray-500">{a.phoneNumber}</td>
                <td className="px-6 py-4 text-sm text-gray-500">{a.zone}</td>
                <td className="px-6 py-4"><ActiveBadge isActive={a.isActive} /></td>
                <td className="px-6 py-4">
                  <div className="flex gap-3">
                    <button onClick={() => openEdit(a)} className="text-sm text-[#16A34A] hover:underline">Modifier</button>
                    <button onClick={() => toggleMut.mutate(a)} className="text-sm text-gray-500 hover:underline">{a.isActive ? 'Désactiver' : 'Activer'}</button>
                    <button onClick={() => { if (confirm('Supprimer ?')) deleteMut.mutate(a.id) }} className="text-sm text-red-500 hover:underline">Suppr.</button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {agents.length === 0 && <div className="py-10 text-center text-gray-400 text-sm">Aucun livreur</div>}
      </div>

      <Modal open={modal !== null} onClose={() => setModal(null)} title={modal === 'create' ? 'Nouveau livreur' : 'Modifier le livreur'}>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <Input label="Nom complet" {...register('fullName')} error={errors.fullName?.message} />
          <Input label="Email" type="email" {...register('email')} error={errors.email?.message} disabled={modal === 'edit'} />
          <Input label="Téléphone" {...register('phoneNumber')} error={errors.phoneNumber?.message} />
          <Input label="Zone de livraison" {...register('zone')} error={errors.zone?.message} />
          {modal === 'create' && <Input label="Mot de passe" type="password" {...register('password')} error={errors.password?.message} />}
          <div className="flex gap-3 pt-2">
            <Button type="button" variant="ghost" onClick={() => setModal(null)} className="flex-1">Annuler</Button>
            <Button type="submit" loading={isSubmitting} className="flex-1">{modal === 'create' ? 'Créer' : 'Enregistrer'}</Button>
          </div>
        </form>
      </Modal>
    </div>
  )
}
