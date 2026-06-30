import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuthStore } from '../../store/authStore'
import { createCustomer } from '../../api/customers'
import { loginCustomer } from '../../api/auth'
import Input from '../../components/ui/Input'
import Button from '../../components/ui/Button'

const schema = z
  .object({
    firstName: z.string().min(1, 'Prénom requis').max(100),
    lastName: z.string().min(1, 'Nom requis').max(100),
    email: z.string().email('Email invalide').max(200),
    phoneNumber: z.string().min(1, 'Téléphone requis').max(20),
    password: z.string().min(8, 'Au moins 8 caractères').max(100),
    confirmPassword: z.string(),
  })
  .refine((d) => d.password === d.confirmPassword, {
    message: 'Les mots de passe ne correspondent pas',
    path: ['confirmPassword'],
  })

type FormData = z.infer<typeof schema>

export default function RegisterPage() {
  const { setToken } = useAuthStore()
  const navigate = useNavigate()
  const [error, setError] = useState('')

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormData>({ resolver: zodResolver(schema) })

  const onSubmit = async (data: FormData) => {
    setError('')
    try {
      await createCustomer({
        firstName: data.firstName,
        lastName: data.lastName,
        email: data.email,
        phoneNumber: data.phoneNumber,
        password: data.password,
      })
      const token = await loginCustomer(data.email, data.password)
      setToken(token.token)
      navigate('/customer/dashboard')
    } catch {
      setError("Une erreur est survenue. Cet email est peut-être déjà utilisé.")
    }
  }

  return (
    <div className="min-h-[calc(100vh-4rem)] flex items-center justify-center py-12 px-4">
      <div className="w-full max-w-md">
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-8">
          <div className="text-center mb-8">
            <h1 className="text-2xl font-bold text-gray-900">Créer un compte</h1>
            <p className="text-gray-500 text-sm mt-1">Rejoignez NunyFood et prenez soin de votre famille</p>
          </div>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <Input
                label="Prénom"
                placeholder="Jean"
                error={errors.firstName?.message}
                {...register('firstName')}
              />
              <Input
                label="Nom"
                placeholder="Morelle"
                error={errors.lastName?.message}
                {...register('lastName')}
              />
            </div>

            <Input
              label="Adresse email"
              type="email"
              placeholder="vous@exemple.com"
              error={errors.email?.message}
              {...register('email')}
            />

            <Input
              label="Téléphone"
              type="tel"
              placeholder="+33 6 00 00 00 00"
              error={errors.phoneNumber?.message}
              {...register('phoneNumber')}
            />

            <Input
              label="Mot de passe"
              type="password"
              placeholder="Au moins 8 caractères"
              error={errors.password?.message}
              {...register('password')}
            />

            <Input
              label="Confirmer le mot de passe"
              type="password"
              placeholder="••••••••"
              error={errors.confirmPassword?.message}
              {...register('confirmPassword')}
            />

            {error && (
              <div className="bg-red-50 border border-red-200 text-red-700 text-sm rounded-lg px-4 py-3">
                {error}
              </div>
            )}

            <Button type="submit" loading={isSubmitting} className="w-full" size="lg">
              Créer mon compte
            </Button>
          </form>

          <p className="text-center text-sm text-gray-500 mt-6">
            Déjà un compte ?{' '}
            <Link to="/login" className="text-[#16A34A] font-medium hover:underline">
              Se connecter
            </Link>
          </p>
        </div>
      </div>
    </div>
  )
}
