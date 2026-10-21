import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideRouter([])],
    }).compileComponents();
  });

  it('se crea la aplicación', () => {
    const fixture = TestBed.createComponent(App);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('muestra el menú con las cuatro secciones', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const enlaces = (fixture.nativeElement as HTMLElement).querySelectorAll('nav a');
    expect(enlaces.length).toBe(4);
    expect(enlaces[0].textContent).toContain('Panel');
  });
});
