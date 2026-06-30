import client from './client'
import type { TokenDto } from '../types'

export const loginCustomer = (email: string, password: string) =>
  client.post<TokenDto>('/auth/customer/login', { email, password }).then((r) => r.data)

export const loginAdmin = (email: string, password: string) =>
  client.post<TokenDto>('/auth/admin/login', { email, password }).then((r) => r.data)

export const loginAgent = (email: string, password: string) =>
  client.post<TokenDto>('/auth/agent/login', { email, password }).then((r) => r.data)
