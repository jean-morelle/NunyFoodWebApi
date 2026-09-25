import client from './client'
import type { OtpChallengeDto, TokenDto } from '../types'

export const loginCustomer = (email: string, password: string) =>
  client.post<OtpChallengeDto>('/auth/customer/login', { email, password }).then((r) => r.data)

export const loginAdmin = (email: string, password: string) =>
  client.post<OtpChallengeDto>('/auth/admin/login', { email, password }).then((r) => r.data)

export const loginAgent = (email: string, password: string) =>
  client.post<OtpChallengeDto>('/auth/agent/login', { email, password }).then((r) => r.data)

export const verifyCustomerOtp = (email: string, code: string) =>
  client.post<TokenDto>('/auth/customer/verify-otp', { email, code }).then((r) => r.data)

export const verifyAdminOtp = (email: string, code: string) =>
  client.post<TokenDto>('/auth/admin/verify-otp', { email, code }).then((r) => r.data)

export const verifyAgentOtp = (email: string, code: string) =>
  client.post<TokenDto>('/auth/agent/verify-otp', { email, code }).then((r) => r.data)

export const forgotPassword = (email: string) =>
  client.post<OtpChallengeDto>('/auth/customer/forgot-password', { email }).then((r) => r.data)

export const resetPassword = (email: string, code: string, newPassword: string) =>
  client.post('/auth/customer/reset-password', { email, code, newPassword })
