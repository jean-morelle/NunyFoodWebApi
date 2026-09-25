import { describe, expect, it } from 'vitest'
import { apiErrorMessage } from './errors'
import { apiError } from '../test/utils'

describe('apiErrorMessage', () => {
  it('annonce le délai quand l’API limite les tentatives (429)', () => {
    expect(apiErrorMessage(apiError(429, {}, { 'retry-after': '90' }), 'x')).toBe('Trop de tentatives. Réessayez dans 2 min.')
    expect(apiErrorMessage(apiError(429), 'x')).toBe('Trop de tentatives. Réessayez dans quelques minutes.')
  })

  it('affiche la première erreur de validation', () => {
    const err = apiError(400, { title: 'One or more validation errors occurred.', errors: { Code: ['Code invalide ou expiré.'] } })
    expect(apiErrorMessage(err, 'x')).toBe('Code invalide ou expiré.')
  })

  it('affiche le message métier d’un 400 sans détail par champ', () => {
    expect(apiErrorMessage(apiError(400, { title: 'Cette livraison a déjà été confirmée.' }), 'x'))
      .toBe('Cette livraison a déjà été confirmée.')
  })

  it('garde le message par défaut pour les erreurs serveur et non HTTP', () => {
    expect(apiErrorMessage(apiError(500, { title: 'An unexpected error occurred.' }), 'Réessayez.')).toBe('Réessayez.')
    expect(apiErrorMessage(new Error('réseau'), 'Réessayez.')).toBe('Réessayez.')
  })
})
