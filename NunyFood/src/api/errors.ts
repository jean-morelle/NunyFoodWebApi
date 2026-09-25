import axios from 'axios'

/**
 * Message à afficher pour une erreur d'API :
 * - 429 : trop de tentatives (limite anti-force brute de l'API) ;
 * - 400 de validation : premier message renvoyé par l'API ;
 * - 400 métier (« Cette livraison a déjà été confirmée. ») : le titre renvoyé par l'API ;
 * - sinon : le message par défaut fourni.
 */
export function apiErrorMessage(err: unknown, fallback: string): string {
  if (!axios.isAxiosError(err)) return fallback
  const data = err.response?.data as { title?: string; errors?: Record<string, string[]> } | undefined

  if (err.response?.status === 429) {
    const retryAfter = Number(err.response.headers['retry-after'])
    return retryAfter > 0
      ? `Trop de tentatives. Réessayez dans ${Math.ceil(retryAfter / 60)} min.`
      : 'Trop de tentatives. Réessayez dans quelques minutes.'
  }

  const firstError = data?.errors && Object.values(data.errors)[0]?.[0]
  if (firstError) return firstError
  return err.response?.status === 400 && data?.title ? data.title : fallback
}
