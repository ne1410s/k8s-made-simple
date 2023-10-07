import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class FileManagerService {

  private readonly baseUrl: string = 'https://fileman.local.ne1410s.co.uk';

  constructor(private httpClient: HttpClient) {}
  
  public scan(file: File) {
    const endpoint = `${this.baseUrl}/antivirus/scan`;
    const formData = new FormData(); 
    formData.append('file', file, file.name);
    return this.httpClient.post<boolean>(endpoint, formData);
  }
}