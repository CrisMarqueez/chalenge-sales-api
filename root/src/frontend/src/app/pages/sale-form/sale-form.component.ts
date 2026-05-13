import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ApiError } from '../../core/api-error';
import {
  fromInputDateTimeLocal,
  toInputDateTimeLocal,
} from '../../core/dates.util';
import type {
  CreateSaleRequest,
  UpdateSaleRequest,
} from '../../models/sale.model';
import { SalesApiService } from '../../services/sales-api.service';

const MAX_QTY = 20;

type Line = {
  key: string;
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  isCancelled: boolean;
};

function emptyLine(): Line {
  return {
    key: crypto.randomUUID(),
    productId: crypto.randomUUID(),
    productName: '',
    quantity: 1,
    unitPrice: 0,
    isCancelled: false,
  };
}

@Component({
  selector: 'app-sale-form',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './sale-form.component.html',
  styleUrl: './sale-form.component.scss',
})
export class SaleFormComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly salesApi = inject(SalesApiService);

  readonly maxQty = MAX_QTY;

  id: string | null = null;
  isEdit = false;

  saleDate = toInputDateTimeLocal(new Date().toISOString());
  customerId = crypto.randomUUID() as string;
  customerName = '';
  branchId = crypto.randomUUID() as string;
  branchName = '';
  saleCancelled = false;
  lines: Line[] = [emptyLine()];

  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    this.isEdit = Boolean(this.id);
    if (this.id) {
      this.loading.set(true);
      void this.load();
    }
  }

  async load(): Promise<void> {
    if (!this.id) return;
    this.error.set(null);
    try {
      const s = await firstValueFrom(this.salesApi.getSale(this.id));
      this.saleDate = toInputDateTimeLocal(s.saleDate);
      this.customerId = s.customerId;
      this.customerName = s.customerName;
      this.branchId = s.branchId;
      this.branchName = s.branchName;
      this.saleCancelled = s.isCancelled;
      this.lines = (s.items ?? []).map((i) => ({
        key: i.id,
        productId: i.productId,
        productName: i.productName,
        quantity: i.quantity,
        unitPrice: i.unitPrice,
        isCancelled: i.isCancelled,
      }));
    } catch (e) {
      this.error.set(
        e instanceof ApiError
          ? e.message
          : e instanceof Error
            ? e.message
            : 'Não foi possível carregar'
      );
    } finally {
      this.loading.set(false);
    }
  }

  addLine(): void {
    this.lines = [...this.lines, emptyLine()];
  }

  removeLine(key: string): void {
    if (this.lines.length <= 1) return;
    this.lines = this.lines.filter((l) => l.key !== key);
  }

  validate(): string | null {
    if (!this.customerName.trim()) return 'Informe o nome do cliente';
    if (!this.branchName.trim()) return 'Informe o nome da filial';
    if (this.lines.length === 0) return 'Inclua ao menos um item';
    for (let i = 0; i < this.lines.length; i++) {
      const l = this.lines[i];
      if (!l.productName.trim())
        return `Item ${i + 1}: nome do produto obrigatório`;
      if (l.quantity < 1 || l.quantity > MAX_QTY) {
        return `Item ${i + 1}: quantidade entre 1 e ${MAX_QTY}`;
      }
      if (l.unitPrice < 0) return `Item ${i + 1}: preço inválido`;
    }
    return null;
  }

  async submit(): Promise<void> {
    const v = this.validate();
    if (v) {
      this.error.set(v);
      return;
    }
    this.saving.set(true);
    this.error.set(null);
    try {
      if (this.isEdit && this.id) {
        const body: UpdateSaleRequest = {
          saleDate: fromInputDateTimeLocal(this.saleDate),
          customerId: this.customerId,
          customerName: this.customerName.trim(),
          branchId: this.branchId,
          branchName: this.branchName.trim(),
          isCancelled: this.saleCancelled,
          items: this.lines.map((l) => ({
            productId: l.productId,
            productName: l.productName.trim(),
            quantity: l.quantity,
            unitPrice: l.unitPrice,
            isCancelled: l.isCancelled,
          })),
        };
        await firstValueFrom(this.salesApi.updateSale(this.id, body));
        await this.router.navigateByUrl(`/sales/${this.id}`);
      } else {
        const body: CreateSaleRequest = {
          saleDate: fromInputDateTimeLocal(this.saleDate),
          customerId: this.customerId,
          customerName: this.customerName.trim(),
          branchId: this.branchId,
          branchName: this.branchName.trim(),
          items: this.lines.map((l) => ({
            productId: l.productId,
            productName: l.productName.trim(),
            quantity: l.quantity,
            unitPrice: l.unitPrice,
          })),
        };
        const created = await firstValueFrom(this.salesApi.createSale(body));
        await this.router.navigateByUrl(`/sales/${created.id}`);
      }
    } catch (err) {
      this.error.set(
        err instanceof ApiError ? err.message : 'Falha ao salvar'
      );
    } finally {
      this.saving.set(false);
    }
  }

  back(): void {
    void this.router.navigateByUrl(
      this.isEdit && this.id ? `/sales/${this.id}` : '/sales'
    );
  }
}
