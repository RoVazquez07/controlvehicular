import { assertApiBaseUrl } from '../config/env'

export class ApiError extends Error {
  readonly status: number
  readonly body: unknown

  constructor(message: string, status: number, body: unknown) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.body = body
  }
}

type RequestOptions = Omit<RequestInit, 'body'> & {
  body?: unknown
  token?: string | null
}

function resolveMessage(status: number, body: unknown): string {
  if (body && typeof body === 'object') {
    const record = body as Record<string, unknown>
    if (typeof record.message === 'string') return record.message
    if (typeof record.title === 'string') return record.title
    if (typeof record.error === 'string') return record.error
  }
  if (status === 401) return 'Credenciales inválidas'
  if (status === 403) return 'No autorizado'
  if (status >= 500) return 'Error del servidor'
  return `Error de red (${status})`
}

export async function apiRequest<T>(
  path: string,
  { body, token, headers, ...init }: RequestOptions = {},
): Promise<T> {
  const baseUrl = assertApiBaseUrl()
  const url = `${baseUrl}${path.startsWith('/') ? path : `/${path}`}`

  const response = await fetch(url, {
    ...init,
    headers: {
      Accept: 'application/json',
      ...(body !== undefined ? { 'Content-Type': 'application/json' } : {}),
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...headers,
    },
    body: body !== undefined ? JSON.stringify(body) : undefined,
  })

  const text = await response.text()
  let parsed: unknown = null
  if (text) {
    try {
      parsed = JSON.parse(text) as unknown
    } catch {
      parsed = text
    }
  }

  if (!response.ok) {
    throw new ApiError(resolveMessage(response.status, parsed), response.status, parsed)
  }

  return parsed as T
}
