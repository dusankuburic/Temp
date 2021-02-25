import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Organization } from '../_models/organization';

@Injectable({
  providedIn: 'root'
})
export class OrganizationService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }


getOrganizations(): any {
  return this.http.get<Organization[]>(this.baseUrl + 'organizations');
}

}
