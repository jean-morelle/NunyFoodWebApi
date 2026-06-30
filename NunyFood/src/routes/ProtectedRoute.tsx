import { Navigate } from 'react-router-dom'
import { useAuthStore } from '../store/authStore'
import type { UserRole } from '../types'

interface Props {
  role: UserRole
  children: React.ReactNode
}

export default function ProtectedRoute({ role, children }: Props) {
  const { token, user } = useAuthStore()

  if (!token || !user) return <Navigate to="/login" replace />
  if (user.role !== role) return <Navigate to="/" replace />

  return <>{children}</>
}
