import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvService } from '../../shared/env.service';
import { catchError, map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FileManagerService {

  private readonly baseUrl: string = this.env.apiUrl;

  constructor(
    private httpClient: HttpClient,
    private env: EnvService) {}
  
  public scan(file: File) {
    const endpoint = `${this.baseUrl}/antivirus/scan`;
    const formData = new FormData(); 
    formData.append('file', file, file.name);
    return this.httpClient.post<{}>(endpoint, formData).pipe(
      map(() => true),
      catchError(() => of(false))
    );
  }
}