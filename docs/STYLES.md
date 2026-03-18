# Styling Guide

## Overview

The NooR Restaurant Management System uses a modern, responsive design approach with consistent styling across all components.

## Design System

### Color Palette

#### Primary Colors
- **Primary**: `#2563eb` (Blue 600)
- **Primary Light**: `#3b82f6` (Blue 500)
- **Primary Dark**: `#1d4ed8` (Blue 700)

#### Secondary Colors
- **Secondary**: `#059669` (Emerald 600)
- **Secondary Light**: `#10b981` (Emerald 500)
- **Secondary Dark**: `#047857` (Emerald 700)

#### Neutral Colors
- **Gray 50**: `#f9fafb`
- **Gray 100**: `#f3f4f6`
- **Gray 200**: `#e5e7eb`
- **Gray 300**: `#d1d5db`
- **Gray 400**: `#9ca3af`
- **Gray 500**: `#6b7280`
- **Gray 600**: `#4b5563`
- **Gray 700**: `#374151`
- **Gray 800**: `#1f2937`
- **Gray 900**: `#111827`

#### Status Colors
- **Success**: `#10b981` (Emerald 500)
- **Warning**: `#f59e0b` (Amber 500)
- **Error**: `#ef4444` (Red 500)
- **Info**: `#3b82f6` (Blue 500)

### Typography

#### Font Family
- **Primary**: `'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif`
- **Monospace**: `'Fira Code', 'Monaco', 'Consolas', monospace`

#### Font Sizes
- **xs**: `0.75rem` (12px)
- **sm**: `0.875rem` (14px)
- **base**: `1rem` (16px)
- **lg**: `1.125rem` (18px)
- **xl**: `1.25rem` (20px)
- **2xl**: `1.5rem` (24px)
- **3xl**: `1.875rem` (30px)
- **4xl**: `2.25rem` (36px)

#### Font Weights
- **Light**: `300`
- **Normal**: `400`
- **Medium**: `500`
- **Semibold**: `600`
- **Bold**: `700`

### Spacing Scale

```scss
$spacing: (
  0: 0,
  1: 0.25rem,    // 4px
  2: 0.5rem,     // 8px
  3: 0.75rem,    // 12px
  4: 1rem,       // 16px
  5: 1.25rem,    // 20px
  6: 1.5rem,     // 24px
  8: 2rem,       // 32px
  10: 2.5rem,    // 40px
  12: 3rem,      // 48px
  16: 4rem,      // 64px
  20: 5rem,      // 80px
  24: 6rem,      // 96px
);
```

## Component Styles

### Buttons

#### Primary Button
```scss
.btn-primary {
  background-color: #2563eb;
  color: white;
  padding: 0.75rem 1.5rem;
  border-radius: 0.5rem;
  font-weight: 500;
  transition: all 0.2s ease;
  
  &:hover {
    background-color: #1d4ed8;
    transform: translateY(-1px);
  }
  
  &:active {
    transform: translateY(0);
  }
}
```

#### Secondary Button
```scss
.btn-secondary {
  background-color: transparent;
  color: #2563eb;
  border: 2px solid #2563eb;
  padding: 0.75rem 1.5rem;
  border-radius: 0.5rem;
  font-weight: 500;
  transition: all 0.2s ease;
  
  &:hover {
    background-color: #2563eb;
    color: white;
  }
}
```

### Cards

```scss
.card {
  background: white;
  border-radius: 0.75rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  padding: 1.5rem;
  transition: box-shadow 0.2s ease;
  
  &:hover {
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  }
}
```

### Forms

#### Input Fields
```scss
.form-input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 2px solid #e5e7eb;
  border-radius: 0.5rem;
  font-size: 1rem;
  transition: border-color 0.2s ease;
  
  &:focus {
    outline: none;
    border-color: #2563eb;
    box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
  }
  
  &.error {
    border-color: #ef4444;
  }
}
```

#### Labels
```scss
.form-label {
  display: block;
  font-weight: 500;
  color: #374151;
  margin-bottom: 0.5rem;
}
```

## Responsive Design

### Breakpoints
```scss
$breakpoints: (
  sm: 640px,
  md: 768px,
  lg: 1024px,
  xl: 1280px,
  2xl: 1536px
);
```

### Grid System
```scss
.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
  
  @media (min-width: 640px) {
    padding: 0 2rem;
  }
}

.grid {
  display: grid;
  gap: 1.5rem;
  
  &.cols-1 { grid-template-columns: repeat(1, 1fr); }
  &.cols-2 { grid-template-columns: repeat(2, 1fr); }
  &.cols-3 { grid-template-columns: repeat(3, 1fr); }
  &.cols-4 { grid-template-columns: repeat(4, 1fr); }
  
  @media (max-width: 768px) {
    &.cols-2,
    &.cols-3,
    &.cols-4 {
      grid-template-columns: 1fr;
    }
  }
}
```

## Animation Guidelines

### Transitions
- Use `ease` or `ease-out` for most transitions
- Duration should be between `0.15s` and `0.3s`
- Avoid transitions longer than `0.5s`

### Hover Effects
```scss
.hover-lift {
  transition: transform 0.2s ease;
  
  &:hover {
    transform: translateY(-2px);
  }
}

.hover-scale {
  transition: transform 0.2s ease;
  
  &:hover {
    transform: scale(1.05);
  }
}
```

## Accessibility

### Focus States
```scss
.focus-visible {
  &:focus-visible {
    outline: 2px solid #2563eb;
    outline-offset: 2px;
  }
}
```

### Color Contrast
- Ensure minimum contrast ratio of 4.5:1 for normal text
- Ensure minimum contrast ratio of 3:1 for large text
- Use tools like WebAIM Color Contrast Checker

### Screen Reader Support
- Use semantic HTML elements
- Provide alt text for images
- Use ARIA labels where appropriate

## CSS Architecture

### File Organization
```
styles/
├── abstracts/
│   ├── _variables.scss
│   ├── _mixins.scss
│   └── _functions.scss
├── base/
│   ├── _reset.scss
│   ├── _typography.scss
│   └── _base.scss
├── components/
│   ├── _buttons.scss
│   ├── _cards.scss
│   ├── _forms.scss
│   └── _navigation.scss
├── layout/
│   ├── _header.scss
│   ├── _footer.scss
│   └── _grid.scss
├── pages/
│   ├── _home.scss
│   ├── _menu.scss
│   └── _checkout.scss
└── main.scss
```

### Naming Conventions
- Use BEM (Block Element Modifier) methodology
- Use kebab-case for class names
- Prefix utility classes with `u-`
- Prefix JavaScript hooks with `js-`

### Example BEM Structure
```scss
.restaurant-card {
  // Block styles
  
  &__image {
    // Element styles
  }
  
  &__title {
    // Element styles
  }
  
  &--featured {
    // Modifier styles
  }
}
```