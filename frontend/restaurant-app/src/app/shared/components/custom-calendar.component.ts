import { Component, Input, forwardRef, signal, HostListener, ElementRef, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { IconComponent } from './icon.component';

@Component({
  selector: 'app-custom-calendar',
  standalone: true,
  imports: [CommonModule, IconComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CustomCalendarComponent),
      multi: true
    }
  ],
  template: `
    <div class="calendar-container" [class.open]="isOpen()">
      <div class="calendar-trigger" (click)="toggleCalendar()">
        <span class="value-label">{{ value() ? (value() | date:'MM/dd/yyyy') : placeholder }}</span>
        <app-icon name="calendar" class="calendar-icon"></app-icon>
      </div>

      <div class="calendar-popover" *ngIf="isOpen()">
        <!-- Header -->
        <div class="calendar-header">
          <button type="button" class="nav-btn" (click)="prevMonth()">
            <app-icon name="chevronLeft"></app-icon>
          </button>
          <span class="month-label">{{ viewingDate() | date:'MMMM yyyy' }}</span>
          <button type="button" class="nav-btn" (click)="nextMonth()">
            <app-icon name="chevronRight"></app-icon>
          </button>
        </div>

        <!-- Weekdays -->
        <div class="weekdays">
          <span *ngFor="let day of weekDays">{{ day }}</span>
        </div>

        <!-- Days Grid -->
        <div class="days-grid">
          <button type="button" 
                  *ngFor="let day of days()" 
                  [disabled]="day.disabled"
                  [class.current-month]="day.currentMonth"
                  [class.selected]="isSelected(day.date)"
                  [class.today]="isToday(day.date)"
                  (click)="selectDate(day.date)"
                  class="day-btn">
            {{ day.date.getDate() }}
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; width: 100%; }
    .calendar-container { position: relative; width: 100%; }
    
    .calendar-trigger {
      width: 100%;
      padding: 0.85rem 1.25rem;
      background: var(--color-surface-2);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      color: var(--color-text);
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: space-between;
      transition: all var(--transition-base);
    }
    .calendar-trigger:hover { border-color: rgba(230,126,34,0.4); background: var(--color-surface-3); }
    .calendar-container.open .calendar-trigger { border-color: var(--color-accent); box-shadow: 0 0 0 3px rgba(230,126,34,0.15); }
    
    .calendar-icon { color: var(--color-accent); font-size: 1.2rem; }

    .calendar-popover {
      position: absolute;
      top: calc(100% + var(--space-2));
      left: 0;
      background: var(--color-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-lg);
      box-shadow: var(--shadow-lg);
      padding: var(--space-4);
      z-index: 1000;
      width: 300px;
      animation: dropdownFade 0.2s ease forwards;
    }

    .calendar-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: var(--space-4);
    }
    .month-label { font-family: var(--font-serif); font-weight: 700; color: var(--color-text); }
    .nav-btn {
      width: 32px; height: 32px; border-radius: 50%;
      display: flex; align-items: center; justify-content: center;
      color: var(--color-text-muted); transition: all 0.2s;
    }
    .nav-btn:hover { background: var(--color-surface-2); color: var(--color-accent); }

    .weekdays {
      display: grid;
      grid-template-columns: repeat(7, 1fr);
      text-align: center;
      margin-bottom: var(--space-2);
    }
    .weekdays span {
      font-size: 0.7rem;
      font-weight: 700;
      text-transform: uppercase;
      color: var(--color-text-dim);
      letter-spacing: 0.05em;
    }

    .days-grid {
      display: grid;
      grid-template-columns: repeat(7, 1fr);
      gap: 2px;
    }
    .day-btn {
      aspect-ratio: 1;
      border-radius: var(--radius-sm);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 0.85rem;
      color: var(--color-text-dim);
      transition: all 0.2s;
      background: transparent;
    }
    .day-btn:disabled { opacity: 0.2; cursor: not-allowed; }
    .day-btn.current-month { color: var(--color-text-muted); }
    .day-btn:hover:not(:disabled) { background: var(--color-surface-2); color: var(--color-text); }
    
    .day-btn.selected { background: var(--color-accent) !important; color: white !important; font-weight: 700; }
    .day-btn.today { border: 1px solid var(--color-accent); color: var(--color-accent); }

    @keyframes dropdownFade {
      from { opacity: 0; transform: translateY(-10px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `]
})
export class CustomCalendarComponent implements ControlValueAccessor {
  @Input() placeholder = 'Select Date';
  @Input() minDate?: string;

  private el = inject(ElementRef);

  isOpen = signal(false);
  value = signal<Date | null>(null);
  viewingDate = signal(new Date());

  weekDays = ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'];

  days = computed(() => {
    const date = this.viewingDate();
    const year = date.getFullYear();
    const month = date.getMonth();
    
    const firstDayOfMonth = new Date(year, month, 1).getDay();
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    
    const prevMonthDays = new Date(year, month, 0).getDate();
    
    const calendarDays = [];
    
    // Previous month filler days
    for (let i = firstDayOfMonth - 1; i >= 0; i--) {
      const d = new Date(year, month - 1, prevMonthDays - i);
      calendarDays.push({ date: d, currentMonth: false, disabled: this.isDateDisabled(d) });
    }
    
    // Current month days
    for (let i = 1; i <= daysInMonth; i++) {
      const d = new Date(year, month, i);
      calendarDays.push({ date: d, currentMonth: true, disabled: this.isDateDisabled(d) });
    }
    
    // Next month filler days
    const totalDaysSoFar = calendarDays.length;
    for (let i = 1; i <= (42 - totalDaysSoFar); i++) {
      const d = new Date(year, month + 1, i);
      calendarDays.push({ date: d, currentMonth: false, disabled: this.isDateDisabled(d) });
    }
    
    return calendarDays;
  });

  private onChange: any = () => {};
  private onTouched: any = () => {};

  toggleCalendar() {
    this.isOpen.set(!this.isOpen());
    if (this.isOpen() && this.value()) {
      this.viewingDate.set(new Date(this.value()!));
    }
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.el.nativeElement.contains(event.target)) {
      this.isOpen.set(false);
    }
  }

  prevMonth() {
    const d = this.viewingDate();
    this.viewingDate.set(new Date(d.getFullYear(), d.getMonth() - 1, 1));
  }

  nextMonth() {
    const d = this.viewingDate();
    this.viewingDate.set(new Date(d.getFullYear(), d.getMonth() + 1, 1));
  }

  selectDate(d: Date) {
    this.value.set(d);
    // Convert to YYYY-MM-DD string as standard date input uses this
    const dateStr = d.toISOString().split('T')[0];
    this.onChange(dateStr);
    this.onTouched();
    this.isOpen.set(false);
  }

  isSelected(d: Date): boolean {
    if (!this.value()) return false;
    const v = this.value()!;
    return d.getDate() === v.getDate() && d.getMonth() === v.getMonth() && d.getFullYear() === v.getFullYear();
  }

  isToday(d: Date): boolean {
    const today = new Date();
    return d.getDate() === today.getDate() && d.getMonth() === today.getMonth() && d.getFullYear() === today.getFullYear();
  }

  isDateDisabled(d: Date): boolean {
    if (!this.minDate) return false;
    const min = new Date(this.minDate);
    min.setHours(0,0,0,0);
    const dateToCheck = new Date(d);
    dateToCheck.setHours(0,0,0,0);
    return dateToCheck < min;
  }

  // ControlValueAccessor methods
  writeValue(value: any): void {
    if (value) {
      this.value.set(new Date(value));
    } else {
      this.value.set(null);
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
}
