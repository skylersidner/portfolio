import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-button-link',
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (routerLink()) {
      <a [routerLink]="routerLink()!" class="button" [class.button--primary]="variant() === 'primary'">
        {{ label() }}
      </a>
    } @else {
      <a
        class="button"
        [class.button--primary]="variant() === 'primary'"
        [href]="href()"
        [attr.target]="external() ? '_blank' : null"
        [attr.rel]="external() ? 'noreferrer' : null">
        {{ label() }}
      </a>
    }
  `,
  styles: [
    `
      .button {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        gap: 0.35rem;
        min-height: 2.75rem;
        padding: 0.65rem 1rem;
        border: 1px solid var(--line);
        border-radius: 999px;
        background: rgba(255, 255, 255, 0.04);
        color: var(--text);
        text-decoration: none;
        font-weight: 600;
        transition: transform 140ms ease, border-color 140ms ease, background 140ms ease;
      }

      .button:hover {
        transform: translateY(-1px);
        border-color: rgba(244, 210, 139, 0.45);
      }

      .button--primary {
        color: #120f1d;
        border-color: transparent;
        background: linear-gradient(135deg, var(--accent), var(--accent-2));
      }
    `
  ]
})
export class ButtonLinkComponent {
  readonly label = input.required<string>();
  readonly href = input('#');
  readonly routerLink = input<string | null>(null);
  readonly variant = input<'primary' | 'secondary'>('secondary');
  readonly external = input(false);
}
