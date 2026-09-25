import type { ReactElement } from 'react'
import { render } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { AxiosError, AxiosHeaders, type AxiosResponse } from 'axios'

/** Rend un composant avec React Query (sans nouvel essai automatique) et un routeur en mémoire. */
export function renderWithProviders(ui: ReactElement, { route = '/', path = '*' }: { route?: string; path?: string } = {}) {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false }, mutations: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter initialEntries={[route]}>
        <Routes>
          <Route path={path} element={ui} />
        </Routes>
      </MemoryRouter>
    </QueryClientProvider>,
  )
}

/** Erreur axios telle que la renvoie l'API (status, corps JSON, en-têtes). */
export function apiError(status: number, data: unknown = {}, headers: Record<string, string> = {}) {
  const response = {
    status,
    data,
    headers: new AxiosHeaders(headers),
    statusText: '',
    config: { headers: new AxiosHeaders() },
  } as AxiosResponse
  return new AxiosError(`HTTP ${status}`, String(status), response.config, null, response)
}
