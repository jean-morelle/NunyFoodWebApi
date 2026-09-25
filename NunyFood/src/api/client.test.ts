import { afterEach, describe, expect, it, vi } from 'vitest'
import { AxiosError, AxiosHeaders, type InternalAxiosRequestConfig } from 'axios'
import client from './client'

// Remplace le transport HTTP : chaque requête échoue avec le statut donné.
function failWith(status: number) {
  client.defaults.adapter = async (config: InternalAxiosRequestConfig) => {
    throw new AxiosError(`HTTP ${status}`, String(status), config, null, {
      status, statusText: '', data: {}, headers: new AxiosHeaders(), config,
    })
  }
}

describe('client API', () => {
  const originalAdapter = client.defaults.adapter
  afterEach(() => { client.defaults.adapter = originalAdapter })

  it('envoie le token enregistré', async () => {
    localStorage.setItem('token', 'abc')
    const seen = vi.fn()
    client.defaults.adapter = async (config) => {
      seen(config.headers.Authorization)
      return { data: {}, status: 200, statusText: '', headers: {}, config }
    }

    await client.get('/orders')

    expect(seen).toHaveBeenCalledWith('Bearer abc')
  })

  it('sur /auth, un 401 (mauvais mot de passe ou code) ne déconnecte pas', async () => {
    localStorage.setItem('token', 'abc')
    failWith(401)

    await expect(client.post('/auth/customer/verify-otp', {})).rejects.toThrow()

    expect(localStorage.getItem('token')).toBe('abc')
  })

  it('ailleurs, un 401 (session expirée) supprime le token', async () => {
    localStorage.setItem('token', 'abc')
    failWith(401)
    // jsdom ne gère pas la navigation déclenchée par la redirection vers /login
    vi.spyOn(console, 'error').mockImplementation(() => {})

    await expect(client.get('/orders')).rejects.toThrow()

    expect(localStorage.getItem('token')).toBeNull()
  })
})
