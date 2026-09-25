import { describe, expect, it, vi } from 'vitest'
import { screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import LoginPage from './LoginPage'
import { loginCustomer } from '../../api/auth'
import { apiError, renderWithProviders } from '../../test/utils'

vi.mock('../../api/auth', () => ({
  loginCustomer: vi.fn(), loginAdmin: vi.fn(), loginAgent: vi.fn(),
  verifyCustomerOtp: vi.fn(), verifyAdminOtp: vi.fn(), verifyAgentOtp: vi.fn(),
}))

describe('LoginPage', () => {
  it('propose « Mot de passe oublié » uniquement aux clients', async () => {
    const user = userEvent.setup()
    renderWithProviders(<LoginPage />)

    expect(screen.getByRole('link', { name: 'Mot de passe oublié ?' })).toHaveAttribute('href', '/forgot-password')

    await user.selectOptions(screen.getByRole('combobox'), 'admin')
    expect(screen.queryByRole('link', { name: 'Mot de passe oublié ?' })).not.toBeInTheDocument()
  })

  it('passe à la saisie du code après un mot de passe correct', async () => {
    vi.mocked(loginCustomer).mockResolvedValue({ email: 'ama@test.local', expiresIn: 600 })
    const user = userEvent.setup()
    renderWithProviders(<LoginPage />)

    await user.type(screen.getByLabelText('Adresse email'), 'ama@test.local')
    await user.type(screen.getByLabelText('Mot de passe'), 'Test@12345')
    await user.click(screen.getByRole('button', { name: 'Se connecter' }))

    expect(loginCustomer).toHaveBeenCalledWith('ama@test.local', 'Test@12345')
    expect(await screen.findByText(/un code à 6 chiffres a été envoyé/i)).toBeInTheDocument()
  })

  it('distingue un mauvais mot de passe d’une limite de tentatives', async () => {
    const user = userEvent.setup()
    renderWithProviders(<LoginPage />)
    const submit = async () => {
      await user.clear(screen.getByLabelText('Adresse email'))
      await user.type(screen.getByLabelText('Adresse email'), 'ama@test.local')
      await user.clear(screen.getByLabelText('Mot de passe'))
      await user.type(screen.getByLabelText('Mot de passe'), 'faux')
      await user.click(screen.getByRole('button', { name: 'Se connecter' }))
    }

    vi.mocked(loginCustomer).mockRejectedValueOnce(apiError(401))
    await submit()
    expect(await screen.findByText('Email ou mot de passe incorrect.')).toBeInTheDocument()

    vi.mocked(loginCustomer).mockRejectedValueOnce(apiError(429, {}, { 'retry-after': '60' }))
    await submit()
    expect(await screen.findByText('Trop de tentatives. Réessayez dans 1 min.')).toBeInTheDocument()
  })
})
