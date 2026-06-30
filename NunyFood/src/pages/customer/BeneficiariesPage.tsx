import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useAuthStore } from '../../store/authStore'
import { getBeneficiaries, createBeneficiary, updateBeneficiary, deleteBeneficiary } from '../../api/beneficiaries'
import type { Beneficiary } from '../../types'
import Button from '../../components/ui/Button'
import Input from '../../components/ui/Input'
import Modal from '../../components/ui/Modal'

const schema = z.object({
  fullName: z.string().min(1, 'Nom requis'),
  phoneNumber: z.string().min(1, 'Téléphone requis'),
  address: z.string().min(1, 'Adresse requise'),
  city: z.string().min(1, 'Ville requise'),
})
type FormData = z.infer<typeof schema>

export default function BeneficiariesPage() {
  const { user } = useAuthStore()
  const queryClient = useQueryClient()
  const [modal, setModal] = useState<'create' | 'edit' | null>(null)
  const [editing, setEditing] = useState<Beneficiary | null>(null)

  const { data: beneficiaries = [] } = useQuery({
    queryKey: ['beneficiaries', user?.id],
    queryFn: () => getBeneficiaries(user!.id),
    enabled: !!user,
  })

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ['beneficiaries'] })

  const createMutation = useMutation({
    mutationFn: (data: FormData) => createBeneficiary({ ...data, customerId: user!.id }),
    onSuccess: () => { invalidate(); setModal(null) },
  })

  const updateMutation = useMutation({
    mutationFn: (data: FormData) => updateBeneficiary(editing!.id, data),
    onSuccess: () => { invalidate(); setModal(null); setEditing(null) },
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteBeneficiary(id),
    onSuccess: invalidate,
  })

  const { register, handleSubmit, reset, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(schema),
  })

  const openCreate = () => { reset({}); setEditing(null); setModal('create') }
  const openEdit = (b: Beneficiary) => {
    reset({ fullName: b.fullName, phoneNumber: b.phoneNumber, address: b.address, city: b.city })
    setEditing(b)
    setModal('edit')
  }

  const onSubmit = (data: FormData) => {
    if (modal === 'create') createMutation.mutate(data)
    else updateMutation.mutate(data)
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Mes bénéficiaires</h2>
          <p className="text-gray-500 mt-1">Personnes qui reçoivent vos packs au Togo</p>
        </div>
        <Button onClick={openCreate}>+ Ajouter</Button>
      </div>

      {beneficiaries.length === 0 ? (
        <div className="bg-white rounded-xl border border-gray-200 py-16 text-center">
          <p className="text-gray-400 text-sm">Aucun bénéficiaire. Ajoutez-en un pour passer une commande.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          {beneficiaries.map((b) => (
            <div key={b.id} className="bg-white rounded-xl border border-gray-200 p-5">
              <div className="flex items-start justify-between">
                <div>
                  <p className="font-semibold text-gray-900">{b.fullName}</p>
                  <p className="text-sm text-gray-500 mt-0.5">{b.phoneNumber}</p>
                  <p className="text-sm text-gray-500">{b.address}, {b.city}</p>
                </div>
                <div className="flex gap-2">
                  <button onClick={() => openEdit(b)} className="text-sm text-[#16A34A] hover:underline">
                    Modifier
                  </button>
                  <button
                    onClick={() => { if (confirm('Supprimer ce bénéficiaire ?')) deleteMutation.mutate(b.id) }}
                    className="text-sm text-red-500 hover:underline"
                  >
                    Supprimer
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      <Modal
        open={modal !== null}
        onClose={() => { setModal(null); setEditing(null) }}
        title={modal === 'create' ? 'Ajouter un bénéficiaire' : 'Modifier le bénéficiaire'}
      >
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <Input label="Nom complet" {...register('fullName')} error={errors.fullName?.message} />
          <Input label="Téléphone" {...register('phoneNumber')} error={errors.phoneNumber?.message} />
          <Input label="Adresse" {...register('address')} error={errors.address?.message} />
          <Input label="Ville" {...register('city')} error={errors.city?.message} />
          <div className="flex gap-3 pt-2">
            <Button type="button" variant="ghost" onClick={() => setModal(null)} className="flex-1">
              Annuler
            </Button>
            <Button type="submit" loading={isSubmitting} className="flex-1">
              {modal === 'create' ? 'Ajouter' : 'Enregistrer'}
            </Button>
          </div>
        </form>
      </Modal>
    </div>
  )
}
