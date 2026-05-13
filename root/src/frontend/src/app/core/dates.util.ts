export function toInputDateTimeLocal(iso: string | null | undefined): string {
  const d =
    iso == null || iso === '' || Number.isNaN(new Date(iso).getTime())
      ? new Date()
      : new Date(iso);
  const p = (n: number) => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${p(d.getMonth() + 1)}-${p(d.getDate())}T${p(d.getHours())}:${p(d.getMinutes())}`;
}

export function fromInputDateTimeLocal(value: string): string {
  return new Date(value).toISOString();
}

export function formatDisplayDate(iso: string | null | undefined): string {
  if (iso == null || iso === '') return '—';
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return '—';
  return d.toLocaleString('pt-BR');
}

export function formatMoney(n: number | string | null | undefined): string {
  const v =
    n == null || n === '' || Number.isNaN(Number(n)) ? 0 : Number(n);
  return v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}
