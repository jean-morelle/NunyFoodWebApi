import { useState, type FormEvent } from 'react'
import Input from '../ui/Input'
import Button from '../ui/Button'
import { apiErrorMessage } from '../../api/errors'

interface Props {
  email: string
  onVerify: (code: string) => Promise<void>
  onResend: () => Promise<void>
  onBack?: () => void
}

export default function OtpVerification({ email, onVerify, onResend, onBack }: Props) {
  const [code, setCode] = useState('')
  const [error, setError] = useState('')
  const [info, setInfo] = useState('')
  const [verifying, setVerifying] = useState(false)
  const [resending, setResending] = useState(false)

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')
    setInfo('')
    if (!/^\d{6}$/.test(code)) {
      setError('Le code doit contenir 6 chiffres.')
      return
    }
    setVerifying(true)
    try {
      await onVerify(code)
    } catch (err) {
      setError(apiErrorMessage(err, 'Code invalide ou expiré.'))
    } finally {
      setVerifying(false)
    }
  }

  const handleResend = async () => {
    setError('')
    setInfo('')
    setResending(true)
    try {
      await onResend()
      setCode('')
      setInfo('Un nouveau code vous a été envoyé.')
    } catch (err) {
      setError(apiErrorMessage(err, 'Impossible de renvoyer le code. Réessayez plus tard.'))
    } finally {
      setResending(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-5">
      <p className="text-sm text-gray-600 text-center">
        Un code à 6 chiffres a été envoyé à <span className="font-medium text-gray-900">{email}</span>.
        Il expire dans 10 minutes.
      </p>

      <Input
        label="Code de vérification"
        inputMode="numeric"
        autoComplete="one-time-code"
        maxLength={6}
        placeholder="123456"
        autoFocus
        value={code}
        onChange={(e) => setCode(e.target.value.replace(/\D/g, ''))}
        className="text-center tracking-[0.5em] text-lg"
      />

      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 text-sm rounded-lg px-4 py-3">
          {error}
        </div>
      )}
      {info && (
        <div className="bg-green-50 border border-green-200 text-green-700 text-sm rounded-lg px-4 py-3">
          {info}
        </div>
      )}

      <Button type="submit" loading={verifying} className="w-full" size="lg">
        Vérifier
      </Button>

      <div className="flex justify-between text-sm">
        {onBack ? (
          <button type="button" onClick={onBack} className="text-gray-500 hover:underline">
            ← Retour
          </button>
        ) : (
          <span />
        )}
        <button
          type="button"
          onClick={handleResend}
          disabled={resending}
          className="text-[#16A34A] font-medium hover:underline disabled:opacity-50"
        >
          {resending ? 'Envoi…' : 'Renvoyer le code'}
        </button>
      </div>
    </form>
  )
}
