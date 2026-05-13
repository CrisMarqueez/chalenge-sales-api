export function formatErrorBody(
  body: unknown,
  status?: number,
  statusText?: string
): string {
  if (body == null) {
    if (status === 401)
      return 'HTTP 401 — não autorizado (token inválido ou expirado; faça login novamente).';
    if (status === 403)
      return 'HTTP 403 — acesso negado (sua conta pode não ter permissão de administrador).';
    if (status != null)
      return `HTTP ${status}${statusText ? ` ${statusText}` : ''} — resposta sem detalhe do servidor.`;
    return 'Erro desconhecido';
  }
  if (typeof body === 'string') return body;
  if (Array.isArray(body)) {
    const msgs = body.map((e) => {
      if (e && typeof e === 'object') {
        const o = e as Record<string, unknown>;
        if (typeof o['errorMessage'] === 'string') return o['errorMessage'] as string;
        if (typeof o['detail'] === 'string') return o['detail'] as string;
        if (typeof o['message'] === 'string') return o['message'] as string;
      }
      return JSON.stringify(e);
    });
    return msgs.join('; ');
  }
  if (typeof body === 'object') {
    const o = body as Record<string, unknown>;
    if (typeof o['detail'] === 'string') {
      if (typeof o['error'] === 'string' && (o['error'] as string).length > 0)
        return `${o['error']}: ${o['detail']}`;
      if (typeof o['type'] === 'string' && (o['type'] as string).length > 0)
        return `[${o['type']}] ${o['detail']}`;
      return o['detail'] as string;
    }
    if (typeof o['error'] === 'string') return o['error'] as string;
    if (typeof o['message'] === 'string') return o['message'] as string;
    if (Array.isArray(o['errors'])) {
      const errs = o['errors'] as Array<{ detail?: string; errorMessage?: string }>;
      return errs
        .map((x) => x.detail ?? x.errorMessage ?? '')
        .filter(Boolean)
        .join('; ');
    }
  }
  try {
    return JSON.stringify(body);
  } catch {
    return 'Erro ao processar resposta';
  }
}

export class ApiError extends Error {
  readonly status: number;
  readonly body: unknown;

  constructor(status: number, body: unknown) {
    const msg = formatErrorBody(body, status) || `HTTP ${status}`;
    super(msg);
    this.name = 'ApiError';
    this.status = status;
    this.body = body;
  }
}
