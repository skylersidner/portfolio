import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-ui-section',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="section" [attr.id]="sectionId() || null">
      <div class="section-header">
        @if (kicker()) {
          <p class="kicker">{{ kicker() }}</p>
        }

        <div>
          <h2>{{ title() }}</h2>
          @if (description()) {
            <p class="description">{{ description() }}</p>
          }
        </div>
      </div>

      <ng-content />
    </section>
  `,
  styles: [
    `
      .section {
        display: grid;
        gap: 1rem;
      }

      .section-header {
        display: grid;
        gap: 0.55rem;
      }

      .kicker {
        margin: 0;
        color: var(--accent-2);
        font-size: 0.78rem;
        font-weight: 700;
        letter-spacing: 0.14em;
        text-transform: uppercase;
      }

      h2 {
        margin: 0;
        font-size: clamp(1.5rem, 2vw, 2rem);
      }

      .description {
        margin: 0.15rem 0 0;
        max-width: 62ch;
        color: var(--muted);
      }
    `
  ]
})
export class UiSectionComponent {
  readonly sectionId = input('');
  readonly kicker = input('');
  readonly title = input.required<string>();
  readonly description = input('');
}
