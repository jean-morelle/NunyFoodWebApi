import client from './client'
import type { Beneficiary } from '../types'

export const getBeneficiaries = (customerId: string) =>
  client.get<Beneficiary[]>(`/beneficiaries?customerId=${customerId}`).then((r) => r.data)

export const getBeneficiary = (id: string) =>
  client.get<Beneficiary>(`/beneficiaries/${id}`).then((r) => r.data)

export const createBeneficiary = (data: {
  customerId: string
  fullName: string
  phoneNumber: string
  address: string
  city: string
}) => client.post<Beneficiary>('/beneficiaries', data).then((r) => r.data)

export const updateBeneficiary = (
  id: string,
  data: { fullName?: string; phoneNumber?: string; address?: string; city?: string },
) => client.put<Beneficiary>(`/beneficiaries/${id}`, data).then((r) => r.data)

export const deleteBeneficiary = (id: string) =>
  client.delete(`/beneficiaries/${id}`)
