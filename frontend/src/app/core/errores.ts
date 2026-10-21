import { HttpErrorResponse } from '@angular/common/http';

/**
 * Convierte un error de la API en un texto para mostrar al usuario.
 * La API devuelve { mensaje } en sus reglas de negocio (409, 400)
 * y un objeto "errors" cuando falla la validación de los campos.
 */
export function textoError(err: unknown): string {
  if (err instanceof HttpErrorResponse) {
    if (err.status === 0) return 'No se puede conectar con la API. ¿Está arrancada en el puerto 5214?';
    const cuerpo = err.error;
    if (cuerpo?.mensaje) return cuerpo.mensaje;
    if (cuerpo?.errors) return Object.values(cuerpo.errors).flat().join(' ');
    if (err.status === 404) return 'No se ha encontrado el registro.';
  }
  return 'Ha ocurrido un error inesperado.';
}
