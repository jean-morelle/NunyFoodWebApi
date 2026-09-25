import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useState } from 'react'
import { Link } from 'react-router-dom'
import { forgotPassword, resetPassword } from '../../api/auth'
import { apiErrorMessage } from '../../api/errors'
import Input from '../../components/ui/Input'
import Button from '../../components/ui/Button'

const emailSchema = z.object({
  email: z.string().email('Email invalide'),
})

const resetSchema = z
  .object({
    code: z.string().regex(/^\d{6}$/, 'Le code doit contenir 6 chiffres.'),
    newPassword: z.string().min(8, 'Au moins 8 caractères'),
    confirmPassword: z.string(),
  })
  .refine((d) => d.newPassword === d.confirmPassword, {
    message: 'Les mots de passe ne correspondent pas.',
    path: ['confirmPassword'],
  })

type EmailData = z.infer<typeof emailSchema>
type ResetData = z.infer<typeof resetSchema>

export default function ForgotPasswordPage() {
  const [email, setEmail] = useState<string | null>(null)
  const [done, setDone] = useState(false)
  const [error, setError] = useState('')
  const [info, setInfo] = useState('')

  const emailForm = useForm<EmailData>({ resolver: zodResolver(emailSchema) })
  const resetForm = useForm<ResetData>({ resolver: zodResolver(resetSchema) })

  const onRequest = async (data: EmailData) => {
    setError('')
    try {
      await forgotPassword(data.email)
      setEmail(data.email)
    } catch (err) {
      setError(apiErrorMessage(err, "Impossible d'envoyer le code. Réessayez plus tard."))
    }
  }

  const onResend = async () => {
    if (!email) return
    setError('')
    setInfo('')
    try {
      await forgotPassword(email)
      resetForm.setValue('code', '')
      setInfo('Un nouveau code vous a été envoyé.')
    } catch (err) {
      setError(apiErrorMessage(err, 'Impossible de renvoyer le code. Réessayez plus tard.'))
    }
  }

  const onReset = async (data: ResetData) => {
    if (!email) return
    setError('')
    setInfo('')
    try {
      await resetPassword(email, data.code, data.newPassword)
      setDone(true)
    } catch (err) {
      setError(apiErrorMessage(err, 'Impossible de réinitialiser le mot de passe.'))
    }
  }

  return (
    <div className="min-h-[calc(100vh-4rem)] flex items-center justify-center py-12 px-4">
      <div className="w-full max-w-md">
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-8">
          <div className="text-center mb-8">
            <h1 className="text-2xl font-bold text-gray-900">Mot de passe oublié</h1>
            <p className="text-gray-500 text-sm mt-1">
              {done ? 'Votre mot de passe a été modifié' : 'Recevez un code par email pour en choisir un nouveau'}
            </p>
          </div>

          {done ? (
            <div className="space-y-5">
              <div className="bg-green-50 border border-green-200 text-green-800 text-sm rounded-lg px-4 py-3">
                Vous pouvez maintenant vous connecter avec votre nouveau mot de passe.
              </div>
              <Link
                to="/login"
                className="flex w-full items-center justify-center rounded-lg bg-[#16A34A] px-6 py-3 text-base font-medium text-white transition-colors hover:bg-[#15803D]"
              >
                Se connecter
              </Link>
            </div>
          ) : email ? (
            <form onSubmit={resetForm.handleSubmit(onReset)} className="space-y-5">
              <p className="text-sm text-gray-600 text-center">
                Si un compte client existe pour <span className="font-medium text-gray-900">{email}</span>,
                un code à 6 chiffres vient d'y être envoyé. Il expire dans 10 minutes.
              </p>

              <Input
                label="Code reçu par email"
                inputMode="numeric"
                autoComplete="one-time-code"
                maxLength={6}
                placeholder="123456"
                error={resetForm.formState.errors.code?.message}
                {...resetForm.register('code')}
              />
              <Input
                label="Nouveau mot de passe"
                type="password"
                autoComplete="new-password"
                placeholder="••••••••"
                error={resetForm.formState.errors.newPassword?.message}
                {...resetForm.register('newPassword')}
              />
              <Input
                label="Confirmer le mot de passe"
                type="password"
                autoComplete="new-password"
                placeholder="••••••••"
                error={resetForm.formState.errors.confirmPassword?.message}
                {...resetForm.register('confirmPassword')}
              />

              {error && (
                <div className="bg-red-50 border border-red-200 text-red-700 text-sm rounded-lg px-4 py-3">{error}</div>
              )}
              {info && (
                <div className="bg-green-50 border border-green-200 text-green-800 text-sm rounded-lg px-4 py-3">{info}</div>
              )}

              <Button type="submit" loading={resetForm.formState.isSubmitting} className="w-full" size="lg">
                Changer le mot de passe
              </Button>
              <div className="flex justify-between text-sm">
                <button type="button" onClick={() => { setEmail(null); setError(''); setInfo('') }} className="text-gray-500 hover:underline">
                  Changer d'email
                </button>
                <button type="button" onClick={onResend} className="text-[#16A34A] font-medium hover:underline">
                  Renvoyer le code
                </button>
              </div>
            </form>
          ) : (
            <form onSubmit={emailForm.handleSubmit(onRequest)} className="space-y-5">
              <Input
                label="Adresse email de votre compte client"
                type="email"
                placeholder="vous@exemple.com"
                error={emailForm.formState.errors.email?.message}
                {...emailForm.register('email')}
              />

              {error && (
                <div className="bg-red-50 border border-red-200 text-red-700 text-sm rounded-lg px-4 py-3">{error}</div>
              )}

              <Button type="submit" loading={emailForm.formState.isSubmitting} className="w-full" size="lg">
                Envoyer le code
              </Button>
            </form>
          )}

          {!done && (
            <p className="text-center text-sm text-gray-500 mt-6">
              <Link to="/login" className="text-[#16A34A] font-medium hover:underline">
                Retour à la connexion
              </Link>
            </p>
          )}
        </div>
      </div>
    </div>
  )
}
