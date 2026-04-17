import { ChangeDetectionStrategy, Component, input } from '@angular/core';

import { ButtonLinkComponent } from './button-link.component';
import { ProjectCardModel } from '../../core/portfolio.store';

@Component({
  selector: 'app-project-card',
  imports: [ButtonLinkComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <article class="card panel">
      <div class="meta-row">
        <span class="stage">{{ project().stage }}</span>
        <span class="focus">{{ project().focus }}</span>
      </div>

      <h3>{{ project().title }}</h3>
      <p>{{ project().summary }}</p>

      <div class="tags">
        @for (tag of project().tags; track tag) {
          <span class="tag">{{ tag }}</span>
        }
      </div>

      <div class="actions">
        @for (link of project().links; track link.label) {
          <app-button-link [label]="link.label" [href]="link.href" [variant]="link.kind ?? 'secondary'" />
        }
      </div>
    </article>
  `,
  styles: [
    `
      .card {
        display: grid;
        gap: 0.9rem;
        padding: 1rem;
      }

      .meta-row,
      .tags,
      .actions {
        display: flex;
        flex-wrap: wrap;
        gap: 0.5rem;
      }

      .stage,
      .focus,
      .tag {
        border: 1px solid var(--line);
        border-radius: 999px;
        padding: 0.35rem 0.65rem;
        color: var(--muted);
        background: rgba(255, 255, 255, 0.04);
        font-size: 0.85rem;
      }

      .stage {
        color: var(--accent-2);
      }

      h3 {
        margin: 0;
        font-size: 1.15rem;
      }

      p {
        margin: 0;
        color: var(--muted);
      }
    `
  ]
})
export class ProjectCardComponent {
  readonly project = input.required<ProjectCardModel>();
}
