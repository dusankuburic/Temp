import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UploadService {
  private baseUrl = environment.apiUrl + '/uploads';

  constructor(private http: HttpClient) {}

  uploadFile(file: File, reportProgress = false): Observable<any> {
    return this.getSasToken(file.name).pipe(
      switchMap(response => {
        const sasUrl = response.url;
        const headers = new HttpHeaders({
          'x-ms-blob-type': 'BlockBlob',
          'Content-Type': file.type || 'application/octet-stream'
        });

        const options: any = {
          headers,
          reportProgress
        };
        
        if (reportProgress) {
          options.observe = 'events';
        }

        return this.http.put(sasUrl, file, options);
      })
    );
  }

  private getSasToken(filename: string): Observable<{ url: string }> {
    return this.http.post<{ url: string }>(`${this.baseUrl}/signed-url`, { filename });
  }
}
