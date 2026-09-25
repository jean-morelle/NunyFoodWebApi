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
    photo: File
    signature: Blob
    latitude?: number
    longitude?: number
  },
) => {
  const form = new FormData()
  form.append('receiverName', data.receiverName)
  form.append('photo', data.photo)
  form.append('signature', data.signature, 'signature.png')
  // Point décimal quelle que soit la langue du navigateur ; l'API lit en culture invariante.
  if (data.latitude !== undefined) form.append('latitude', data.latitude.toString())
  if (data.longitude !== undefined) form.append('longitude', data.longitude.toString())
  return client.patch<Delivery>(`/deliveries/${id}/confirm`, form).then((r) => r.data)
}

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
