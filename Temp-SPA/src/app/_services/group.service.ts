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

getGroup(id: number): any {
  return this.http.get<Group>(this.baseUrl + 'groups/' + id);
}

getGroups(organizationId: number): any {
  return this.http.get<Group[]>(this.baseUrl + 'organizations/inner-groups/' + organizationId);
}

createGroup(group: Group): any {
  return this.http.post(this.baseUrl + 'groups', group);
}

updateGroup(id: number, group: Group): any {
  return this.http.put(this.baseUrl + 'groups/' + id, group);
}

}