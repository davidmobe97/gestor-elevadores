import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { Shell } from './shell';

describe('Shell', () => {
  beforeEach(async () => {
    localStorage.clear();
    await TestBed.configureTestingModule({
      imports: [Shell],
      providers: [provideRouter([]), provideHttpClient()],
    }).compileComponents();
  });

  it('muestra el menú con las cuatro secciones', async () => {
    const fixture = TestBed.createComponent(Shell);
    await fixture.whenStable();
    const enlaces = (fixture.nativeElement as HTMLElement).querySelectorAll('nav a');
    expect(enlaces.length).toBe(4);
    expect(enlaces[0].textContent).toContain('Panel');
  });

  it('muestra el usuario de la sesión guardada', async () => {
    const futuro = new Date(Date.now() + 3600_000).toISOString();
    localStorage.setItem('gestor-elevadores.sesion', JSON.stringify(
      { token: 'x', expira: futuro, nombre: 'Ana Admin', email: 'a@a.es', rol: 'Administrador' }));
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [Shell],
      providers: [provideRouter([]), provideHttpClient()],
    }).compileComponents();

    const fixture = TestBed.createComponent(Shell);
    await fixture.whenStable();
    expect((fixture.nativeElement as HTMLElement).querySelector('.usuario')?.textContent).toContain('Ana Admin');
  });
});
