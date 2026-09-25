import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import PublicLayout from './layouts/PublicLayout'
import CustomerLayout from './layouts/CustomerLayout'
import AdminLayout from './layouts/AdminLayout'
import DeliveryLayout from './layouts/DeliveryLayout'
import ProtectedRoute from './routes/ProtectedRoute'

import HomePage from './pages/public/HomePage'
import LoginPage from './pages/public/LoginPage'
import RegisterPage from './pages/public/RegisterPage'
import ForgotPasswordPage from './pages/public/ForgotPasswordPage'

import CustomerDashboardPage from './pages/customer/DashboardPage'
import CustomerPacksPage from './pages/customer/PacksPage'
import PackDetailPage from './pages/customer/PackDetailPage'
import BeneficiariesPage from './pages/customer/BeneficiariesPage'
import CustomerOrdersPage from './pages/customer/OrdersPage'
import OrderDetailPage from './pages/customer/OrderDetailPage'
import CustomerPaymentsPage from './pages/customer/PaymentsPage'
import ProfilePage from './pages/customer/ProfilePage'

import AgentDashboardPage from './pages/agent/DashboardPage'
import AgentDeliveriesPage from './pages/agent/DeliveriesPage'

import AdminDashboardPage from './pages/admin/DashboardPage'
import AdminCustomersPage from './pages/admin/CustomersPage'
import AdminProductsPage from './pages/admin/ProductsPage'
import AdminPacksPage from './pages/admin/PacksPage'
import AdminOrdersPage from './pages/admin/OrdersPage'
import AdminPaymentsPage from './pages/admin/PaymentsPage'
import AdminDeliveryAgentsPage from './pages/admin/DeliveryAgentsPage'
import AdminDeliveriesPage from './pages/admin/DeliveriesPage'

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Public */}
        <Route element={<PublicLayout />}>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/forgot-password" element={<ForgotPasswordPage />} />
        </Route>

        {/* Customer */}
        <Route
          path="/customer"
          element={
            <ProtectedRoute role="Customer">
              <CustomerLayout />
            </ProtectedRoute>
          }
        >
          <Route index element={<Navigate to="dashboard" replace />} />
          <Route path="dashboard" element={<CustomerDashboardPage />} />
          <Route path="packs" element={<CustomerPacksPage />} />
          <Route path="packs/:id" element={<PackDetailPage />} />
          <Route path="beneficiaries" element={<BeneficiariesPage />} />
          <Route path="orders" element={<CustomerOrdersPage />} />
          <Route path="orders/:id" element={<OrderDetailPage />} />
          <Route path="payments" element={<CustomerPaymentsPage />} />
          <Route path="profile" element={<ProfilePage />} />
        </Route>

        {/* Agent */}
        <Route
          path="/agent"
          element={
            <ProtectedRoute role="DeliveryAgent">
              <DeliveryLayout />
            </ProtectedRoute>
          }
        >
          <Route index element={<Navigate to="dashboard" replace />} />
          <Route path="dashboard" element={<AgentDashboardPage />} />
          <Route path="deliveries" element={<AgentDeliveriesPage />} />
        </Route>

        {/* Admin */}
        <Route
          path="/admin"
          element={
            <ProtectedRoute role="Admin">
              <AdminLayout />
            </ProtectedRoute>
          }
        >
          <Route index element={<Navigate to="dashboard" replace />} />
          <Route path="dashboard" element={<AdminDashboardPage />} />
          <Route path="customers" element={<AdminCustomersPage />} />
          <Route path="products" element={<AdminProductsPage />} />
          <Route path="packs" element={<AdminPacksPage />} />
          <Route path="orders" element={<AdminOrdersPage />} />
          <Route path="payments" element={<AdminPaymentsPage />} />
          <Route path="delivery-agents" element={<AdminDeliveryAgentsPage />} />
          <Route path="deliveries" element={<AdminDeliveriesPage />} />
        </Route>

        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  )
}
