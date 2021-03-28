import { Injectable } from '@angular/core';
import { Employee } from './employee';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

  private employeesUrl = 'https://localhost:44319/api/employees';

  constructor(private http: HttpClient) { }

  // getEmployees(): Employee[] {

  //   let emps: Employee[] = [
  //     { "empno": 1001, "ename": "Jag", "salary": 4500, "deptno": 20 },
  //     { "empno": 1002, "ename": "Chat", "salary": 2500, "deptno": 10 }
  //   ];

  //   return emps;
  // }

  getEmployees(): Observable<Employee[]> {
    return this.http.get<Employee[]>(this.employeesUrl);
  }

}
