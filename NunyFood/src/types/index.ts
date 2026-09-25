export interface Customer {
  id: string
  firstName: string
  lastName: string
  email: string
  phoneNumber: string
  createdAt: string
}

export interface Beneficiary {
  id: string
  customerId: string
  fullName: string
  phoneNumber: string
  address: string
  city: string
  createdAt: string
}

export interface Product {
  id: string
  name: string
  description: string
  isActive: boolean
  createdAt: string
}

export interface PackProduct {
  packId: string
  productId: string
  quantity: number
}

export interface Pack {
  id: string
  name: string
  description: string
  price: number
  imageUrl: string | null
  isActive: boolean
  createdAt: string
}

export type OrderStatus =
  | 'Created'
  | 'PendingPayment'
  | 'Paid'
  | 'Preparing'
  | 'Assigned'
  | 'InDelivery'
  | 'Delivered'
  | 'Confirmed'
  | 'Cancelled'

export interface Order {
  id: string
  customerId: string
  beneficiaryId: string
  packId: string
  amount: number
  status: OrderStatus
  createdAt: string
}

export interface OrderStatusHistory {
  id: string
  orderId: string
  status: OrderStatus
  changedAt: string
  createdAt: string
}

export type PaymentMethod = 'PayPal' | 'TMoney' | 'Flooz'
export type PaymentStatus = 'Pending' | 'Succeeded' | 'Failed'

export interface Payment {
  id: string
  orderId: string
  amount: number
  method: PaymentMethod
  status: PaymentStatus
  transactionId: string | null
  createdAt: string
}

export interface DeliveryAgent {
  id: string
  fullName: string
  email: string
  phoneNumber: string
  zone: string
  isActive: boolean
  createdAt: string
}

export interface Delivery {
  id: string
  orderId: string
  deliveryAgentId: string
  receiverName: string
  photoUrl: string | null
  signatureUrl: string | null
  latitude: number | null
  longitude: number | null
  deliveredAt: string | null
  createdAt: string
}

export interface OtpChallengeDto {
  email: string
  expiresIn: number
}

export interface TokenDto {
  token: string
  tokenType: string
  expiresIn: number
}

export type UserRole = 'Customer' | 'Admin' | 'DeliveryAgent'

export interface AuthUser {
  id: string
  email: string
  role: UserRole
}
