import client from './client'
import type { Customer } from '../types'

export const getCustomers = () =>
  client.get<Customer[]>('/customers').then((r) => r.data)

export const getCustomer = (id: string) =>
  client.get<Customer>(`/customers/${id}`).then((r) => r.data)

export const createCustomer = (data: {
  firstName: string
  lastName: string
  email: string
  phoneNumber: string
  password: string
}) => client.post<Customer>('/customers', data).then((r) => r.data)

export const updateCustomer = (
  id: string,
  data: { firstName?: string; lastName?: string; phoneNumber?: string },
) => client.put<Customer>(`/customers/${id}`, data).then((r) => r.data)

export const deleteCustomer = (id: string) =>
  client.delete(`/customers/${id}`)

export const changePassword = (
  id: string,
  data: { currentPassword: string; newPassword: string },
) => client.patch(`/customers/${id}/change-password`, data)
