import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Workplace } from '../_models/workplace';

@Injectable({
  providedIn: 'root'
})
export class WorkplaceService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }


getWorkplaces(): any {
  return this.http.get<Workplace[]>(this.baseUrl + 'workplaces');
}



}


