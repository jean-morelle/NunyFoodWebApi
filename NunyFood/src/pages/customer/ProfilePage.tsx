import { useQuery, useMutation } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useState } from 'react'
import { useAuthStore } from '../../store/authStore'
import { getCustomer, updateCustomer, changePassword } from '../../api/customers'
import Input from '../../components/ui/Input'
import Button from '../../components/ui/Button'

const profileSchema = z.object({
  firstName: z.string().min(1),
  lastName: z.string().min(1),
  phoneNumber: z.string().min(1),
})

const passwordSchema = z.object({
  currentPassword: z.string().min(1),
  newPassword: z.string().min(8, 'Au moins 8 caractères'),
}).refine((d) => d.newPassword !== d.currentPassword, {
  message: "Le nouveau mot de passe doit être différent",
  path: ['newPassword'],
})

type ProfileData = z.infer<typeof profileSchema>
type PasswordData = z.infer<typeof passwordSchema>

export default function ProfilePage() {
  const { user } = useAuthStore()
  const [profileSuccess, setProfileSuccess] = useState(false)
  const [passwordMsg, setPasswordMsg] = useState('')

  const { data: customer } = useQuery({
    queryKey: ['customer', user?.id],
    queryFn: () => getCustomer(user!.id),
    enabled: !!user,
  })

  const profileForm = useForm<ProfileData>({
    resolver: zodResolver(profileSchema),
    values: { firstName: customer?.firstName ?? '', lastName: customer?.lastName ?? '', phoneNumber: customer?.phoneNumber ?? '' },
  })

  const passwordForm = useForm<PasswordData>({ resolver: zodResolver(passwordSchema) })

  const updateMutation = useMutation({
    mutationFn: (data: ProfileData) => updateCustomer(user!.id, data),
    onSuccess: () => { setProfileSuccess(true); setTimeout(() => setProfileSuccess(false), 3000) },
  })

  const passwordMutation = useMutation({
    mutationFn: (data: PasswordData) => changePassword(user!.id, data),
    onSuccess: () => { setPasswordMsg('Mot de passe modifié avec succès.'); passwordForm.reset() },
    onError: () => setPasswordMsg('Mot de passe actuel incorrect.'),
  })

  return (
    <div className="max-w-xl space-y-8">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Mon profil</h2>
        <p className="text-gray-500 mt-1">{customer?.email}</p>
      </div>

      {/* Profile form */}
      <div className="bg-white rounded-xl border border-gray-200 p-6">
        <h3 className="font-semibold text-gray-900 mb-4">Informations personnelles</h3>
        <form onSubmit={profileForm.handleSubmit((d) => updateMutation.mutate(d))} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <Input label="Prénom" {...profileForm.register('firstName')} error={profileForm.formState.errors.firstName?.message} />
            <Input label="Nom" {...profileForm.register('lastName')} error={profileForm.formState.errors.lastName?.message} />
          </div>
          <Input label="Téléphone" {...profileForm.register('phoneNumber')} error={profileForm.formState.errors.phoneNumber?.message} />
          {profileSuccess && <p className="text-sm text-[#16A34A]">Profil mis à jour !</p>}
          <Button type="submit" loading={updateMutation.isPending}>Enregistrer</Button>
        </form>
      </div>

      {/* Password form */}
      <div className="bg-white rounded-xl border border-gray-200 p-6">
        <h3 className="font-semibold text-gray-900 mb-4">Changer le mot de passe</h3>
        <form onSubmit={passwordForm.handleSubmit((d) => { setPasswordMsg(''); passwordMutation.mutate(d) })} className="space-y-4">
          <Input label="Mot de passe actuel" type="password" {...passwordForm.register('currentPassword')} error={passwordForm.formState.errors.currentPassword?.message} />
          <Input label="Nouveau mot de passe" type="password" {...passwordForm.register('newPassword')} error={passwordForm.formState.errors.newPassword?.message} />
          {passwordMsg && (
            <p className={`text-sm ${passwordMsg.includes('succès') ? 'text-[#16A34A]' : 'text-red-600'}`}>{passwordMsg}</p>
          )}
          <Button type="submit" loading={passwordMutation.isPending}>Modifier le mot de passe</Button>
        </form>
      </div>
    </div>
  )
}
