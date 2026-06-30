import client from './client'
import type { Order, OrderStatus, OrderStatusHistory } from '../types'

export const getOrders = (customerId?: string) => {
  const url = customerId ? `/orders?customerId=${customerId}` : '/orders'
  return client.get<Order[]>(url).then((r) => r.data)
}

export const getOrder = (id: string) =>
  client.get<Order>(`/orders/${id}`).then((r) => r.data)

export const createOrder = (data: {
  customerId: string
  beneficiaryId: string
  packId: string
}) => client.post<Order>('/orders', data).then((r) => r.data)

export const updateOrderStatus = (id: string, status: OrderStatus) =>
  client.patch<Order>(`/orders/${id}/status`, { status }).then((r) => r.data)

export const getOrderStatusHistory = (id: string) =>
  client.get<OrderStatusHistory[]>(`/orders/${id}/status-history`).then((r) => r.data)
