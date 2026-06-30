import client from './client'
import type { Product } from '../types'

export const getProducts = () =>
  client.get<Product[]>('/products').then((r) => r.data)

export const getProduct = (id: string) =>
  client.get<Product>(`/products/${id}`).then((r) => r.data)

export const createProduct = (data: { name: string; description: string }) =>
  client.post<Product>('/products', data).then((r) => r.data)

export const updateProduct = (
  id: string,
  data: { name?: string; description?: string; isActive?: boolean },
) => client.put<Product>(`/products/${id}`, data).then((r) => r.data)

export const deleteProduct = (id: string) =>
  client.delete(`/products/${id}`)
