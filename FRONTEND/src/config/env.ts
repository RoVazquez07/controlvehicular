/** Base URL del API. Configurar en `.env` → `VITE_API_URL`. */
export function getApiBaseUrl(): string {
  const value = import.meta.env.VITE_API_URL
  return typeof value === 'string' ? value.replace(/\/$/, '') : ''
}

export function assertApiBaseUrl(): string {
  const baseUrl = getApiBaseUrl()
  if (!baseUrl) {
    throw new Error(
      'Falta VITE_API_URL. Copia FRONTEND/.env.example a .env y define la URL del backend.',
    )
  }
  return baseUrl
}
