# Angular Migration Plan: v17.1 → Latest Version

## Current State
- **Angular Version**: 17.1.0
- **TypeScript**: 5.3.2
- **RxJS**: 7.8.0
- **Architecture**: NgModule-based (no standalone components)

## Target State
- **Angular Version**: 18.x (or 19.x if available)
- **TypeScript**: Compatible version
- **Architecture**: Hybrid (NgModule with option to migrate to standalone)

---

## Phase 1: Pre-Migration Fixes (CRITICAL - Must Complete First)

### 1.1 Enable TypeScript Strict Mode ⚠️ HIGH PRIORITY
**File**: `tsconfig.json`
- [ ] Change `"strict": false` to `"strict": true`
- [ ] Fix all resulting compilation errors
- **Impact**: ~38 type errors to resolve across 25 files

### 1.2 Fix 'any' Type Usage (38 instances)
**Critical Files**:
- [ ] `src/app/core/services/auth.service.ts` (6 instances)
  - `decodedToken: any` → `decodedToken: JwtPayload | null`
  - `login(model: any)` → `login(model: LoginDto)`
  - `logout(): any` → `logout(): Observable<void>`
- [ ] `src/app/nav/nav.component.ts` (2 instances)
- [ ] All service files with untyped return values

### 1.3 Fix Deprecated RxJS Patterns
**Files to Update**:
- [ ] `src/app/application/application-moderator-list/application-moderator-list.component.ts:51`
  ```typescript
  // BEFORE:
  .toPromise().then((res: ModeratorListApplication[]) => { ... })

  // AFTER:
  const res = await firstValueFrom(this.service.call());
  // use res directly
  ```

- [ ] `src/app/employee/employee-edit/employee-edit.component.ts` (lines 109, 113, 117, 124)
- [ ] `src/app/employee/employee-edit-modal/employee-edit-modal.component.ts` (lines 130, 135, 138, 145)
  ```typescript
  // BEFORE:
  await lastValueFrom(...).then((data) => { ... })

  // AFTER:
  const data = await lastValueFrom(...);
  // use data directly
  ```

### 1.4 Add Interface Implementations to Guards
**Files to Update**:
- [ ] `src/app/core/guards/auth.guard.ts`
- [ ] `src/app/core/guards/moderator.guard.ts`
- [ ] `src/app/core/guards/user.guard.ts`
  ```typescript
  // BEFORE:
  export class AuthGuard { ... }

  // AFTER:
  export class AuthGuard implements CanActivate { ... }
  ```

### 1.5 Add Interface Implementations to Resolvers (20+ files)
**Pattern to Apply**:
```typescript
// BEFORE:
export class EmployeeListResolver {
  resolve(): Observable<PaginatedResult<Employee[]>> { ... }
}

// AFTER:
import { Resolve } from '@angular/router';

export class EmployeeListResolver implements Resolve<PaginatedResult<Employee[]>> {
  resolve(): Observable<PaginatedResult<Employee[]>> { ... }
}
```

**All Resolver Files** (src/app/core/resolvers/):
- [ ] application/application-create.resolver.ts
- [ ] application/application-moderator-list.resolver.ts
- [ ] application/application-user-list.resolver.ts
- [ ] employee/assigned-groups.resolver.ts
- [ ] employee/employee-edit.resolver.ts
- [ ] employee/employee-list.resolver.ts
- [ ] employment-status/employment-status-edit.resolver.ts
- [ ] employment-status/employment-status-list.resolver.ts
- [ ] engagement/engagement-create.resolver.ts
- [ ] engagement/engagement-user-list.resolver.ts
- [ ] engagement/engagement-with-employee-list.resolver.ts
- [ ] engagement/engagement-without-employee-list.resolver.ts
- [ ] group/assigned-inner-teams.resolver.ts
- [ ] group/group-edit.resolver.ts
- [ ] group/inner-group-list.resolver.ts
- [ ] organization/organization-edit.resolver.ts
- [ ] organization/organization-list.resolver.ts
- [ ] team/team-edit.resolver.ts
- [ ] team/inner-team-list.resolver.ts
- [ ] workplace/workplace-edit.resolver.ts
- [ ] workplace/workplace-list.resolver.ts

---

## Phase 2: Dependency Updates

### 2.1 Update Angular CLI
```bash
npm install -g @angular/cli@latest
```

### 2.2 Run Angular Update Command
```bash
ng update @angular/core@18 @angular/cli@18
```

### 2.3 Update ngx-bootstrap (if needed)
```bash
npm update ngx-bootstrap
```

