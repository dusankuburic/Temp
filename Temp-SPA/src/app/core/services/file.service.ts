import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { BlobDto, BlobResponse, FileType } from '../models/blob';

@Injectable({
  providedIn: 'root'
})
export class FileService {
  private baseUrl = environment.apiUrl + 'files';

  constructor(private http: HttpClient) {}

  listAll(): Observable<BlobDto[]> {
    return this.http.get<BlobDto[]>(this.baseUrl);
  }

  listImages(from?: Date, to?: Date): Observable<BlobDto[]> {
    let params = new HttpParams();
    if (from) params = params.append('from', from.toISOString());
    if (to) params = params.append('to', to.toISOString());
    return this.http.get<BlobDto[]>(`${this.baseUrl}/images`, { params });
  }

  listDocuments(from?: Date, to?: Date): Observable<BlobDto[]> {
    let params = new HttpParams();
    if (from) params = params.append('from', from.toISOString());
    if (to) params = params.append('to', to.toISOString());
    return this.http.get<BlobDto[]>(`${this.baseUrl}/documents`, { params });
  }

  upload(file: File): Observable<BlobResponse> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<BlobResponse>(this.baseUrl, formData);
  }

  uploadImage(file: File, createdAt?: Date): Observable<BlobResponse> {
    const formData = new FormData();
    formData.append('file', file);
    let params = new HttpParams();
    if (createdAt) params = params.append('createdAt', createdAt.toISOString());
    return this.http.post<BlobResponse>(`${this.baseUrl}/images`, formData, { params });
  }

  uploadDocument(file: File, createdAt?: Date): Observable<BlobResponse> {
    const formData = new FormData();
    formData.append('file', file);
    let params = new HttpParams();
    if (createdAt) params = params.append('createdAt', createdAt.toISOString());
    return this.http.post<BlobResponse>(`${this.baseUrl}/documents`, formData, { params });
  }

  uploadForEmployee(employeeId: number, file: File, fileType: FileType): Observable<BlobResponse> {
    const formData = new FormData();
    formData.append('file', file);

    if (fileType === 'Image') {
      return this.http.post<BlobResponse>(`${this.baseUrl}/images`, formData);
    }
    return this.http.post<BlobResponse>(`${this.baseUrl}/documents`, formData);
  }

  getDownloadUrl(path: string, expiresInMinutes = 60): Observable<{ url: string }> {
    let params = new HttpParams();
    params = params.append('path', path);
    params = params.append('expiresInMinutes', expiresInMinutes);
    return this.http.get<{ url: string }>(`${this.baseUrl}/url`, { params });
  }

  delete(path: string): Observable<BlobResponse> {
    let params = new HttpParams();
    params = params.append('path', path);
    return this.http.delete<BlobResponse>(this.baseUrl, { params });
  }

  exists(path: string): Observable<{ exists: boolean }> {
    let params = new HttpParams();
    params = params.append('path', path);
    return this.http.get<{ exists: boolean }>(`${this.baseUrl}/exists`, { params });
  }
}
