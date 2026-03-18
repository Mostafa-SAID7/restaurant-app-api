import { Component, Input, forwardRef, signal, HostListener, ElementRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { IconComponent } from './icon.component';

@Component({
  selector: 'app-custom-select',
  standalone: true,
  imports: [CommonModule, IconComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CustomSelectComponent),
      multi: true
    }
  ],
  template: `
    <div class="custom-select-container" [class.open]="isOpen()">
      <div class="select-trigger" (click)="toggleDropdown()" [class.has-value]="value()">
        <span class="selected-label">{{ selectedLabel() || placeholder }}</span>
        <app-icon name="chevronDown" class="select-arrow" [class.rotated]="isOpen()"></app-icon>
      </div>

      <div class="select-dropdown" *ngIf="isOpen()">
        <ul class="select-options custom-scrollbar">
          <li *ngFor="let option of options" 
              (click)="selectOption(option)"
              [class.selected]="isSelected(option)"
              class="select-option">
            {{ getLabel(option) }}
          </li>
        </ul>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; width: 100%; }
    .custom-select-container { position: relative; width: 100%; }
    .select-trigger {
      width: 100%;
      padding: 0.85rem 1.25rem;
      background: var(--color-surface-2);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      color: var(--color-text-muted);
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: space-between;
      transition: all var(--transition-base);
      user-select: none;
    }
    .select-trigger.has-value { color: var(--color-text); }
    .select-trigger:hover { border-color: rgba(230,126,34,0.4); background: var(--color-surface-3); }
    .custom-select-container.open .select-trigger { border-color: var(--color-accent); box-shadow: 0 0 0 3px rgba(230,126,34,0.15); }

    .select-arrow { font-size: 1.2rem; color: var(--color-accent); transition: transform var(--transition-base); }
    .select-arrow.rotated { transform: rotate(180deg); }

    .select-dropdown {
      position: absolute;
      top: calc(100% + var(--space-2));
      left: 0;
      right: 0;
      background: var(--color-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      box-shadow: var(--shadow-lg);
      z-index: 1000;
      animation: dropdownFade 0.2s ease forwards;
    }

    .select-options {
      max-height: 240px;
      overflow-y: auto;
      padding: var(--space-2);
      margin: 0;
    }

    .select-option {
      padding: var(--space-3) var(--space-4);
      border-radius: var(--radius-sm);
      cursor: pointer;
      transition: all var(--transition-fast);
      font-size: 0.9rem;
      color: var(--color-text-muted);
    }

    .select-option:hover { background: var(--color-surface-2); color: var(--color-text); }
    .select-option.selected { background: var(--color-accent-muted); color: var(--color-accent); font-weight: 600; }

    @keyframes dropdownFade {
      from { opacity: 0; transform: translateY(-10px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `]
})
export class CustomSelectComponent implements ControlValueAccessor {
  @Input() options: any[] = [];
  @Input() labelKey?: string;
  @Input() valueKey?: string;
  @Input() placeholder: string = 'Select an option';

  private el = inject(ElementRef);
  
  isOpen = signal(false);
  value = signal<any>(null);

  protected onChange: any = () => {};
  protected onTouched: any = () => {};

  toggleDropdown() {
    this.isOpen.set(!this.isOpen());
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.el.nativeElement.contains(event.target)) {
      this.isOpen.set(false);
    }
  }

  selectOption(option: any) {
    const val = this.getValue(option);
    this.value.set(val);
    this.onChange(val);
    this.onTouched();
    this.isOpen.set(false);
  }

  getLabel(option: any): string {
    if (this.labelKey && typeof option === 'object') return option[this.labelKey];
    return option.toString();
  }

  getValue(option: any): any {
    if (this.valueKey && typeof option === 'object') return option[this.valueKey];
    return option;
  }

  selectedLabel(): string {
    const val = this.value();
    if (val === null || val === undefined) return '';
    
    const found = this.options.find(o => this.getValue(o) === val);
    return found ? this.getLabel(found) : val.toString();
  }

  isSelected(option: any): boolean {
    return this.getValue(option) === this.value();
  }

  // ControlValueAccessor methods
  writeValue(value: any): void {
    this.value.set(value);
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    // TODO: implement disabled state if needed
  }
}
