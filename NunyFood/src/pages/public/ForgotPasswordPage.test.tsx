import { beforeEach, describe, expect, it, vi } from 'vitest'
import { screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import ForgotPasswordPage from './ForgotPasswordPage'
import { forgotPassword, resetPassword } from '../../api/auth'
import { apiError, renderWithProviders } from '../../test/utils'

vi.mock('../../api/auth', () => ({ forgotPassword: vi.fn(), resetPassword: vi.fn() }))

async function requestCode(user: ReturnType<typeof userEvent.setup>) {
  await user.type(screen.getByLabelText(/adresse email/i), 'ama@test.local')
  await user.click(screen.getByRole('button', { name: 'Envoyer le code' }))
  await screen.findByText(/si un compte client existe/i)
}

describe('ForgotPasswordPage', () => {
  beforeEach(() => {
    vi.mocked(forgotPassword).mockResolvedValue({ email: 'ama@test.local', expiresIn: 600 })
    vi.mocked(resetPassword).mockResolvedValue({} as never)
  })

  it('refuse un email invalide sans appeler l’API', async () => {
    const user = userEvent.setup()
    renderWithProviders(<ForgotPasswordPage />)

    await user.type(screen.getByLabelText(/adresse email/i), 'pas-un-email')
    await user.click(screen.getByRole('button', { name: 'Envoyer le code' }))

    // Le champ type="email" bloque l'envoi (validation native du navigateur) : on reste à l'étape 1.
    expect(screen.getByLabelText(/adresse email/i)).toBeInvalid()
    expect(screen.getByRole('button', { name: 'Envoyer le code' })).toBeInTheDocument()
    expect(forgotPassword).not.toHaveBeenCalled()
  })

  it('refuse deux mots de passe différents', async () => {
    const user = userEvent.setup()
    renderWithProviders(<ForgotPasswordPage />)
    await requestCode(user)

    await user.type(screen.getByLabelText(/code reçu/i), '123456')
    await user.type(screen.getByLabelText('Nouveau mot de passe'), 'Nouveau@12345')
    await user.type(screen.getByLabelText('Confirmer le mot de passe'), 'Autre@12345')
    await user.click(screen.getByRole('button', { name: 'Changer le mot de passe' }))

    expect(await screen.findByText('Les mots de passe ne correspondent pas.')).toBeInTheDocument()
    expect(resetPassword).not.toHaveBeenCalled()
  })

  it('affiche l’erreur renvoyée par l’API pour un mauvais code', async () => {
    vi.mocked(resetPassword).mockRejectedValue(apiError(400, { errors: { Code: ['Code invalide ou expiré.'] } }))
    const user = userEvent.setup()
    renderWithProviders(<ForgotPasswordPage />)
    await requestCode(user)

    await user.type(screen.getByLabelText(/code reçu/i), '000000')
    await user.type(screen.getByLabelText('Nouveau mot de passe'), 'Nouveau@12345')
    await user.type(screen.getByLabelText('Confirmer le mot de passe'), 'Nouveau@12345')
    await user.click(screen.getByRole('button', { name: 'Changer le mot de passe' }))

    expect(await screen.findByText('Code invalide ou expiré.')).toBeInTheDocument()
  })

  it('change le mot de passe puis propose de se connecter', async () => {
    const user = userEvent.setup()
    renderWithProviders(<ForgotPasswordPage />)
    await requestCode(user)

    await user.type(screen.getByLabelText(/code reçu/i), '123456')
    await user.type(screen.getByLabelText('Nouveau mot de passe'), 'Nouveau@12345')
    await user.type(screen.getByLabelText('Confirmer le mot de passe'), 'Nouveau@12345')
    await user.click(screen.getByRole('button', { name: 'Changer le mot de passe' }))

    expect(resetPassword).toHaveBeenCalledWith('ama@test.local', '123456', 'Nouveau@12345')
    expect(await screen.findByRole('link', { name: 'Se connecter' })).toHaveAttribute('href', '/login')
  })

  it('signale la limite de tentatives', async () => {
    vi.mocked(forgotPassword).mockRejectedValue(apiError(429, {}, { 'retry-after': '600' }))
    const user = userEvent.setup()
    renderWithProviders(<ForgotPasswordPage />)

    await user.type(screen.getByLabelText(/adresse email/i), 'ama@test.local')
    await user.click(screen.getByRole('button', { name: 'Envoyer le code' }))

    expect(await screen.findByText('Trop de tentatives. Réessayez dans 10 min.')).toBeInTheDocument()
  })
})
