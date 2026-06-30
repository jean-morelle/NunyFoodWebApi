import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { AuthUser, UserRole } from '../types'

function parseJwt(token: string): { sub: string; email: string; role: string } | null {
  try {
    const base64url = token.split('.')[1]
    const base64 = base64url.replace(/-/g, '+').replace(/_/g, '/')
    const padded = base64.padEnd(base64.length + (4 - (base64.length % 4)) % 4, '=')
    return JSON.parse(atob(padded))
  } catch {
    return null
  }
}

interface AuthState {
  token: string | null
  user: AuthUser | null
  setToken: (token: string) => void
  logout: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      user: null,
      setToken: (token) => {
        const payload = parseJwt(token)
        if (!payload) return
        localStorage.setItem('token', token)
        set({
          token,
          user: {
            id: payload.sub,
            email: payload.email,
            role: payload.role as UserRole,
          },
        })
      },
      logout: () => {
        localStorage.removeItem('token')
        set({ token: null, user: null })
      },
    }),
    { name: 'nunyfood-auth', partialize: (s) => ({ token: s.token, user: s.user }) },
  ),
)
