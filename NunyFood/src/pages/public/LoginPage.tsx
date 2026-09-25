import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuthStore } from '../../store/authStore'
import {
  loginCustomer,
  loginAdmin,
  loginAgent,
  verifyCustomerOtp,
  verifyAdminOtp,
  verifyAgentOtp,
} from '../../api/auth'
import { apiErrorMessage } from '../../api/errors'
import Input from '../../components/ui/Input'
import Button from '../../components/ui/Button'
import OtpVerification from '../../components/auth/OtpVerification'

const schema = z.object({
  email: z.string().email('Email invalide'),
  password: z.string().min(1, 'Mot de passe requis'),
  role: z.enum(['customer', 'admin', 'agent']),
})

type FormData = z.infer<typeof schema>

const loginFns = {
  customer: loginCustomer,
  admin: loginAdmin,
  agent: loginAgent,
}

const verifyFns = {
  customer: verifyCustomerOtp,
  admin: verifyAdminOtp,
  agent: verifyAgentOtp,
}

const redirects = {
  customer: '/customer/dashboard',
  admin: '/admin/dashboard',
  agent: '/agent/dashboard',
}

export default function LoginPage() {
  const { setToken } = useAuthStore()
  const navigate = useNavigate()
  const [error, setError] = useState('')
  const [pending, setPending] = useState<FormData | null>(null)

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors, isSubmitting },
  } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: { role: 'customer' },
  })
  // La réinitialisation du mot de passe n'existe que pour les clients.
  const role = watch('role')

  const onSubmit = async (data: FormData) => {
    setError('')
    try {
      await loginFns[data.role](data.email, data.password)
      setPending(data)
    } catch (err) {
      setError(apiErrorMessage(err, 'Email ou mot de passe incorrect.'))
    }
  }

  const onVerify = async (code: string) => {
    if (!pending) return
    const token = await verifyFns[pending.role](pending.email, code)
    setToken(token.token)
    navigate(redirects[pending.role])
  }

  const onResend = async () => {
    if (!pending) return
    await loginFns[pending.role](pending.email, pending.password)
  }

  return (
    <div className="min-h-[calc(100vh-4rem)] flex items-center justify-center py-12 px-4">
      <div className="w-full max-w-md">
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-8">
          <div className="text-center mb-8">
            <h1 className="text-2xl font-bold text-gray-900">Connexion</h1>
            <p className="text-gray-500 text-sm mt-1">Accédez à votre espace NunyFood</p>
          </div>

          {pending ? (
            <OtpVerification
              email={pending.email}
              onVerify={onVerify}
              onResend={onResend}
              onBack={() => setPending(null)}
            />
          ) : (
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
              <div>
                <label className="text-sm font-medium text-gray-700 block mb-1">Type de compte</label>
                <select
                  {...register('role')}
                  className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-[#16A34A] focus:border-[#16A34A]"
                >
                  <option value="customer">Client</option>
                  <option value="admin">Administrateur</option>
                  <option value="agent">Livreur</option>
                </select>
              </div>
  
              <Input
                label="Adresse email"
                type="email"
                placeholder="vous@exemple.com"
                error={errors.email?.message}
                {...register('email')}
              />
  
              <Input
                label="Mot de passe"
                type="password"
                placeholder="••••••••"
                error={errors.password?.message}
                {...register('password')}
              />

              {role === 'customer' && (
                <div className="-mt-2 text-right">
                  <Link to="/forgot-password" className="text-sm text-[#16A34A] hover:underline">
                    Mot de passe oublié ?
                  </Link>
                </div>
              )}

              {error && (
                <div className="bg-red-50 border border-red-200 text-red-700 text-sm rounded-lg px-4 py-3">
                  {error}
                </div>
              )}
  
              <Button type="submit" loading={isSubmitting} className="w-full" size="lg">
                Se connecter
              </Button>
            </form>
          )}

          <p className="text-center text-sm text-gray-500 mt-6">
            Pas encore de compte ?{' '}
            <Link to="/register" className="text-[#16A34A] font-medium hover:underline">
              S'inscrire
            </Link>
          </p>
        </div>
      </div>
    </div>
  )
}
