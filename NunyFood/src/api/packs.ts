import client from './client'
import type { Pack, PackProduct } from '../types'

export const getPacks = () =>
  client.get<Pack[]>('/packs').then((r) => r.data)

export const getPack = (id: string) =>
  client.get<Pack>(`/packs/${id}`).then((r) => r.data)

export const createPack = (data: {
  name: string
  description: string
  price: number
  imageUrl?: string
}) => client.post<Pack>('/packs', data).then((r) => r.data)

export const updatePack = (
  id: string,
  data: { name?: string; description?: string; price?: number; imageUrl?: string; isActive?: boolean },
) => client.put<Pack>(`/packs/${id}`, data).then((r) => r.data)

export const deletePack = (id: string) =>
  client.delete(`/packs/${id}`)

export const getPackProducts = (packId: string) =>
  client.get<PackProduct[]>(`/packproducts?packId=${packId}`).then((r) => r.data)

export const addProductToPack = (packId: string, data: { productId: string; quantity: number }) =>
  client.post<PackProduct>(`/packproducts/${packId}/products`, data).then((r) => r.data)

export const removeProductFromPack = (packId: string, productId: string) =>
  client.delete(`/packproducts/${packId}/products/${productId}`)
