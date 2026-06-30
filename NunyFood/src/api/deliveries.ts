import client from './client'
import type { Delivery, DeliveryAgent } from '../types'

export const getDeliveryByOrder = (orderId: string) =>
  client.get<Delivery>(`/deliveries?orderId=${orderId}`).then((r) => r.data)

export const getDeliveriesByAgent = (agentId: string) =>
  client.get<Delivery[]>(`/deliveries?agentId=${agentId}`).then((r) => r.data)

export const createDelivery = (data: {
  orderId: string
  deliveryAgentId: string
  receiverName: string
}) => client.post<Delivery>('/deliveries', data).then((r) => r.data)

export const confirmDelivery = (
  id: string,
  data: {
    receiverName: string
    photoUrl?: string
    signatureUrl?: string
    latitude?: number
    longitude?: number
  },
) => client.patch<Delivery>(`/deliveries/${id}/confirm`, data).then((r) => r.data)

export const getDeliveryAgents = () =>
  client.get<DeliveryAgent[]>('/deliveryagents').then((r) => r.data)

export const createDeliveryAgent = (data: {
  fullName: string
  email: string
  phoneNumber: string
  zone: string
  password: string
}) => client.post<DeliveryAgent>('/deliveryagents', data).then((r) => r.data)

export const updateDeliveryAgent = (
  id: string,
  data: { fullName?: string; phoneNumber?: string; zone?: string; isActive?: boolean },
) => client.put<DeliveryAgent>(`/deliveryagents/${id}`, data).then((r) => r.data)

export const deleteDeliveryAgent = (id: string) =>
  client.delete(`/deliveryagents/${id}`)