### 2.4 Update Other Dependencies
```bash
npm update @auth0/angular-jwt
npm update @fortawesome/angular-fontawesome
npm update rxjs
```

---

## Phase 3: Code Migration

### 3.1 Update Component Imports (Auto-handled by ng update)
- Router imports may change
- HttpClient imports should remain stable

### 3.2 Verify Interceptors
- [ ] Test error.interceptor.ts
- [ ] Test auth.interceptor.ts
- Ensure they work with updated HttpClient

### 3.3 Update Route Configuration (if needed)
- Verify all lazy-loaded routes work
- Test all guards and resolvers

---

## Phase 4: Testing & Validation

### 4.1 Unit Tests
- [ ] Run `npm test`
- [ ] Fix any broken tests
- [ ] Add tests for modified code

### 4.2 Build Verification
- [ ] Run `npm run build`
- [ ] Verify no build errors
- [ ] Check bundle sizes

### 4.3 Manual Testing
- [ ] Login/Authentication flow
- [ ] CRUD operations for all entities
- [ ] Navigation and routing
- [ ] Modal dialogs (ngx-bootstrap)
- [ ] Form validation
- [ ] Error handling

### 4.4 Docker Build
- [ ] Rebuild Docker image
- [ ] Test production build
- [ ] Verify API connectivity

---

## Phase 5: Optional Enhancements

### 5.1 Migrate to Standalone Components (Angular 14+ Feature)
- Start with new components
- Gradually migrate existing components
- Remove NgModule declarations

### 5.2 Enable Additional Strict Checks
```json
{
  "compilerOptions": {
    "noImplicitReturns": true,      // ✅ Already enabled
    "noFallthroughCasesInSwitch": true,  // ✅ Already enabled
    "noUnusedLocals": true,         // Add this
    "noUnusedParameters": true,     // Add this
    "noImplicitOverride": true      // Add this
  }
}
```

### 5.3 Modern Angular Features
- Consider using inject() function (Angular 14+)
- Use new control flow syntax (@if, @for) in Angular 17+
- Migrate to signals (Angular 16+)

---

## Migration Checklist Summary

### Pre-Migration (Must Complete)
- [ ] Enable strict mode in tsconfig.json
- [ ] Fix all 'any' types (38 instances)
- [ ] Replace toPromise() with firstValueFrom/lastValueFrom
- [ ] Remove .then() chains after firstValueFrom/lastValueFrom
- [ ] Add CanActivate interface to guards (3 files)
- [ ] Add Resolve<T> interface to resolvers (20+ files)

### During Migration
- [ ] Backup current working code
- [ ] Run `ng update @angular/core@18 @angular/cli@18`
- [ ] Fix any migration warnings
- [ ] Update package.json dependencies
- [ ] Run `npm install`

### Post-Migration
- [ ] Run all tests
- [ ] Build production bundle
- [ ] Test in Docker
- [ ] Manual QA testing
- [ ] Update documentation

---

## Estimated Timeline

| Phase | Effort | Duration |
|-------|--------|----------|
| Phase 1: Pre-Migration Fixes | High | 3-5 days |
| Phase 2: Dependency Updates | Low | 1 day |
| Phase 3: Code Migration | Medium | 2-3 days |
| Phase 4: Testing | High | 3-4 days |
| Phase 5: Optional | Low | 1-2 days |
| **Total** | | **10-15 days** |

---

## Risk Assessment

| Risk | Severity | Mitigation |
|------|----------|------------|
| ngx-bootstrap incompatibility | Medium | Test early, have fallback plan |
| Type errors from strict mode | High | Fix incrementally, test frequently |
| Breaking changes in Angular | Low | Follow official migration guide |
| Production downtime | Low | Test thoroughly before deploy |

---

## Rollback Plan

1. Keep current package.json as package.json.backup
2. Commit working state before migration
3. Tag release: `git tag pre-angular-18-migration`
4. If issues occur:
   ```bash
   git checkout pre-angular-18-migration
   npm install
   docker compose build
   ```

---

## Next Steps

1. **Start with Phase 1.1**: Enable strict mode
2. **Fix compilation errors**: Address type issues one file at a time
3. **Test incrementally**: Don't fix everything before testing
4. **Document issues**: Track problems in this file
5. **Commit frequently**: Small, focused commits

---

## Resources

- [Angular Update Guide](https://update.angular.io/)
- [Angular 18 Release Notes](https://blog.angular.io/angular-v18-is-now-available-e79d5ac0affe)
- [RxJS Migration Guide](https://rxjs.dev/deprecations/to-promise)
- [TypeScript Strict Mode](https://www.typescriptlang.org/tsconfig#strict)
