import { Injectable, computed, signal } from '@angular/core';

export type FocusArea = 'All' | 'Platform' | 'AI' | 'Experience';

export interface ProjectLink {
  label: string;
  href: string;
  kind?: 'primary' | 'secondary';
}

export interface ProjectCardModel {
  title: string;
  summary: string;
  stage: string;
  focus: Exclude<FocusArea, 'All'>;
  tags: string[];
  links: ProjectLink[];
}

@Injectable({ providedIn: 'root' })
export class PortfolioStore {
  readonly intro = {
    eyebrow: 'Quiet Constellation Hybrid',
    headline: 'Project-first portfolio foundation with a calm constellation feel.',
    summary:
      'A minimal Angular front end focused on featured work, thoughtful presentation, and room for a future full-stack platform.'
  };

  readonly heroStats = signal([
    { value: 'Phase 0-2', label: 'foundation in progress' },
    { value: 'Angular 21', label: 'latest stable scaffold' },
    { value: 'Signals', label: 'state-ready UI patterns' }
  ]);

  readonly principles = signal([
    {
      title: 'Projects lead the story',
      body: 'The first screen emphasizes featured work and product thinking before resume material.'
    },
    {
      title: 'Reusable pieces first',
      body: 'Shared sections, calls to action, and project cards provide a clean base for later features.'
    },
    {
      title: 'Ready for evolution',
      body: 'The structure stays small now while leaving a clear path for a future BFF and content APIs.'
    }
  ]);

  readonly focusAreas: FocusArea[] = ['All', 'Platform', 'AI', 'Experience'];

  private readonly selectedFocusState = signal<FocusArea>('All');

  private readonly projectsState = signal<ProjectCardModel[]>([
    {
      title: 'Portfolio Platform',
      summary: 'The root experience that introduces work clearly, routes visitors to projects, and keeps the product narrative cohesive.',
      stage: 'Foundation',
      focus: 'Platform',
      tags: ['Angular', 'Design System', 'Routing'],
      links: [{ label: 'View app direction', href: '#principles', kind: 'primary' }]
    },
    {
      title: 'AI Workflow Experiments',
      summary: 'A space for agent-assisted tools and practical experiments that translate ideas into real product flows.',
      stage: 'Featured concept',
      focus: 'AI',
      tags: ['AI', 'Automation', 'Product UX'],
      links: [{ label: 'See build approach', href: '#featured-work' }]
    },
    {
      title: 'Experience Notes',
      summary: 'Short case-study style summaries that turn past work into readable, project-centered evidence.',
      stage: 'Planned',
      focus: 'Experience',
      tags: ['Case Studies', 'Storytelling', 'Frontend'],
      links: [{ label: 'Contact section', href: '#contact' }]
    }
  ]);

  readonly selectedFocus = this.selectedFocusState.asReadonly();

  readonly visibleProjects = computed(() => {
    const focus = this.selectedFocusState();
    const projects = this.projectsState();

    return focus === 'All' ? projects : projects.filter((project) => project.focus === focus);
  });

  setFocus(focus: FocusArea): void {
    this.selectedFocusState.set(focus);
  }
}
