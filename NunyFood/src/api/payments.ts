import client from './client'
import type { Payment, PaymentMethod } from '../types'

export const getPaymentsByOrder = (orderId: string) =>
  client.get<Payment[]>(`/payments?orderId=${orderId}`).then((r) => r.data)

export const createPayment = (data: {
  orderId: string
  amount: number
  method: PaymentMethod
}) => client.post<Payment>('/payments', data).then((r) => r.data)
