import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';

interface OrderLine {
  product: string;
  unit: string;
  quantity: number;
}

@Component({
  selector: 'app-order-creation',
  templateUrl: './order-creation.html',
  styleUrl: './order-creation.scss',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatTableModule,
  ],
})
export class OrderCreation {
  private readonly fb = new FormBuilder();

  readonly customers = ['Acme Industries', 'Brighton Tools', 'Northwind Hardware'];
  readonly products = ['Hammer', 'Screwdriver', 'Wrench', 'Drill'];
  readonly units = ['pcs', 'box', 'set'];

  readonly orderForm = this.fb.nonNullable.group({
    customer: ['', Validators.required],
  });

  readonly lineForm = this.fb.nonNullable.group({
    product: ['', Validators.required],
    unit: ['', Validators.required],
    quantity: [1, [Validators.required, Validators.min(1)]],
  });

  readonly displayedColumns = ['product', 'unit', 'quantity'];
  lines: OrderLine[] = [];

  addLine() {
    if (this.lineForm.invalid) {
      this.lineForm.markAllAsTouched();
      return;
    }

    const { product, unit, quantity } = this.lineForm.getRawValue();
    this.lines = [...this.lines, { product, unit, quantity }];
    this.lineForm.reset({ product: '', unit: '', quantity: 1 });
  }
}
