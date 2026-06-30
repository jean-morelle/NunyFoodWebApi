import { forwardRef, type InputHTMLAttributes } from 'react'

interface Props extends InputHTMLAttributes<HTMLInputElement> {
  label?: string
  error?: string
}

const Input = forwardRef<HTMLInputElement, Props>(({ label, error, className = '', ...props }, ref) => (
  <div className="flex flex-col gap-1">
    {label && (
      <label className="text-sm font-medium text-gray-700 dark:text-gray-300">
        {label}
      </label>
    )}
    <input
      ref={ref}
      {...props}
      className={`block w-full rounded-lg border px-3 py-2 text-sm text-gray-900 dark:text-gray-100 bg-white dark:bg-gray-800 placeholder-gray-400 dark:placeholder-gray-500 transition-colors focus:outline-none focus:ring-2 focus:ring-[#16A34A] focus:border-[#16A34A] ${
        error ? 'border-red-500' : 'border-gray-300 dark:border-gray-600'
      } ${className}`}
    />
    {error && <p className="text-xs text-red-600 dark:text-red-400">{error}</p>}
  </div>
))

Input.displayName = 'Input'
export default Input
