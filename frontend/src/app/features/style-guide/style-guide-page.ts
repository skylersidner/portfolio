import { ChangeDetectionStrategy, Component } from '@angular/core';

import { ButtonLinkComponent } from '../../shared/ui/button-link.component';
import { ProjectCardComponent } from '../../shared/ui/project-card.component';
import { UiSectionComponent } from '../../shared/ui/section-shell.component';
import { ProjectCardModel } from '../../core/portfolio.store';

@Component({
  selector: 'app-style-guide-page',
  imports: [UiSectionComponent, ButtonLinkComponent, ProjectCardComponent],
  templateUrl: './style-guide-page.html',
  styleUrl: './style-guide-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StyleGuidePageComponent {
  protected readonly sampleProject: ProjectCardModel = {
    title: 'Sample project card',
    summary: 'A reusable preview state for validating layout, tone, spacing, and CTA treatments during development.',
    stage: 'Preview only',
    focus: 'Experience',
    tags: ['Non-production', 'UI Primitive', 'Review'],
    links: [{ label: 'Primary action', href: '#', kind: 'primary' }]
  };
}
