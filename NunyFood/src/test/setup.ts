import '@testing-library/jest-dom/vitest'
import { cleanup } from '@testing-library/react'
import { afterEach, vi } from 'vitest'

afterEach(() => {
  cleanup()
  localStorage.clear()
})

// jsdom n'implémente pas le dessin sur canvas (utilisé par SignaturePad) : contexte factice.
HTMLCanvasElement.prototype.getContext = vi.fn(() => ({
  scale: vi.fn(), fillRect: vi.fn(), beginPath: vi.fn(), moveTo: vi.fn(), lineTo: vi.fn(),
  stroke: vi.fn(), save: vi.fn(), restore: vi.fn(), setTransform: vi.fn(),
})) as unknown as typeof HTMLCanvasElement.prototype.getContext
HTMLCanvasElement.prototype.toBlob = function (callback: BlobCallback) {
  callback(new Blob(['signature'], { type: 'image/png' }))
}
Element.prototype.setPointerCapture ??= vi.fn()

// Aperçu de la photo (URL.createObjectURL) : absent de jsdom.
URL.createObjectURL = vi.fn(() => 'blob:apercu')
URL.revokeObjectURL = vi.fn()
