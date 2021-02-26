import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root'
})
export class GroupService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getGroups(organizationId: number): any {
  return this.http.get<Group[]>(this.baseUrl + 'organizations/inner-groups/' + organizationId);
}

}
